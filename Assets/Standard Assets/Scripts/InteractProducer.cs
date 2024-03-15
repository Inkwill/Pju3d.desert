using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class InteractProducer : MonoBehaviour, IInteractable
{
	[SerializeField]
	InteractData m_data;
	public InteractData Data { get { return m_data; } }
	[SerializeField]
	ResItem resItem;
	[SerializeField]
	int maxCount = 10;
	[SerializeField]
	int initCount = 10;
	[SerializeField]
	float m_replenishTime = 10;
	public UnityEvent<GameObject, GameObject> InteractEvent;
	public UnityEvent<GameObject> ExhaustedEvent;
	public UIItemGrid itemGrid;
	public IInteractable CurrentInteractor { get { return m_interactor; } set { m_interactor = value; } }
	protected IInteractable m_interactor;
	int m_count;

	void Start()
	{
		m_count = initCount;
		itemGrid?.Init(maxCount);
		itemGrid?.ShowItem(m_count);
		GetComponent<InteractHandle>()?.SetHandle(true);
		if (m_count < maxCount && m_replenishTime != -1) StartCoroutine(Replenish());
	}
	public bool CanInteract(IInteractable target)
	{
		if ((m_count > 0 || maxCount == -1) && target is Character && target.CanInteract(this))
		{
			var character = target as Character;
			if (!character.HoldTools(resItem))
			{
				character.GetComponentInChildren<UIRoleHud>().Bubble("I need a " + resItem.toolType);
				return false;
			}
			if (resItem.requireContainer)
			{
				if (!character.Inventory.ResInventories.ContainsKey(resItem.resType))
				{
					character.GetComponentInChildren<UIRoleHud>().Bubble("I need a container of " + resItem.ItemName);
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public void InteractWith(IInteractable target)
	{
		if (target is Character)
		{
			var character = target as Character;
			InteractEvent?.Invoke(gameObject, character.gameObject);
			Helpers.Log(this, "InteractWith", character.CharacterName);
			if (maxCount != -1 && --m_count < 1 && m_replenishTime == -1) OnExhausted();
			if (m_replenishTime != -1) StartCoroutine(Replenish());
			itemGrid?.ShowItem(m_count);
		}
	}
	IEnumerator Replenish()
	{
		yield return new WaitForSeconds(m_replenishTime);
		m_count++;
		itemGrid?.ShowItem(m_count);
		if (m_count < maxCount) StartCoroutine(Replenish());
	}

	void OnExhausted()
	{
		GetComponent<InteractHandle>()?.SetHandle(false);
		ExhaustedEvent?.Invoke(gameObject);
	}
}
