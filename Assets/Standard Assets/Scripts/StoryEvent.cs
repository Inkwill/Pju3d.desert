using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	List<KeyValueData.KeyValue<string, float>> Dialogues;
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> TriggerEffects;
	bool m_completed;
	CharacterData m_player;

	public void OnDamageEvent(Damage damage)
	{
		if (m_completed) return;
		m_player = damage.Source;
		if (m_player == GameManager.CurHero) StartStoryEvent();
	}
	public void OnTriggerEvent(GameObject player, string eventName)
	{
		if (m_completed) return;
		if (eventName == "Ready")
		{
			m_player = player.GetComponent<CharacterData>();
			if (m_player == GameManager.CurHero) StartStoryEvent();
		}
	}

	void StartStoryEvent()
	{
		// m_player.BaseAI.Stop();
		// m_player.BaseAI.LookAt(Camera.main.transform);
		GameManager.StoryMode = true;
		UIRoleHud hud = m_player.GetComponentInChildren<UIRoleHud>();
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
		EffectData.TakeEffects(TriggerEffects, m_player.gameObject, m_player.gameObject);
	}
}
