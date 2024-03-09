using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	bool storyMode;
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
	List<KeyValueData.KeyValue<string, string>> Dialogues;
	public UnityEvent<GameObject, GameObject> StoryEndEvents;
	GameObject m_target;
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
		m_target = damage.Source.gameObject;
		StartEvent(damage.Source);
	}
	public void OnEnter(GameObject enter)
	{
		m_target = enter;
		StartEvent(enter.GetComponent<CharacterData>());
	}

	public void OnStay(GameObject enter, float during)
	{
		if (during >= stayTimes) { StartEvent(enter.GetComponent<CharacterData>()); m_target = enter; }
	}

	public void StartEvent(CharacterData character)
	{
		if (m_completed || m_started || m_cd > 0 || !ConditionData.JudgmentList(Conditions)) return;
		if (++m_triggerTimes < triggerTimes) return;
		m_started = true;
		UIWorldHud target_hud = character.GetComponentInChildren<UIWorldHud>();
		UIWorldHud self_hud = GetComponentInChildren<UIWorldHud>();
		StartCoroutine(Dialogue(self_hud, target_hud));
		GameManager.StoryMode = storyMode;
	}

	IEnumerator Dialogue(UIWorldHud self_hud, UIWorldHud target_hud)
	{
		foreach (var dialog in Dialogues)
		{
			var hud = (dialog.Key == "target") ? target_hud : self_hud;
			hud?.Bubble(dialog.Value);
			yield return new WaitForSeconds(1.5f);
		}
		EndDialogue();
	}

	void EndDialogue()
	{
		m_started = false;
		if (storyMode) GameManager.StoryMode = false;
		StoryEndEvents?.Invoke(gameObject, m_target);
		m_target = null;
		if (--loopTimes < 1) m_completed = true;
		else m_cd = cdTimes;
	}

	public void StopDialogue(CharacterData character)
	{
		m_completed = true;
		if (storyMode) GameManager.StoryMode = false;
	}
}
