using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UIRoleHud : UIWorldHud
{
	[SerializeField]
	TMP_Text textName;
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
		if (m_character.BaseAI != null) m_character.StateUpdateAction += OnCharacterStating;
		m_character.SkillAction += OnSkillAction;
		sliderPg?.gameObject.SetActive(false);
		sliderInteract?.gameObject.SetActive(false);
		m_character.DeathEvent.AddListener((character) =>
		{
			sliderPg?.gameObject.SetActive(false);
		});
	}

	void OnCharacterStating(AIBase.State curState)
	{
		switch (curState)
		{
			// case AIBase.State.IDLE:
			// 	sliderHp?.gameObject.SetActive(false);
			// 	break;
			// case AIBase.State.ATTACKING:
			// case AIBase.State.PURSUING:
			// 	sliderHp?.gameObject.SetActive(true);
			// 	break;
			case AIBase.State.SKILLING:
				sliderPg?.gameObject.SetActive(true);
				sliderPg.maxValue = m_character.SkillUser.CurSkill.skill.Duration;
				break;

		}
	}

	void OnSkillAction(Skill skill, string eventName)
	{
		switch (eventName)
		{
			case "Operat":
				if (!sliderPg.gameObject.activeSelf) sliderPg.gameObject.SetActive(true);
				sliderPg.value = m_character.GetComponent<AIBase>().CurStateDuring;
				break;
			case "Implement":
				sliderPg?.gameObject.SetActive(false);
				break;
			default:
				break;
		}
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