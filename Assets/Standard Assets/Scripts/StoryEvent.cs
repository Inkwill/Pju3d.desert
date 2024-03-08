using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvent : MonoBehaviour
{
	public enum EventType
	{
		Bubble,
		PlayerDialogue
	}

	public EventType eventType = EventType.Bubble;

	[SerializeField]
	int triggerTimes = 1;
	[SerializeField]
	int loopTimes = 1;
	[SerializeField]
	int stayTimes = 1;
	[SerializeField]
	int cdTimes = 30;
	[SerializeField]
	List<KeyValueData.KeyValue<ConditionData, string[]>> Conditions;
	[SerializeField]
	List<KeyValueData.KeyValue<string, float>> Dialogues;
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> TriggerEffects;
	bool m_started;
	bool m_completed;
	int m_triggerTimes;
	float m_cd;

	void Update()
	{
		if (m_cd > 0) m_cd -= Time.deltaTime;
	}
	public void OnDamageEvent(Damage damage)
	{
		StartEvent(damage.Source);
	}
	public void OnEnter(GameObject enter)
	{
		StartEvent(enter.GetComponent<CharacterData>());
	}

	public void OnStay(GameObject enter, float during)
	{
		if (during >= stayTimes)
			StartEvent(enter.GetComponent<CharacterData>());
	}

	public void StartEvent(CharacterData character)
	{
		if (m_completed || m_started || m_cd > 0 || !ConditionData.JudgmentList(Conditions)) return;
		if (++m_triggerTimes < triggerTimes) return;
		m_started = true;
		UIRoleHud hud = character.GetComponentInChildren<UIRoleHud>();
		switch (eventType)
		{
			case EventType.PlayerDialogue:
				if (character == GameManager.CurHero)
				{
					StartCoroutine(Dialogue(hud));
					GameManager.StoryMode = true;
				}
				break;
			case EventType.Bubble:
				StartCoroutine(Dialogue(hud));
				break;
			default:
				break;
		}
	}

	IEnumerator Dialogue(UIRoleHud hud)
	{
		foreach (var dialog in Dialogues)
		{
			hud.Bubble(dialog.Key);
			yield return new WaitForSeconds(dialog.Value);
		}
		hud.Bubble("", 1.0f);
		EndDialogue();
	}

	void EndDialogue()
	{
		m_started = false;
		if (eventType == EventType.PlayerDialogue) GameManager.StoryMode = false;
		if (TriggerEffects != null && TriggerEffects.Count > 0)
			EffectData.TakeEffects(TriggerEffects, GameManager.CurHero.gameObject, GameManager.CurHero.gameObject);
		if (--loopTimes < 1) m_completed = true;
		else m_cd = cdTimes;
	}

	public void StopDialogue(CharacterData character)
	{
		m_completed = true;
		if (eventType == EventType.PlayerDialogue) GameManager.StoryMode = false;
	}
}
