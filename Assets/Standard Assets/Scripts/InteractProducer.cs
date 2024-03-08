using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class InteractProducer : MonoBehaviour, IInteractable
{
	[SerializeField]
	int maxCount = 10;
	[SerializeField]
	int initCount = 10;
	[SerializeField]
	int m_collectCd = 1;
	[SerializeField]
	string CollectAnim;
	[SerializeField]
	float m_replenishTime = 10;
	public UnityEvent<GameObject, GameObject> InteractEvent;
	public UnityEvent<GameObject> ExhaustedEvent;
	public UIItemGrid itemGrid;

	float m_cd;
	int m_count;

	void Start()
	{
		m_count = initCount;
		itemGrid?.Init(maxCount);
		itemGrid?.ShowItem(m_count);
		if (m_count < maxCount && m_replenishTime != -1) StartCoroutine(Replenish());
	}
	public bool CanInteract(CharacterData character)
	{
		return character.BaseAI.isIdle && m_cd <= 0 && (m_count > 0 || maxCount == -1);
	}

	public void OnInteractorEnter(CharacterData character)
	{

	}

	public string InteractAnim(CharacterData character)
	{
		return CollectAnim;
	}

	public void InteractWith(CharacterData character)
	{
		InteractEvent?.Invoke(gameObject, character.gameObject);
		Helpers.Log(this, "InteractWith", character.CharacterName);
		m_cd = m_collectCd;
		if (maxCount != -1 && --m_count < 1 && m_replenishTime == -1) OnExhausted(character);
		if (m_replenishTime != -1) StartCoroutine(Replenish());
		itemGrid?.ShowItem(m_count);
	}
	IEnumerator Replenish()
	{
		yield return new WaitForSeconds(m_replenishTime);
		m_count++;
		itemGrid?.ShowItem(m_count);
		if (m_count < maxCount) StartCoroutine(Replenish());
	}

	void OnExhausted(CharacterData character)
	{
		GetComponent<InteractHandle>()?.SetHandle(false);
		ExhaustedEvent?.Invoke(gameObject);
	}

	void Update()
	{
		if (m_cd > 0) m_cd -= Time.deltaTime;
	}
}
