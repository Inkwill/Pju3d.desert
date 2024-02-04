using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;
using TMPro;

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
	RoleControl m_role;

	void Start()
	{
		m_role = GetComponentInParent<RoleControl>();
		if (m_role) m_role.eventSender.events.AddListener(OnRoleEvent);
		if (sliderHp)
		{
			sliderHp.maxValue = m_role.Data.Stats.stats.health;
			sliderHp.value = m_role.Data.Stats.CurrentHealth;
			sliderHp.gameObject.SetActive(false);
		}
		sliderPg?.gameObject.SetActive(false);
	}

	void OnRoleEvent(GameObject role, string eventName)
	{
		switch (eventName)
		{
			case "roleEvent_OnState_ATTACKING":
			case "roleEvent_OnState_PURSUING":
				sliderHp?.gameObject.SetActive(true);
				break;
			case "roleEvent_OnState_SKILLING":
				sliderPg?.gameObject.SetActive(true);
				sliderPg.maxValue = m_role.CurSkill.Duration;
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
			case "roleEvent_OnOperatSkill":
				if (!sliderPg.gameObject.activeSelf) sliderPg.gameObject.SetActive(true);
				sliderPg.value = m_role.CurStateDuring;
				break;
			case "roleEvent_OnImplementSkill":
				sliderPg?.gameObject.SetActive(false);
				break;
			default:
				break;
		}
	}
	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
	}

	public void Bubble(string content, float duration = 1.0f)
	{
		bubble_text.text = content;
		bubble_anim.SetTrigger("show");
		StartCoroutine(WaitAndPrint(duration));

	}
	IEnumerator WaitAndPrint(float waitTime)
	{
		// 等待一定的时间
		yield return new WaitForSeconds(waitTime);
		bubble_anim.SetTrigger("hide");
	}
}