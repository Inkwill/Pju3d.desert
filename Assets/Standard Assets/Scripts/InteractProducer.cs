using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class InteractProducer : MonoBehaviour, IInteractable
{
	[SerializeField]
	InteractData m_data;
	[SerializeField]
	bool autoActive;
	public InteractData Data { get { return m_data; } }
	public UnityEvent<GameObject> ExhaustedEvent;
	public UIItemGrid itemGrid;
	public IInteractable CurrentInteractor { get { return m_interactor; } set { m_interactor = value; } }
	protected IInteractable m_interactor;
	Timer m_timer;
	void Awake()
	{
		m_timer = gameObject.AddComponent<Timer>();
	}
	void Start()
	{
		if (autoActive) ActiveInteract();
		if (m_timer)
		{
			m_timer.timerDuration = m_data.BehaveDuring;
			m_timer.MaxTimes = m_data.maxActTimes;
			m_timer.cd = m_data.actCd;
			m_timer.refreshAction += ActiveInteract;
			m_timer.behaveAction += () =>
			{
				GetComponent<InteractHandle>()?.SetHandle(false);
				m_data.InteractBehave(transform);
				itemGrid?.ShowItem(m_timer.LeftTimes);
			};

			m_timer.endAction += () =>
			{
				if (m_data.maxActTimes != -1)
					OnExhausted();
			};
		}

		itemGrid?.Init(m_data.maxActTimes);
		itemGrid?.ShowItem(m_timer.LeftTimes);
	}

	void ActiveInteract()
	{
		GetComponent<InteractHandle>()?.SetHandle(true);
	}


	public bool CanInteract(IInteractable target)
	{
		if ((m_timer.CurCd <= 0) && (m_timer.LeftTimes > 0 || m_data.maxActTimes == -1) && target is Character && target.CanInteract(this))
		{
			var character = target as Character;
			if (!character.HoldTools(m_data.resItem))
			{
				character.GetComponentInChildren<UIRoleHud>().Bubble("I need a " + m_data.resItem.toolType);
				return false;
			}
			if (m_data.resItem.requireContainer)
			{
				if (!character.Inventory.ResInventories.ContainsKey(m_data.resItem.resType))
				{
					character.GetComponentInChildren<UIRoleHud>().Bubble("I need a container of " + m_data.resItem.ItemName);
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public void InteractWith(IInteractable target)
	{
		m_timer.StartTimer();
	}

	void OnExhausted()
	{
		GetComponent<InteractHandle>()?.SetHandle(false);
		ExhaustedEvent?.Invoke(gameObject);
		if (m_data.autoDestroy) Destroy(gameObject);
	}
}
