using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class ItemDemander : TimerBehaviour
{
	[SerializeField]
	InteractOnTrigger detector;
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

		//detector?.OnEnter.AddListener(OnInterEnter);
		//detector?.OnExit.AddListener(OnInterExit);
		//detector?.OnStay.AddListener(OnInterStay);
	}

	public void OnInteractEvent(RoleControl actor, string eventName)
	{
		if (eventName == "Completed")
		{
			if (actor.isIdle)
			{
				if (m_Demand.Completed) { isStarted = true; actor.Data.Inventory.Actions -= OnInventoryAction; }
				else { actor.Data.Inventory.Actions += OnInventoryAction; m_Demand.Fulfill(actor.Data.Inventory); }
			}
		}
	}

	// void OnInterEnter(GameObject enter)
	// {
	// 	Helpers.Log(this, "OninterEnter", "enter= " + enter);
	// }

	// void OnInterExit(GameObject exiter)
	// {
	// 	Helpers.Log(this, "OninterEnter", "exiter= " + exiter);
	// }

	// void OnInterStay(GameObject stayer, float during)
	// {
	// 	Helpers.Log(this, "OninterEnter", "stayer= " + stayer);
	// }


	// RoleControl role = inter.GetComponent<RoleControl>();
	// if (role && role.isIdle)
	// {
	// 	if (m_Demand.Completed) { isStarted = true; role.Data.Inventory.Actions -= OnInventoryAction; }
	// 	else { role.Data.Inventory.Actions += OnInventoryAction; m_Demand.Fulfill(role.Data.Inventory); }
	// }


	// protected override void OnInterval()
	// {
	// 	if (m_role && m_role.isIdle)
	// 	{
	// 		if (m_Demand.Completed) isStarted = true;
	// 		else m_Demand.Fulfill(m_role.Data.Inventory);
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
		Helpers.Log(this, "OnTimer");
		// GameObject createObj = Resources.Load(creatPrefab) as GameObject;
		// if (createObj)
		// {
		// 	GameObject obj = Instantiate(createObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
		// 	//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		// }
	}

	void OnInventoryAction(string itemName, string actionName, int itemCount)
	{
		if (actionName == "Fulfill")
		{
			ui_demand.Show(m_Demand);
			if (m_Demand.Completed) isStarted = true;
			Helpers.Log(this, "Fulfill", $"{itemName}x{itemCount}");
		}
	}
}
