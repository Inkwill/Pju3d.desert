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
	RoleControl m_role;

	void Start()
	{
		m_role = GetComponentInParent<RoleControl>();
		if (m_role)
		{
			m_role.eventSender.events.AddListener(OnRoleEvent);
			m_role.Data.Inventory.ItemEvent += OnItemEvent;
		}

		if (sliderHp)
		{
			sliderHp.maxValue = m_role.Data.Stats.stats.health;
			sliderHp.value = m_role.Data.Stats.CurrentHealth;
			sliderHp.gameObject.SetActive(false);
		}
		sliderPg?.gameObject.SetActive(false);
		sliderInteract?.gameObject.SetActive(false);
		GetComponentInParent<StoryTeller>()?.tellerEvent.AddListener(OnTellerEvent);
		//iconStory?.gameObject.SetActive(false);
	}

	void OnRoleEvent(GameObject role, string eventName)
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
				sliderPg.maxValue = m_role.SkillUser.CurSkill.skill.Duration;
				break;
			case "roleEvent_OnState_DEAD":
				sliderHp?.gameObject.SetActive(false);
				sliderPg?.gameObject.SetActive(false);
				m_role.eventSender.events.RemoveListener(OnRoleEvent);
				break;
			case "statEvent_OnHpChange":
				if (!sliderHp.gameObject.activeSelf) sliderHp.gameObject.SetActive(true);
				sliderHp.value = m_role.Data.Stats.CurrentHealth;
				break;
			case "skillEvent_OnOperat":
				if (!sliderPg.gameObject.activeSelf) sliderPg.gameObject.SetActive(true);
				sliderPg.value = m_role.CurStateDuring;
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
		if (bubble_anim.GetCurrentAnimatorStateInfo(0).IsName("hide"))
		{
			bubble_text.text = content;
			bubble_anim.SetTrigger("show");
			StartCoroutine(GameManager.Instance.WaitAction(duration, () => bubble_anim.SetTrigger("hide")));
		}
		else
		{
			StartCoroutine(GameManager.Instance.WaitAction(1.0f, () => Bubble(content, duration)));
		}
	}
}