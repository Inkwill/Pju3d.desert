using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	public static CharacterControl Player;
	public static UIManager GameUI;
	public static KeyValueData Data => Instance.DemoData;
	public LightManager DayNight;
	public CinemachineVirtualCamera VCamera;
	public CameraController CameraCtrl;
	public SFXManager SFXManager;

	public TerrainTool TerrainTool;

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
	private void Awake()
	{
		Instance = this;
		Player = Instantiate(prefab_character, position_born, Quaternion.Euler(0, 180, 0)).GetComponent<CharacterControl>();
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
		var dic = new Dictionary<string, int>
		{
			{ "Wood", 1 },
			{ "Money", 1 },
		};
		testDemand = new InventorySystem.ItemDemand(dic);
	}

	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit!,terrian= " + Terrain.activeTerrain);
		TerrainTool.ResetTerrainData();
	}

	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			Player.CurState = RoleControl.State.ATTACKING;
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
			UIRoleHud hud = Player.GetComponentInChildren<UIRoleHud>();
			if (hud != null) hud.Bubble("你好我好大家好!");
			Player.CurState = RoleControl.State.DEAD;
		}

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if (!Mathf.Approximately(mouseWheel, 0.0f))
		{
			Vector3 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
				CameraCtrl.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
		}
	}

	public static string SceneBoxInfo(bool display)
	{
		GameObject sceneBox = Player.BaseAI.SceneDetector.lastInner;
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
