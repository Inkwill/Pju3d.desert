using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using DG.Tweening;
using System.Linq;

public class ItemDemander : MonoBehaviour, IInteractable
{
	[SerializeField]
	InteractData m_data;
	public InteractData Data { get { return m_data; } }
	[SerializeField]
	UIItemDemand ui_demand;
	[SerializeField]
	bool autoActive;
	public IInteractable CurrentInteractor { get { return m_interactor; } set { m_interactor = value; } }
	protected IInteractable m_interactor;
	int m_curSelected;
	List<InventorySystem.ItemDemand> m_demands;
	Timer m_timer;
	GameObject m_target;

	void Awake()
	{
		if (m_data.BehaveDuring > 0 || m_data.actTimes > 1)
			m_timer = gameObject.AddComponent<Timer>();

	}
	void Start()
	{
		ui_demand.gameObject.SetActive(false);
		if (autoActive) ActiveInteract();
		if (m_timer)
		{
			m_timer.timerDuration = m_data.BehaveDuring;
			m_timer.loopTimes = m_data.actTimes;
			m_timer.cd = m_data.actCd;
			m_timer.refreshEvent += ActiveInteract;
			m_timer.behaveEvent += OnBehaved;
			if (m_data.autoDestroy) m_timer.endEvent += () => Destroy(gameObject);
		}
	}

	public void ActiveInteract()
	{
		m_demands = new List<InventorySystem.ItemDemand>();
		foreach (var data in m_data.demands.Value)
		{
			m_demands.Add(data.CreatItemDemand());
		}
		if (m_demands.Count > 0) ui_demand.Show(m_demands[0]);
		GetComponent<InteractHandle>()?.SetHandle(true);
	}

	public bool CanInteract(IInteractable target)
	{
		return !m_demands[m_curSelected].Completed && target.CanInteract(this) && m_interactor == null;
	}

	public void InteractWith(IInteractable target)
	{
		if (m_interactor == null && target is Character)
		{
			target.CurrentInteractor = null;
			var character = target as Character;
			var submitable = m_demands[m_curSelected].Submittable(character.Inventory);
			if (submitable.Values.Sum() > 0)
			{
				m_interactor = character;
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
					m_demands[m_curSelected].Fulfill(character.Inventory);
				}
			}
			else ui_demand.Fail();
		}
	}

	public void OnClick_Interact(UIInteractWindow win)
	{
		GameManager.CurHero.Inventory.ItemAction += OnItemEvent;
		m_demands[m_curSelected].Fulfill(GameManager.CurHero.Inventory);
		m_curSelected = win.SelectedIndex;
		win.Close();
	}

	void OnBehaved()
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
			ui_demand.Show(m_demands[m_curSelected]);
			Character character = m_interactor as Character;
			m_interactor = null;
			if (character != null)
			{
				character.Inventory.ItemAction -= OnItemEvent;
				if (m_demands[m_curSelected].Completed)
				{
					if (m_timer) m_timer.StartTimer();
					else OnBehaved();
					ui_demand.gameObject.SetActive(false);
				}
				ResCollector collector = character.GetComponent<ResCollector>();
				if (collector)
				{
					m_target = character.gameObject;
					Helpers.Log(this, "Fulfill", $"{item.ItemName}x{itemCount}");
					StartCoroutine(UpdateDemand(item, itemCount));
				}
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
