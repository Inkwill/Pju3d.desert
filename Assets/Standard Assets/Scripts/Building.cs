using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class Building : TimerBehaviour
{
	[SerializeField]
	string creatPrefab;
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	GameObject fxProgress;
	RoleControl m_builder;
	InventorySystem.ItemDemand m_Demand;
	void Start()
	{
		var dic = new Dictionary<string, int>
		{
			{ "Wood", 10 },
			{ "Money", 1 },
		};
		m_Demand = new InventorySystem.ItemDemand(dic);
		ui_demand.Show(m_Demand);
	}

	protected override void OnInterval()
	{
		if (m_builder && m_builder.isIdle)
		{
			if (m_Demand.Completed) isStarted = true;
			else m_Demand.Fulfill(m_builder.Data.Inventory);
		}

	}
	public void OnRoleEnter(GameObject enter)
	{
		if (isStarted) return;
		if (m_builder == null) m_builder = enter.GetComponent<RoleControl>();
		if (m_builder.Data.Inventory != null)
		{
			m_builder.Data.Inventory.Actions += OnInventoryAction;
			Debug.Log("OnRoleEnter: " + m_builder);
		}
	}

	public void OnRoleExit(GameObject enter)
	{
		if (isStarted) return;
		RoleControl role = enter.GetComponent<RoleControl>();
		if (role && role == m_builder)
		{
			m_builder.Data.Inventory.Actions -= OnInventoryAction;
			Debug.Log("OnRoleExit: " + m_builder);
			m_builder = null;
		}
	}

	protected override void OnStart()
	{
		if (fxProgress) fxProgress.SetActive(true);
		ui_demand.gameObject.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnEnd()
	{
		if (fxProgress) fxProgress.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, false);
		//Destroy(gameObject, interval);
	}
	protected override void OnTimer()
	{
		Debug.Log("Building Completed!");
		// GameObject createObj = Resources.Load(creatPrefab) as GameObject;
		// if (createObj)
		// {
		// 	GameObject obj = Instantiate(createObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// }
	}

	void OnInventoryAction(string itemName, string actionName)
	{
		if (actionName == "Fulfill")
		{
			ui_demand.Show(m_Demand);
			if (m_Demand.Completed) isStarted = true;
			Debug.Log("fulfill entry,item =" + itemName);
		}
	}
}
