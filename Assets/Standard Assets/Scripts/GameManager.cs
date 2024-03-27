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
	public static LordData Lord => Instance.GetComponent<LordData>();
	public static Character CurHero;
	public static StoryListener StoryListener => Instance.GetComponent<StoryListener>();
	public static GameGoalSystem GameGoal => Instance.GetComponent<GameGoalSystem>();
	public static LevelSystem GameLevel => Instance.m_levelSystem;
	public static UIManager GameUI;
	public static KeyValueData Config => Instance.DemoData;
	public LightManager DayNight;
	public CinemachineVirtualCamera VCamera;
	public CameraController CameraCtrl;
	public SFXManager SFXManager;
	public TerrainTool TerrainTool;
	LevelSystem m_levelSystem;
	public List<FormulaData> formulas;
	public List<Spawner> spawners;
	public LevelData startLevel;

	public static void SetStoryModel(bool active, Action action = null)
	{
		if (storyMode == active) { action?.Invoke(); return; }
		storyMode = active;
		GameUI.JoyStick.enabled = !active;
		if (active)
		{
			Instance.CameraCtrl.SetMode(CameraController.Mode.STORY, action);
			CurHero.BaseAI.SetState(AIBase.State.INACTIVE);
		}
		else
		{
			Instance.CameraCtrl.SetMode(CameraController.Mode.RPG, action);
			CurHero.BaseAI.SetState(AIBase.State.IDLE);
		}
	}
	public static bool storyMode { get; private set; }

	[SerializeField]
	Transform ui_trans;
	[SerializeField]
	Transform position_born;
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
	private void Awake()
	{
		Instance = this;
		Lord.Init();
		CurHero = Instantiate(prefab_character, position_born.position, Quaternion.Euler(0, 180, 0)).GetComponent<Character>();
		GameGoal.Init();
		SFXManager.ListenerTarget = CurHero.gameObject.transform;
		GameUI = Instantiate(prefab_gameui).GetComponent<UIManager>();
		GameUI.transform.SetParent(ui_trans);
		VCamera.Follow = CurHero.gameObject.transform;
		VCamera.LookAt = CurHero.gameObject.transform;

		m_levelSystem = new LevelSystem();


		// 注册应用退出事件
		Application.quitting += OnApplicationQuit;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}

	void Start()
	{
		ResInventoryItem moneyInventory = KeyValueData.GetValue<Item>(GameManager.Config.Item, "ResInventory_Money") as ResInventoryItem;
		if (moneyInventory) GameManager.CurHero.Inventory.ResInventories.Add(ResItem.ResType.Money, new InventorySystem.ResInventory(moneyInventory));
		GameUI.OpenWindow("winRpg");
		GameUI.OpenWindow("winLevelSelect");
	}

	void OnApplicationQuit()
	{
		Helpers.Log(this, "ApplicationQuit");
		TerrainTool.ResetTerrainData();
	}

	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			if (CameraCtrl.CurMode != CameraController.Mode.BUILD) CameraCtrl.SetMode(CameraController.Mode.BUILD);
			else CameraCtrl.SetMode(CameraController.Mode.RPG);
			//UIHudCanvas.Instance.button.transform.position = Camera.main.WorldToScreenPoint(CurHero.transform.position);
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
			CurHero.BaseAI.Back(5.0f);
		}

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if (!Mathf.Approximately(mouseWheel, 0.0f))
		{
			Vector3 view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
				CameraCtrl.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
		}
	}

	public static void StartWaitAction(float waitTime, Action action)
	{
		Instance.StartCoroutine(Instance.WaitAction(waitTime, action));
	}
	IEnumerator WaitAction(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action?.Invoke();
	}

	public static void StartNextFrameAction(Action action)
	{
		Instance.StartCoroutine(Instance.NextFrameAction(action));
	}
	IEnumerator NextFrameAction(Action action)
	{
		yield return new WaitForNextFrameUnit();
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
