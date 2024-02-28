using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	float m_TriggerTime;
	[SerializeField]
	List<KeyValueData.KeyValue<string, float>> Dialogues;
	CharacterData m_player;
	void Start()
	{
		GetComponent<InteractOnTrigger>()?.OnEnter.AddListener(OnPlayerEnter);
		GetComponent<InteractOnTrigger>()?.OnStay.AddListener(OnPlayerStay);
	}
	public void OnPlayerEnter(GameObject enter)
	{
		if (m_TriggerTime > 0) return;
		var player = enter.GetComponent<CharacterData>();
		if (player == GameManager.CurHero) m_player = player;
		if (m_player) StartStoryEvent();
	}

	public void OnPlayerStay(GameObject stayer, float during)
	{
		if (m_TriggerTime <= 0) return;
		var player = stayer.GetComponent<CharacterData>();
		if (player == GameManager.CurHero) m_player = player;
		if (m_player && during >= m_TriggerTime) StartStoryEvent();

	}


	void StartStoryEvent()
	{
		GameManager.StoryMode = true;
		m_player.BaseAI.Stop();
		m_player.BaseAI.LookAt(Camera.main.transform);
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
		GameManager.StoryMode = false;
		GameManager.GameGoal.Init();
		Destroy(gameObject);
	}
}
