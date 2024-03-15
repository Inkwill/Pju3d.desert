using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UIRoleHud : UIHudBase
{
	[SerializeField]
	TMP_Text textName;
	[SerializeField]
	Slider sliderPg;
	[SerializeField]
	Animator story_anim;
	[SerializeField]
	Slider sliderInteract;
	[SerializeField]
	Button btPlace;
	Character m_character;

	void Start()
	{
		m_character = GetComponentInParent<Character>();
		if (m_character.BaseAI != null) m_character.StateUpdateAction += OnCharacterStating;
		m_character.Inventory.ItemAction += OnItemAction;
		m_character.SkillAction += OnSkillAction;
		sliderPg?.gameObject.SetActive(false);
		sliderInteract?.gameObject.SetActive(false);
		m_character.DeathEvent.AddListener((character) =>
		{
			sliderPg?.gameObject.SetActive(false);
		});
		btPlace.gameObject.SetActive(false);
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

	void OnItemAction(Item item, string eventName, int num)
	{
		if (eventName == "NotEnoughSpace")
		{
			GetComponentInChildren<UIRoleHud>()?.Bubble("Not enough space of " + item.ItemName + "!");
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

	public void ShowPlaceButton()
	{
		btPlace.gameObject.SetActive(true);
		ShowUIHud();
	}

	public void OnClick_Place()
	{
		m_character.GetComponent<PlaceHandle>()?.PlaceDevice();
		btPlace.gameObject.SetActive(false);
		ShowWorldHud();
	}
}