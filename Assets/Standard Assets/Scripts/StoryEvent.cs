using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryEvent : MonoBehaviour
{
	[SerializeField]
	StoryEventData m_data;
	public UnityEvent<GameObject, GameObject> endEvent;
	GameObject m_target;
	bool m_started;
	int m_triggerTimes;
	int m_loopTimes;
	float m_cd;

	public void Init(StoryEventData data)
	{
		m_data = data;
	}
	void Start()
	{
		switch (m_data.triggerType)
		{
			case StoryEventData.TriggerType.AutoStart:
				StartEvent(gameObject);
				break;
			case StoryEventData.TriggerType.EnterArea:
				AddAreaTrigger().OnEnter.AddListener(enter => StartEvent(enter));
				break;
			case StoryEventData.TriggerType.StayArea:
				AddAreaTrigger().OnStay.AddListener((stayer, during) => { if (during >= m_data.stayTime) StartEvent(stayer); });
				break;
			case StoryEventData.TriggerType.Damaged:
				Character character = GetComponent<Character>();
				if (character) character.DamageEvent.AddListener(damage => StartEvent(damage.Source.gameObject));
				break;
			default:
				break;
		}
		m_loopTimes = m_data.loopTimes;
	}

	InteractOnTrigger AddAreaTrigger()
	{
		var collier = gameObject.AddComponent<BoxCollider>();
		collier.isTrigger = true;
		collier.size = new Vector3(m_data.areaRadius, 1, m_data.areaRadius);
		var trigger = gameObject.AddComponent<InteractOnTrigger>();
		trigger.layers = LayerMask.GetMask("Player");
		trigger.once = m_data.loopTimes == 1;
		trigger.OnEnter = new UnityEvent<GameObject>();
		trigger.OnStay = new UnityEvent<GameObject, float>();
		return trigger;
	}

	void Update()
	{
		if (m_cd > 0) m_cd -= Time.deltaTime;
	}

	public void StartEvent(GameObject target)
	{
		m_target = target;
		if (m_started || m_cd > 0) return;

		Character character = target.GetComponent<Character>();
		if (character && !ConditionData.JudgmentList(character, m_data.Conditions)) return;

		if (++m_triggerTimes < m_data.triggerTimes) return;

		m_started = true;
		m_triggerTimes = 0;

		if (m_data.storyType == StoryEventData.StoryType.Dialogue)
		{
			UIHudBase target_hud = character.GetComponentInChildren<UIHudBase>();
			UIHudBase self_hud = GetComponentInChildren<UIHudBase>();
			StartCoroutine(Dialogue(self_hud, target_hud));
			GameManager.StoryMode = m_data.storyMode;
		}
	}

	IEnumerator Dialogue(UIHudBase self_hud, UIHudBase target_hud)
	{
		foreach (var dialog in m_data.Dialogues)
		{
			var hud = (dialog.Key == "target") ? target_hud : self_hud;
			hud?.Bubble(dialog.Value);
			yield return new WaitForSeconds(1.5f);
		}
		EndStoryEvent();
	}

	void EndStoryEvent()
	{
		m_started = false;
		if (m_data.storyMode) GameManager.StoryMode = false;
		endEvent?.Invoke(gameObject, m_target);
		EffectData.TakeEffects(m_data.Effects, gameObject, m_target);

		if (--m_loopTimes >= 1)
		{
			m_cd = m_data.cd;
			return;
		}
		m_target = null;
		if (m_data.triggerType == StoryEventData.TriggerType.AutoStart || m_data.triggerType == StoryEventData.TriggerType.Damaged)
			Destroy(this);
		else if (m_data.triggerType == StoryEventData.TriggerType.EnterArea || m_data.triggerType == StoryEventData.TriggerType.StayArea)
			Destroy(gameObject);
	}
}
