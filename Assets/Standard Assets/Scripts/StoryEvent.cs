using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	StoryEventData data;
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
		StartEvent(enter.GetComponent<Character>());
	}

	public void OnStay(GameObject enter, float during)
	{
		if (during >= data.stayTime) { StartEvent(enter.GetComponent<Character>()); m_target = enter; }
	}

	public void StartEvent(Character character)
	{
		if (m_completed || m_started || m_cd > 0 || !ConditionData.JudgmentList(data.Conditions)) return;
		if (++m_triggerTimes < data.triggerTimes) return;
		m_started = true;
		UIHudBase target_hud = character.GetComponentInChildren<UIHudBase>();
		UIHudBase self_hud = GetComponentInChildren<UIHudBase>();
		StartCoroutine(Dialogue(self_hud, target_hud));
		GameManager.StoryMode = data.storyMode;
	}

	IEnumerator Dialogue(UIHudBase self_hud, UIHudBase target_hud)
	{
		foreach (var dialog in data.Dialogues)
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
		if (data.storyMode) GameManager.StoryMode = false;
		StoryEndEvents?.Invoke(gameObject, m_target);
		m_target = null;
		if (--data.loopTimes < 1) m_completed = true;
		else m_cd = data.cd;
	}

	public void StopDialogue(Character character)
	{
		m_completed = true;
		if (data.storyMode) GameManager.StoryMode = false;
	}
}
