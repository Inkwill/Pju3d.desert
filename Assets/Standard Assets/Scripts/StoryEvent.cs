using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	int TriggerTimes = 1;
	[SerializeField]
	List<KeyValueData.KeyValue<ConditionData, string[]>> Conditions;
	[SerializeField]
	List<KeyValueData.KeyValue<string, float>> Dialogues;
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> TriggerEffects;
	bool m_completed;
	int m_triggerTimes;

	public void OnDamageEvent(Damage damage)
	{
		OnTrigger(damage.Source);
	}
	public void OnTriggerEvent(GameObject player)
	{
		OnTrigger(player.GetComponent<CharacterData>());
	}

	void OnTrigger(CharacterData player)
	{
		if (m_completed || !ConditionData.JudgmentList(Conditions)) return;
		if (++m_triggerTimes < TriggerTimes) return;
		if (player == GameManager.CurHero) StartStoryEvent();
	}

	void StartStoryEvent()
	{
		// m_player.BaseAI.Stop();
		// m_player.BaseAI.LookAt(Camera.main.transform);
		GameManager.StoryMode = true;
		UIRoleHud hud = GameManager.CurHero.GetComponentInChildren<UIRoleHud>();
		StartCoroutine(PalyStory(hud));
	}

	IEnumerator PalyStory(UIRoleHud hud)
	{
		foreach (var dialog in Dialogues)
		{
			hud.Bubble(dialog.Key);
			yield return new WaitForSeconds(dialog.Value);
		}
		hud.Bubble("", 1.0f);
		EndStory();
	}

	void EndStory()
	{
		m_completed = true;
		GameManager.StoryMode = false;
		EffectData.TakeEffects(TriggerEffects, GameManager.CurHero.gameObject, GameManager.CurHero.gameObject);
	}
}
