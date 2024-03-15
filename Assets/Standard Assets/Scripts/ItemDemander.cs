using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using DG.Tweening;
using System.Linq;

public class ItemDemander : TimerBehaviour, IInteractable
{
	[SerializeField]
	InteractData m_data;
	public InteractData Data { get { return m_data; } }
	[SerializeField]
	UIItemDemand ui_demand;
	public UnityEvent ReadyEvents;
	public UnityEvent RefreshEvents;
	[SerializeField]
	bool autoActive;
	public IInteractable CurrentInteractor { get { return m_interactor; } set { m_interactor = value; } }
	protected IInteractable m_interactor;
	Item m_curSelected;
	InventorySystem.ItemDemand m_curDemand;
	void Start()
	{
		ui_demand.gameObject.SetActive(false);
		if (autoActive) ActiveInteract();
	}

	public void ActiveInteract()
	{
		if (m_data.demands.Count > 0) m_curDemand = m_data.demands[0].Instance();
		ui_demand.Show(m_curDemand);
		GetComponent<InteractHandle>()?.SetHandle(true);
	}

	public bool CanInteract(IInteractable target)
	{
		return !m_curDemand.Completed && !isStarted && target.CanInteract(this) && m_interactor == null;
	}

	public void InteractWith(IInteractable target)
	{
		if (m_interactor == null && target is Character)
		{
			var character = target as Character;
			var submitable = m_curDemand.Submittable(character.Inventory);
			if (submitable.Values.Sum() > 0)
			{
				ReadyEvents?.Invoke();
				target.CurrentInteractor = null;
				if (character == GameManager.CurHero)
				{
					UIInteractWindow win = GameManager.GameUI.GetWindow("winInteract") as UIInteractWindow;
					win.Init(this);
					win.Open();
					GetComponent<InteractHandle>()?.ExitEvent.AddListener(() => win.Close());
					win.bt_interact.onClick.AddListener(() => OnClick_Interact(win));
				}
				else
				{
					m_interactor = character;
					character.Inventory.ItemAction += OnItemEvent;
					m_curDemand.Fulfill(character.Inventory);
				}
			}
			else { ui_demand.Fail(); target.CurrentInteractor = null; }
		}
	}

	public void OnClick_Interact(UIInteractWindow win)
	{
		m_interactor = GameManager.CurHero;
		GameManager.CurHero.Inventory.ItemAction += OnItemEvent;
		m_curDemand.Fulfill(GameManager.CurHero.Inventory);
		m_curSelected = win.SelectedItem;
		win.Close();
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

	protected override void OnEnd()
	{
		Destroy(gameObject);
	}

	protected override void OnBehaved()
	{
		m_data.InteractBehave(transform, m_curSelected);
		Character character = m_interactor as Character;
		if (character)
		{
			character.GetComponent<EventSender>()?.Count(EventSender.EventType.DemandComplete, m_data.Key);
		}
		m_interactor = null;
	}

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		if (actionName == "Fulfill")
		{
			ui_demand.Show(m_curDemand);
			Character character = m_interactor as Character;
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
				if (m_curDemand.Completed) StartTimer();
			}
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
		}
	}
}
