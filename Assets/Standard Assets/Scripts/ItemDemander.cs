using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using DG.Tweening;

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
		if (m_character == null) m_character = actor.GetComponent<CharacterData>();
		if (eventName == "Enter" && m_character != null)
		{
			ui_demand.Inter();
			m_character.Inventory.ItemEvent += OnItemEvent;
		}
		if (eventName == "Ready" && m_character != null)
		{
			if (m_character.BaseAI.isIdle && !m_Demand.Completed)
			{
				var submitable = m_Demand.Submittable(m_character.Inventory);
				if (submitable.Count > 0)
				{
					m_Demand.Fulfill(m_character.Inventory);
				}
				else ui_demand.Fail();
			}
		}
		if (eventName == "Exit" && m_character != null)
		{
			m_character.Inventory.ItemEvent -= OnItemEvent;
			m_character = null;
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
			ResCollector collector = m_character.GetComponent<ResCollector>();
			if (collector)
			{
				//collector.Display(item, itemCount, UpdateDemand);
				Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
				StartCoroutine(UpdateDemand(item, itemCount));
			}
		}
	}

	IEnumerator UpdateDemand(Item item, int itemCount)
	{
		var resObjs = new List<GameObject>();
		for (int i = 0; i < itemCount; i++)
		{
			GameObject resObj = Instantiate(item.WorldObjectPrefab, transform);
			resObjs.Add(resObj);
			resObj.transform.DOMove(m_character.transform.position + new Vector3(0, 2, 0), 1).From();
		}
		yield return new WaitForSeconds(1.0f);
		ui_demand.Show(m_Demand);
		if (m_Demand.Completed) isStarted = true;
		foreach (var obj in resObjs)
		{
			Destroy(obj);
		}
	}
}
