﻿using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Need to be added to the GameObject that have the AnimatorController. This will receive the Event defined in the
/// import of the animations and can dispatch them to some receivers. Used by step event and attack frame event on
/// characters.
/// </summary>
public class AnimationDispatcher : MonoBehaviour
{
	public UnityEvent AttackStep;
	public UnityEvent FootStep;
	public UnityEvent SkillStep;
	RoleControl m_role;
	Animator m_Animator;

	[SerializeField]
	string DeathTrigger = "Death";
	[SerializeField]
	string SpeedTrigger = "Speed";
	[SerializeField]
	string AttackTrigger = "Attack";
	[SerializeField]
	string HitTrigger = "Hit";
	[SerializeField]
	string RespawnTrigger = "Respawn";
	[SerializeField]
	string FaintTrigger = "Faint";
	void Start()
	{
		m_role = GetComponentInParent<RoleControl>();
		if (m_role) m_role.eventSender.events.AddListener(OnRoleEvent);
		m_Animator = GetComponent<Animator>();
	}

	void OnRoleEvent(GameObject obj, string eventName)
	{
		switch (eventName)
		{
			case "roleEvent_OnState_DEAD":
				m_Animator.SetTrigger(DeathTrigger);
				return;
			case "roleEvent_OnDamage":
				m_Animator.SetTrigger(HitTrigger);
				break;
			case "roleEvent_OnAttack":
				m_Animator.SetTrigger(AttackTrigger);
				break;
			default:
				break;
		}
		m_Animator.SetFloat(SpeedTrigger, m_role.BaseAI.SpeedScale);
		// if (eventName == "roleEvent_OnDamage") m_Animator.SetTrigger(HitTrigger);
		// if (eventName == "roleEvent_OnState_DEAD") m_Animator.SetTrigger(Animator.StringToHash(DeathTrigger));
		// if (eventName == "roleEvent_OnState_ATTACKING") m_Animator.SetTrigger(AttackTrigger);
		// if (eventName == "roleEvent_OnIdling" || eventName == "roleEvent_OnMoving" || eventName == "roleEvent_OnPursuing" || eventName == "roleEvent_OnState_ATTACKING")
		// 	m_Animator.SetFloat(SpeedTrigger, m_role.BaseAI.SpeedScale);

	}

	void AttackEvent()
	{
		AttackStep?.Invoke();
	}

	void FootstepEvent()
	{
		FootStep?.Invoke();
	}
	void SkillstepEvent()
	{
		SkillStep?.Invoke();
	}
}
