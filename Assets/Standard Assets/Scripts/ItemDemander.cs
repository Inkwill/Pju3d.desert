using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using DG.Tweening;

public class ItemDemander : TimerBehaviour, IInteractable
{
	public string demanderId;
	public UnityEvent RefreshEvents;
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	List<KeyValueData.KeyValue<Item, int>> DemandData;
	InventorySystem.ItemDemand m_Demand;
	bool m_interactable;
	void Start()
	{
		Init();
	}

	public void Init()
	{
		m_interactable = true;
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(DemandData));
		ui_demand.Show(m_Demand);
	}

	public void OnInteractorEnter(CharacterData enter)
	{
		ui_demand.Inter();
		m_interactable = true;
	}

	public string InteractAnim(CharacterData character)
	{
		return "";
	}

	public bool CanInteract(CharacterData character)
	{
		return character.BaseAI.isIdle && !m_Demand.Completed && m_interactable;
	}

	public void InteractWith(CharacterData character)
	{
		var submitable = m_Demand.Submittable(character.Inventory);
		if (submitable.Count > 0)
		{
			m_target = character.gameObject;
			character.Inventory.ItemAction += OnItemEvent;
			m_Demand.Fulfill(character.Inventory);
		}
		else { ui_demand.Fail(); m_interactable = false; }
	}

	protected override void OnRefresh()
	{
		Init();
		RefreshEvents?.Invoke();
	}
	protected override void OnStart()
	{
		ui_demand.gameObject.SetActive(false);
		m_interactable = false;
		//if (m_character) m_character.ChangeState(CharacterControl.State.WORKING, true);
	}

	protected override void OnBehaved()
	{
		m_target?.GetComponent<EventSender>()?.Count(EventSender.EventType.DemandComplete, demanderId);
		m_target = null;
	}

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		if (actionName == "Fulfill")
		{
			if (m_target != null)
			{
				m_target.GetComponent<CharacterData>().Inventory.ItemAction -= OnItemEvent;
				ResCollector collector = m_target.GetComponent<ResCollector>();
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
			resObj.transform.DOMove(m_target.transform.position + new Vector3(0, 2, 0), 1).From();
		}
		yield return new WaitForSeconds(1.0f);
		foreach (var obj in resObjs)
		{
			Destroy(obj);
		}
		ui_demand.Show(m_Demand);
		if (m_Demand.Completed) StartBehaviour();
		else m_target = null;
	}
}
