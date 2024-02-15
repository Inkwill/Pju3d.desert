using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class ItemDemander : TimerBehaviour
{
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	List<KeyValueData.KeyValue<Item, int>> DemandData;
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> EndEffect;
	InventorySystem.ItemDemand m_Demand;
	void Start()
	{
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(DemandData));
		ui_demand.Show(m_Demand);

		GetComponent<InteractHandle>()?.OnTargetStay.AddListener(OnInterStay);
	}

	void OnInterStay(GameObject inter)
	{
		RoleControl role = inter.GetComponent<RoleControl>();
		if (role && role.isIdle)
		{
			if (m_Demand.Completed) { isStarted = true; role.Data.Inventory.Actions -= OnInventoryAction; }
			else { role.Data.Inventory.Actions += OnInventoryAction; m_Demand.Fulfill(role.Data.Inventory); }
		}
	}

	// protected override void OnInterval()
	// {
	// 	if (m_role && m_role.isIdle)
	// 	{
	// 		if (m_Demand.Completed) isStarted = true;
	// 		else m_Demand.Fulfill(m_role.Data.Inventory);
	// 	}

	// }
	// public void OnRoleEnter(GameObject enter)
	// {
	// 	if (isStarted) return;
	// 	if (m_role == null) m_role = enter.GetComponent<RoleControl>();
	// 	if (m_role.Data.Inventory != null)
	// 	{
	// 		m_role.Data.Inventory.Actions += OnInventoryAction;
	// 		Debug.Log("OnRoleEnter: " + m_role);
	// 	}
	// }



	// public void OnRoleExit(GameObject enter)
	// {
	// 	if (isStarted) return;
	// 	RoleControl role = enter.GetComponent<RoleControl>();
	// 	if (role && role == m_role)
	// 	{
	// 		m_role.Data.Inventory.Actions -= OnInventoryAction;
	// 		Debug.Log("OnRoleExit: " + m_role);
	// 		m_role = null;
	// 	}
	// }

	protected override void OnStart()
	{
		ui_demand.gameObject.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnEnd()
	{
		foreach (var effect in EndEffect)
		{
			effect.Key.Take(gameObject, effect.Value);
		}
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
			//if (m_Demand.Completed) isStarted = true;
			Debug.Log("fulfill entry,item =" + itemName);
		}
	}
}
