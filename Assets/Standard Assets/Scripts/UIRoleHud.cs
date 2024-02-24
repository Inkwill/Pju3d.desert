using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;
using UnityEngine.InputSystem.Controls;

public class UIRoleHud : MonoBehaviour
{
	[SerializeField]
	TMP_Text textName;
	[SerializeField]
	Slider sliderHp;
	[SerializeField]
	Slider sliderPg;
	[SerializeField]
	Animator bubble_anim;
	[SerializeField]
	Text bubble_text;
	[SerializeField]
	Animator story_anim;
	[SerializeField]
	Slider sliderInteract;
	CharacterData m_character;

	void Start()
	{
		m_character = GetComponentInParent<CharacterData>();
		if (m_character) m_character.Inventory.ItemEvent += OnItemEvent;
		m_character.GetComponent<EventSender>()?.events.AddListener(OnCharacterEvent);

		if (sliderHp)
		{
			sliderHp.maxValue = m_character.Stats.stats.health;
			sliderHp.value = m_character.Stats.CurrentHealth;
			sliderHp.gameObject.SetActive(false);
		}
		sliderPg?.gameObject.SetActive(false);
		sliderInteract?.gameObject.SetActive(false);
		GetComponentInParent<StoryTeller>()?.tellerEvent.AddListener(OnTellerEvent);
		//iconStory?.gameObject.SetActive(false);
	}

	void OnCharacterEvent(GameObject role, string eventName)
	{
		switch (eventName)
		{
			case "roleEvent_OnState_IDLE":
				sliderHp?.gameObject.SetActive(false);
				break;
			case "roleEvent_OnState_ATTACKING":
			case "roleEvent_OnState_PURSUING":
				sliderHp?.gameObject.SetActive(true);
				break;
			case "roleEvent_OnState_SKILLING":
				sliderPg?.gameObject.SetActive(true);
				sliderPg.maxValue = m_character.SkillUser.CurSkill.skill.Duration;
				break;
			case "characterEvent_OnDeath":
				sliderHp?.gameObject.SetActive(false);
				sliderPg?.gameObject.SetActive(false);
				GetComponentInParent<EventSender>()?.events.RemoveListener(OnCharacterEvent);
				break;
			case "characterEvent_OnHpChange":
				if (!sliderHp.gameObject.activeSelf) sliderHp.gameObject.SetActive(true);
				sliderHp.value = m_character.Stats.CurrentHealth;
				break;
			case "skillEvent_OnOperat":
				if (!sliderPg.gameObject.activeSelf) sliderPg.gameObject.SetActive(true);
				sliderPg.value = m_character.GetComponent<AIBase>().CurStateDuring;
				break;
			case "skillEvent_OnImplement":
				sliderPg?.gameObject.SetActive(false);
				break;
			default:
				break;
		}
	}

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		Bubble(actionName + "item = " + item.ItemName + "count=" + itemCount);
	}

	void OnTellerEvent(StoryTeller teller, string eventName)
	{
		// if (eventName == "tellStory")
		// {
		// 	if (teller.CurrentNode != null) story_anim.SetTrigger("show");
		// 	else story_anim.SetTrigger("hide");
		// }

	}
	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
	}

	public void Bubble(string content, float duration = 1.0f)
	{
		// if (bubble_anim.GetCurrentAnimatorStateInfo(0).IsName("hide"))
		// {

		// }
		// else
		// {
		// 	bubble_text.text = content;
		// }
		bubble_text.text = content;
		bubble_anim.SetTrigger("show");
		StartCoroutine(GameManager.Instance.WaitAction(duration, () => bubble_anim.SetTrigger("hide")));
	}
}