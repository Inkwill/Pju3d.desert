using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using DG.Tweening;
using System.Linq;

public class ItemDemander : MonoBehaviour, IInteractable
{
	public IInteractable CurrentInteractor { get { return m_submitter; } set { if (value == null && m_submitter != null) m_submitter = null; } }
	public InteractData Data { get { return m_data; } }
	[SerializeField]
	InteractData m_data;
	[SerializeField]
	bool autoActive;
	public Action<InventorySystem.ItemDemand, string> demandEvent;

	List<InventorySystem.ItemDemand> m_demands;
	int m_curDemandIndex;
	Timer m_timer;
	Character m_submitter;

	void Awake()
	{
		if (m_data.BehaveDuring > 0 || m_data.maxActTimes > 1)
			m_timer = gameObject.AddComponent<Timer>();

	}
	void Start()
	{
		if (autoActive)
		{
			GetComponent<InteractHandle>()?.SetHandle(true);
			ActiveInteract();
		}
		if (m_timer)
		{
			m_timer.timerDuration = m_data.BehaveDuring;
			m_timer.loopTimes = m_data.maxActTimes;
			m_timer.cd = m_data.actCd;
			m_timer.refreshEvent += ActiveInteract;
			m_timer.behaveEvent += () => m_data.InteractBehave(transform, m_curDemandIndex);
			if (m_data.autoDestroy) m_timer.endEvent += () => Destroy(gameObject);
		}
	}

	void ActiveInteract()
	{
		m_demands = new List<InventorySystem.ItemDemand>();
		foreach (var data in m_data.demands.Value)
		{
			m_demands.Add(data.CreatItemDemand());
		}
		if (m_data.Type == InteractData.InteractType.DeviceFixer && m_demands.Count > 0)
		{
			m_curDemandIndex = 0;
			demandEvent?.Invoke(m_demands[m_curDemandIndex], "Active");
		}
	}

	public bool CanInteract(IInteractable target)
	{
		return !m_demands[m_curDemandIndex].Completed && target.CanInteract(this) && m_submitter == null;
	}

	public void InteractWith(IInteractable target)
	{
		if (target is Character && m_submitter == null)
		{
			target.CurrentInteractor = null;
			var character = target as Character;
			var submitable = m_demands[m_curDemandIndex].Submittable(character.Inventory);
			if (submitable.Values.Sum() > 0)
			{
				m_submitter = character;
				if (character == GameManager.CurHero)
				{
					UIInteractWindow win = GameManager.GameUI.GetWindow("winInteract") as UIInteractWindow;
					win.Init(this, m_demands);
					win.Open();
					GetComponent<InteractHandle>()?.ExitEvent.AddListener(() => win.Close());
					win.bt_Confirm.onClick.AddListener(() => OnClick_Interact(win));
				}
				else
				{
					character.Inventory.ItemAction += OnItemEvent;
					m_demands[m_curDemandIndex].Fulfill(character.Inventory);
				}
				demandEvent += OnDemandEvent;
			}
			else demandEvent?.Invoke(m_demands[m_curDemandIndex], "Fail");
		}
	}

	void OnDemandEvent(InventorySystem.ItemDemand demand, string eventName)
	{
		if (eventName == "Fulfill" && demand.Completed)
		{
			demandEvent?.Invoke(demand, "Complete");
			if (m_timer) m_timer.StartTimer();
			else m_data.InteractBehave(transform, m_curDemandIndex);
			m_submitter?.GetComponent<EventSender>()?.Count(EventSender.EventType.DemandComplete, m_data.Key);
		}
		m_submitter = null;
		demandEvent -= OnDemandEvent;
	}

	public void OnClick_Interact(UIInteractWindow win)
	{
		GameManager.CurHero.Inventory.ItemAction += OnItemEvent;
		m_demands[m_curDemandIndex].Fulfill(GameManager.CurHero.Inventory);
		m_curDemandIndex = win.SelectedIndex;
		win.Close();
	}

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		if (actionName == "Fulfill" && m_submitter != null)
		{
			m_submitter.Inventory.ItemAction -= OnItemEvent;
			ResCollector collector = m_submitter.GetComponent<ResCollector>();
			if (collector)
			{
				Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
				StartCoroutine(UpdateDemand(item, itemCount));

			}
			else demandEvent?.Invoke(m_demands[m_curDemandIndex], "Fulfill");
		}
	}

	IEnumerator UpdateDemand(Item item, int itemCount)
	{
		var resObjs = new List<GameObject>();
		for (int i = 0; i < itemCount; i++)
		{
			GameObject resObj = Instantiate(item.WorldObjectPrefab, transform);
			resObjs.Add(resObj);
			resObj.transform.DOMove(m_submitter.transform.position + new Vector3(0, 2, 0), 1).From();
		}
		yield return new WaitForSeconds(1.0f);
		foreach (var obj in resObjs)
		{
			Destroy(obj);
		}
		demandEvent?.Invoke(m_demands[m_curDemandIndex], "Fulfill");
	}
}
