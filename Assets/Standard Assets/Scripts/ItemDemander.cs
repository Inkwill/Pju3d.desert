using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using DG.Tweening;
using System.Linq;

public class ItemDemander : TimerBehaviour, IInteractable
{
	public string demanderId;
	public UnityEvent RefreshEvents;
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	List<KeyValueData.KeyValue<Item, int>> DemandData;
	InventorySystem.ItemDemand m_Demand;
	[SerializeField]
	bool autoActive;
	public IInteractable CurrentInteractor { get { return m_interactor; } set { m_interactor = value; } }
	protected IInteractable m_interactor;
	void Start()
	{
		ui_demand.gameObject.SetActive(false);
		if (autoActive) ActiveInteract();
	}

	public void ActiveInteract()
	{
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(DemandData));
		ui_demand.Show(m_Demand);
		GetComponent<InteractHandle>()?.SetHandle(true);
	}

	public string InteractAnim(IInteractable target)
	{
		return "Skill";
	}

	public bool CanInteract(IInteractable target)
	{
		return !m_Demand.Completed && !isStarted && target.CanInteract(this) && m_interactor == null;
	}

	public void InteractWith(IInteractable target)
	{
		if (m_interactor == null && target is CharacterData)
		{
			var character = target as CharacterData;
			var submitable = m_Demand.Submittable(character.Inventory);
			if (submitable.Values.Sum() > 0)
			{
				m_interactor = character;
				character.Inventory.ItemAction += OnItemEvent;
				m_Demand.Fulfill(character.Inventory);
			}
			else ui_demand.Fail();
		}
	}

	protected override void OnRefresh()
	{
		ActiveInteract();
		RefreshEvents?.Invoke();
	}
	protected override void OnStart()
	{
		ui_demand.gameObject.SetActive(false);
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
			CharacterData character = m_interactor as CharacterData;
			m_interactor = null;
			if (character != null)
			{
				character.Inventory.ItemAction -= OnItemEvent;
				ResCollector collector = character.GetComponent<ResCollector>();
				if (collector)
				{
					m_target = character.gameObject;
					Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
					StartCoroutine(UpdateDemand(item, itemCount));
				}
				else if (m_Demand.Completed) StartTimer();
			}
			else ui_demand.Show(m_Demand);
		}
	}

	IEnumerator UpdateDemand(Item item, int itemCount)
	{
		if (m_target != null)
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
			if (m_Demand.Completed) StartTimer();
		}
	}
}
