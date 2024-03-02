using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using DG.Tweening;

[RequireComponent(typeof(InteractHandle))]
public class ItemDemander : TimerBehaviour
{
	public string demanderId;
	public UnityEvent<GameObject, GameObject> DemandeEvents;
	public UnityEvent RefreshEvents;
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	List<KeyValueData.KeyValue<Item, int>> DemandData;
	InventorySystem.ItemDemand m_Demand;
	CharacterData m_character;
	InteractHandle m_interactHandle;
	void Start()
	{
		m_interactHandle = GetComponent<InteractHandle>();
		m_interactHandle.InteractEvent.AddListener(OnInteractEvent);
		m_interactHandle.SetHandle(true);
		Init();
	}

	public void Init()
	{
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(DemandData));
		ui_demand.Show(m_Demand);
	}

	public void OnInteractEvent(GameObject actor, string eventName)
	{
		var character = actor.GetComponent<CharacterData>();
		if (eventName == "Enter" && character != null)
		{
			ui_demand.Inter();
		}
		if (eventName == "Ready" && m_character != null)
		{
			if (m_character.BaseAI.isIdle && !m_Demand.Completed)
			{
				var submitable = m_Demand.Submittable(m_character.Inventory);
				if (submitable.Count > 0)
				{
					m_character.Inventory.ItemEvent += OnItemEvent;
					m_Demand.Fulfill(m_character.Inventory);
				}
				else ui_demand.Fail();
			}
		}
	}

	protected override void OnRefresh()
	{
		Init();
		RefreshEvents?.Invoke();
	}
	protected override void OnStart()
	{
		ui_demand.gameObject.SetActive(false);
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnEnd()
	{
		m_interactHandle.SetHandle(false);
	}
	protected override void OnTimer()
	{
		if (m_character != null)
		{
			DemandeEvents?.Invoke(gameObject, m_character.gameObject);
			m_character.GetComponent<EventSender>()?.Count(EventSender.EventType.DemandComplete, demanderId);
		}


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
			if (m_character != null)
			{
				m_character.Inventory.ItemEvent -= OnItemEvent;
				ResCollector collector = m_character.GetComponent<ResCollector>();
				if (collector)
				{
					//collector.Display(item, itemCount, UpdateDemand);
					Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
					StartCoroutine(UpdateDemand(item, itemCount));
				}
			}
			else ui_demand.Show(m_Demand);
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
