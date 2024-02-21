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

	[SerializeField]
	Transform ui_trans;
	[SerializeField]
	Vector3 position_born;
	[SerializeField]
	GameObject prefab_character;
	[SerializeField]
	GameObject prefab_gameui;

	[SerializeField]
	KeyValueData DemoData;
	InventorySystem.ItemDemand testDemand;
	Vector3 m_dragMousePos = Vector3.zero;
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
	}

	IEnumerator StartPaly(UIRoleHud hud)
	{
		hud.Bubble("麋鹿麋鹿迷了路,");
		// yield return new WaitForSeconds(2.0f);
		// hud.Bubble("前方有个小怪物,");
		// yield return new WaitForSeconds(2.0f);
		// hud.Bubble("待我上前问问路!");
		yield return new WaitForSeconds(1.0f);
		CameraCtrl.SetMode(CameraController.Mode.RPG);
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
					buildModeFollow.transform.position -= new Vector3(direction.x, 0, direction.y) * 1.0f * Time.deltaTime;
					m_dragMousePos = Input.mousePosition;
					//Helpers.Log(this, "MouseDrag", $"from{m_dragMousePos}To{Input.mousePosition}->{direction.magnitude}");
				}
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
