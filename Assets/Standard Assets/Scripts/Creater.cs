using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;

public class Creater : TimerBehaviour
{
	public GameObject fxProgress;

	public GameObject showObj;


	protected float digDeep;
	protected TerrainTool terrainTool;
	protected float m_initialDeep;
	protected string m_targetName;

	protected CharacterControl m_character;

	public void DoCreate(CharacterControl character, string pbName)
	{
		m_targetName = pbName;
		m_character = character;
		switch (pbName)
		{
			case "pit":
				digDeep = 0.1f;
				break;
			default:
				break;
		}
		isStarted = true;

	}

	void Awake()
	{
		if (fxProgress) fxProgress.SetActive(false);
		if (showObj) showObj.SetActive(false);
		terrainTool = GetComponent<TerrainTool>();
		m_initialDeep = Terrain.activeTerrain.SampleHeight(transform.position);

	}
	protected override void OnStart()
	{
		if (fxProgress) fxProgress.SetActive(true);
		if (showObj) showObj.SetActive(true);
		if (m_character) m_character.PlayWork(true);
	}

	protected override void OnEnd()
	{
		if (fxProgress) fxProgress.SetActive(false);
		if (showObj) showObj.SetActive(false);
		if (m_character) m_character.PlayWork(false);
		Destroy(gameObject, interval);
	}
	protected override void OnTimer()
	{
		CreateObj(transform);
		//GetComponent<EventSender>()?.Send(gameObject, "event_work_completed");
	}

	void CreateObj(Transform trans)
	{
		GameObject createObj = Resources.Load(m_targetName) as GameObject;
		if (createObj)
		{
			GameObject obj = Instantiate(createObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		}
	}

	protected override void OnProcessing(float completed)
	{
		if (digDeep > 0) terrainTool.LowerTerrain(transform.position, digDeep * 0.01f, 8, 8);
	}
}
