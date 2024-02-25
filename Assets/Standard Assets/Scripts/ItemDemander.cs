using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class ItemDemander : TimerBehaviour
{
	public string demanderId;
	[SerializeField]
	InteractOnTrigger detector;
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	List<KeyValueData.KeyValue<Item, int>> DemandData;
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> EndEffect;
	InventorySystem.ItemDemand m_Demand;
	CharacterData m_character;
	void Start()
	{
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(DemandData));
		ui_demand.Show(m_Demand);

		//detector?.OnEnter.AddListener(OnInterEnter);
		//detector?.OnExit.AddListener(OnInterExit);
		//detector?.OnStay.AddListener(OnInterStay);
	}

	public void OnInteractEvent(GameObject actor, string eventName)
	{
		m_character = actor.GetComponent<CharacterData>();
		if (eventName == "Ready" && m_character != null)
		{
			if (m_character.BaseAI.isIdle && !m_Demand.Completed)
			{
				m_character.Inventory.ItemEvent += OnItemEvent;
				m_Demand.Fulfill(m_character.Inventory);
			}
		}
	}

	protected override void OnStart()
	{
		ui_demand.gameObject.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnEnd()
	{
		foreach (var effect in EndEffect)
		{
			effect.Key.TakeEffect(gameObject, gameObject, effect.Value);
		}
		m_character.GetComponent<EventSender>()?.Count(EventSender.EventType.DemandComplete, demanderId);
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

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		if (actionName == "Fulfill")
		{
			ui_demand.Show(m_Demand);
			Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
			if (m_Demand.Completed) OnDemandeComplete();
		}
	}

	void OnDemandeComplete()
	{
		isStarted = true;
		m_character.Inventory.ItemEvent -= OnItemEvent;
	}
}
