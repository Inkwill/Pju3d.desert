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
	public CinemachineVirtualCamera Camera;
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

	private void Awake()
	{
		Instance = this;
		Player = Instantiate(prefab_character, position_born, Quaternion.Euler(0, 180, 0)).GetComponent<CharacterControl>();
		Player.eventSender.events.AddListener((player, eventName) =>
		{
			if (eventName == "playerEvent_OnInit") Debug.Log("player Init Ok!");
		});
		SFXManager.ListenerTarget = Player.gameObject.transform;
		GameUI = Instantiate(prefab_gameui).GetComponent<UIManager>();
		GameUI.transform.parent = ui_trans;
		Camera.Follow = Player.gameObject.transform;
		Camera.LookAt = Player.gameObject.transform;

		// 注册应用退出事件
		Application.quitting += OnApplicationQuit;

	}

	void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit!,terrian= " + Terrain.activeTerrain);
		TerrainTool.ResetTerrainData();
	}

	private void Update()
	{
		// if (Input.GetKeyDown("space"))
		// {
		// 	GetTerrainTextureInfo(gameObject.transform.position, 10);

		// }

		// if (Input.GetKeyDown(KeyCode.Return))
		// {
		// 	LocalNavMeshBuilder builder = GetComponent<LocalNavMeshBuilder>();

		// 	GameObject obj = Instantiate(Resources.Load("building_hotel"), builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	Debug.Log("build at :" + obj.transform.position);
		// }
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Debug.Log(DemoData.AudioDic[0].Key);
		}
		// if (Input.GetKeyDown("space"))
		// {
		// 	obj.Dehighlight();
		// }
	}

}
