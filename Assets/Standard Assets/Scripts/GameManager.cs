using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Cinemachine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using System.Diagnostics;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static RoleControl Player;
	public static StoryListener StoryListener => Instance.GetComponent<StoryListener>();
	public static UIManager GameUI;
	public static KeyValueData Config => Instance.DemoData;
	public LightManager DayNight;
	public CinemachineVirtualCamera VCamera;
	public CameraController CameraCtrl;
	public SFXManager SFXManager;
	public TerrainTool TerrainTool;
	public Transform buildModeFollow;
	public static GameObject CurrentSlected
	{
		get { return m_CurrentSelected; }
		set
		{
			m_CurrentSelected?.GetComponent<HighlightableObject>()?.Dehighlight();
			m_CurrentSelected = value;
			m_CurrentSelected?.GetComponent<HighlightableObject>()?.Highlight();
			Helpers.Log(Instance, "PickObject", "target= " + m_CurrentSelected.name);
		}
	}
	static GameObject m_CurrentSelected;

	public static bool BuildMode
	{
		get { return m_buildMode; }
		set
		{
			m_buildMode = value;
			GameManager.GameUI.JoyStick.enabled = !value;
			Instance.buildModeFollow.position = Player.transform.position;
			if (value)
			{
				Instance.CameraCtrl.SetMode(CameraController.Mode.BUILD);
			}
			else
			{
				Instance.CameraCtrl.SetMode(CameraController.Mode.RPG);
			}
		}
	}
	public static bool m_buildMode;
	public static bool StoryMode
	{
		get { return m_storyMode; }
		set
		{
			m_storyMode = value;
			GameManager.GameUI.JoyStick.enabled = !value;
			if (value)
			{
				Instance.CameraCtrl.SetMode(CameraController.Mode.STORY);
			}
			else
			{
				Instance.CameraCtrl.SetMode(CameraController.Mode.RPG);
			}
		}
	}
	public static bool m_storyMode;

	[SerializeField]
	Transform ui_trans;
	[SerializeField]
	Vector3 position_born;
	[SerializeField]
	Vector2 baseBoundary_LB;
	[SerializeField]
	Vector2 baseBoundary_RT;
	[SerializeField]
	GameObject prefab_character;
	[SerializeField]
	GameObject prefab_gameui;

	[SerializeField]
	KeyValueData DemoData;
	InventorySystem.ItemDemand testDemand;
	Vector3 m_dragMousePos = Vector3.zero;
	Vector3 m_revisCampos = Vector3.zero;
	private void Awake()
	{
		Instance = this;
		Player = Instantiate(prefab_character, position_born, Quaternion.Euler(0, 180, 0)).GetComponent<RoleControl>();
		SFXManager.ListenerTarget = Player.gameObject.transform;
		GameUI = Instantiate(prefab_gameui).GetComponent<UIManager>();
		GameUI.transform.SetParent(ui_trans);
		VCamera.Follow = Player.gameObject.transform;
		VCamera.LookAt = Player.gameObject.transform;


		// 注册应用退出事件
		Application.quitting += OnApplicationQuit;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		GameUI.OpenWindow("winMain");
		UIRoleHud playerHud = Player.GetComponentInChildren<UIRoleHud>();
		StartCoroutine(StartPaly(playerHud));
		StoryMode = true;
	}

	IEnumerator StartPaly(UIRoleHud hud)
	{
		hud.Bubble("麋鹿麋鹿迷了路,");
		yield return new WaitForSeconds(2.0f);
		hud.Bubble("地上一堆小杂物,");
		yield return new WaitForSeconds(2.0f);
		hud.Bubble("捡起杂物做任务！");
		yield return new WaitForSeconds(2.0f);
		StoryMode = false;
	}

	void OnApplicationQuit()
	{
		Helpers.Log(this, "ApplicationQuit");
		TerrainTool.ResetTerrainData();
	}

	private void Update()
	{
		if (m_buildMode)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_dragMousePos = Input.mousePosition;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				// 如果射线与物体碰撞
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "Player", "Enemy", "Neutral", "Interactable" })))
				{
					CurrentSlected = hit.collider.gameObject;
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				m_dragMousePos = Vector3.zero;
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 direction = Input.mousePosition - m_dragMousePos;
				if (direction.magnitude > 0)
				{
					Vector3 transPos = buildModeFollow.transform.position - new Vector3(direction.x, 0, direction.y) * 1.0f * Time.deltaTime;
					if (!InBaseBoundary(transPos)) transPos = buildModeFollow.transform.position - new Vector3(direction.x, 0, direction.y) * 0.1f * Time.deltaTime;
					buildModeFollow.transform.position = transPos;
					m_dragMousePos = Input.mousePosition;
					//Helpers.Log(this, "MouseDrag", $"from{m_dragMousePos}To{Input.mousePosition}->{direction.magnitude}");
				}
			}
			else if (!InBaseBoundary(buildModeFollow.transform.position))
			{
				buildModeFollow.transform.position = Vector3.Lerp(buildModeFollow.transform.position, ConstraintPos(buildModeFollow.transform.position), 3.0f);
				if (InBaseBoundary(buildModeFollow.transform.position)) m_revisCampos = Vector3.zero;
			}

		}

		if (Input.GetKeyDown("space"))
		{
			BuildMode = !m_buildMode;
		}

		// if (Input.GetKeyDown(KeyCode.Return))
		// {
		// 	LocalNavMeshBuilder builder = GetComponent<LocalNavMeshBuilder>();

		// 	GameObject obj = Instantiate(Resources.Load("building_hotel"), builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	Debug.Log("build at :" + obj.transform.position);
		// }
		if (Input.GetKeyDown(KeyCode.Return))
		{
			//EffectAction("Test", new string[] { "EffectAcion Test!" });
		}

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if (!Mathf.Approximately(mouseWheel, 0.0f))
		{
			Vector3 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
				CameraCtrl.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
		}
	}

	public IEnumerator WaitAction(float waitTime, Action action)
	{
		// 等待一定的时间
		yield return new WaitForSeconds(waitTime);
		action?.Invoke();
	}

	bool InBaseBoundary(Vector3 pos)
	{
		return pos.x > baseBoundary_LB.x && pos.x < baseBoundary_RT.x
				&& pos.z > baseBoundary_LB.y && pos.z < baseBoundary_RT.y;
	}

	Vector3 ConstraintPos(Vector3 sourcePos)
	{
		Vector3 pos = sourcePos;
		if (pos.x < baseBoundary_LB.x) pos.x = baseBoundary_LB.x;
		if (pos.x > baseBoundary_RT.x) pos.x = baseBoundary_RT.x;
		if (pos.z < baseBoundary_LB.y) pos.z = baseBoundary_LB.y;
		if (pos.z > baseBoundary_RT.y) pos.z = baseBoundary_RT.y;
		return pos;
	}
	public static string SceneBoxInfo(GameObject sceneBox, bool display)
	{
		if (!sceneBox) return display ? "空地" : "blank";
		string sceneTag = sceneBox.tag;
		switch (sceneTag)
		{
			case "road":
				return display ? "道路" : sceneTag;
			case "building":
				return display ? "建筑:" + sceneBox : sceneTag;
			case "pit":
				return display ? "坑:" + sceneBox : sceneTag;
			case "mount":
				return display ? "山体:" : sceneTag;
			case "npc":
				return display ? "npc:" + sceneBox : sceneTag;
			case "creater":
				return display ? "建造中..." : sceneTag;
			default:
				break;
		}
		return display ? "空地" : "blank";
	}
}
