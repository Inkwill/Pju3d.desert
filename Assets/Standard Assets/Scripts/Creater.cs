using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;

public class Creater : TimerBehaviour
{
	public GameObject fxProgress;
	protected float m_initialDeep;
	[SerializeField]
	protected string creatPrefab;
	protected CharacterControl m_character;

	public bool Auto = false;

	public void DoCreate(CharacterControl character, string pbName)
	{
		creatPrefab = pbName;
		m_character = character;
		isStarted = true;
	}

	void Start()
	{
		if (fxProgress) fxProgress.SetActive(false);
		m_initialDeep = Terrain.activeTerrain.SampleHeight(transform.position);
		isStarted = Auto;

	}
	protected override void OnStart()
	{
		if (fxProgress) fxProgress.SetActive(true);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnEnd()
	{
		if (fxProgress) fxProgress.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, false);
		Destroy(gameObject, interval);
	}
	protected override void OnTimer()
	{
		CreateObj(transform);
		//GetComponent<EventSender>()?.Send(gameObject, "event_work_completed");
	}

	void CreateObj(Transform trans)
	{
		GameObject createObj = Resources.Load(creatPrefab) as GameObject;
		if (createObj)
		{
			GameObject obj = Instantiate(createObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		}
	}
}
