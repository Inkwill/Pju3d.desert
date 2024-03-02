using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;
using CreatorKitCodeInternal;

public class UIRoleHud : UIWorldHud
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
		m_character.OnDamage.AddListener((damage) =>
		{
			sliderHp.gameObject.SetActive(true);
			sliderHp.value = m_character.Stats.CurrentHealth;
			DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
		});
		if (m_character.BaseAI != null) m_character.BaseAI.StateUpdateEvent += OnCharacterStating;
		//m_character.Inventory.ItemEvent += OnItemEvent;
		m_character.GetComponent<EventSender>()?.events.AddListener(OnCharacterEvent);

		if (sliderHp)
		{
			sliderHp.maxValue = m_character.Stats.stats.health;
			sliderHp.value = m_character.Stats.CurrentHealth;
			sliderHp.gameObject.SetActive(false);
		}
		sliderPg?.gameObject.SetActive(false);
		sliderInteract?.gameObject.SetActive(false);
		//bubble_anim?.gameObject.SetActive(false);
		//GetComponentInParent<StoryTeller>()?.tellerEvent.AddListener(OnTellerEvent);
		m_character.OnDeath.AddListener((character) =>
		{
			sliderHp?.gameObject.SetActive(false);
			sliderPg?.gameObject.SetActive(false);
			//GetComponentInParent<EventSender>()?.events.RemoveListener(OnCharacterEvent);
		});
	}

	void OnCharacterStating(AIBase.State curState)
	{
		switch (curState)
		{
			case AIBase.State.IDLE:
				sliderHp?.gameObject.SetActive(false);
				break;
			case AIBase.State.ATTACKING:
			case AIBase.State.PURSUING:
				sliderHp?.gameObject.SetActive(true);
				break;
			case AIBase.State.SKILLING:
				sliderPg?.gameObject.SetActive(true);
				sliderPg.maxValue = m_character.BaseAI.SkillUser.CurSkill.skill.Duration;
				break;

		}
	}

	void OnCharacterEvent(GameObject character, string eventName)
	{

		switch (eventName)
		{
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
		//Bubble(actionName + "item = " + item.ItemName + "count=" + itemCount);
	}

	void OnTellerEvent(StoryTeller teller, string eventName)
	{
		// if (eventName == "tellStory")
		// {
		// 	if (teller.CurrentNode != null) story_anim.SetTrigger("show");
		// 	else story_anim.SetTrigger("hide");
		// }

	}

	public void Bubble(string content, float duration = 0f)
	{
		if (!bubble_anim.gameObject.activeSelf) bubble_anim.gameObject.SetActive(true);
		else bubble_anim.SetBool("show", true);
		StartCoroutine(ShowText(content));
		if (duration > 0) GameManager.StartWaitAction(duration, () => bubble_anim.SetBool("show", false));
	}

	IEnumerator ShowText(string content)
	{
		for (int i = 0; i < content.Length; i++)
		{
			bubble_text.text = content.Substring(0, i + 1);
			yield return new WaitForSeconds(0.1f);
		}
	}
}