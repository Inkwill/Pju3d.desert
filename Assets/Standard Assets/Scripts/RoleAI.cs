using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using MyBox;

[RequireComponent(typeof(RoleControl))]
public class RoleAI : MonoBehaviour
{
	protected RoleControl m_role;

	public virtual void Init()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);

		if (m_role.EnemyDetector)
		{
			m_role.EnemyDetector.OnEnter.AddListener(OnEnemyEnter);
			m_role.EnemyDetector.OnExit.AddListener(OnEnemyExit);
			m_role.EnemyDetector.OnEvent.AddListener(OnEnemyEvent);
		}
		if (m_role.SkillDetector)
		{
			m_role.SkillDetector.OnEnter.AddListener(OnSkillTargetEnter);
			m_role.SkillDetector.OnExit.AddListener(OnSkillTargetExit);
			m_role.SkillDetector.OnEvent.AddListener(OnSkillTargetEvent);
		}
		if (m_role.Interactor) m_role.Interactor.OnInteracting.AddListener(OnInteracting);
	}

	void Update()
	{
		if (m_role.isIdle && m_role.EnemyDetector.Inners.Count > 0)
			m_role.CurrentEnemy = m_role.EnemyDetector.GetNearest().GetComponent<CharacterData>();
	}

	protected virtual void OnRoleEvent(GameObject obj, string eventName)
	{
		// UIRoleHud hud = m_role.GetComponentInChildren<UIRoleHud>();
		// if (hud != null) hud.Bubble(eventName);
	}

	protected virtual void OnEnemyEnter(GameObject enter)
	{
		if (m_role.CurrentEnemy == null) m_role.CurrentEnemy = enter.GetComponent<CharacterData>();
		//enter.GetComponent<EventSender>()?.events.AddListener(OnEnemyEvent);
		//m_eventSender.Send(enter, "roleEvent_OnEnemyEnter");
	}

	protected virtual void OnEnemyExit(GameObject exiter)
	{
		if (m_role.CurrentEnemy && m_role.CurrentEnemy.gameObject == exiter)
		{
			m_role.CurrentEnemy = m_role.EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
			//m_eventSender.Send(exiter, "roleEvent_OnEnemyExit");
		}
	}
	protected virtual void OnEnemyEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			//m_role.CurState = RoleControl.State.IDLE;
			//Debug.Log("OnEnemyEvent: target= " + sender + "event= " + eventMessage);
		}
	}

	void OnSkillTargetEnter(GameObject enter)
	{
		if (m_role.SkillUser && m_role.SkillUser.CurSkill != null) m_role.SkillUser.AddTarget(enter);
		//Debug.Log("OnSkillTargetEnter: enter= " + enter);
	}

	void OnSkillTargetExit(GameObject exiter)
	{
		if (m_role.SkillUser && m_role.SkillUser.CurSkill != null) m_role.SkillUser.RemoveTarget(exiter);
		//Debug.Log("OnSkillTargetExit: exiter= " + exiter);
	}
	void OnSkillTargetEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			Debug.Log("OnSkillTargetEvent: target= " + sender + "event= " + eventMessage);
		}
	}

	protected virtual void OnInteracting(InteractHandle interactor, string eventName)
	{
		if (eventName == "Start") m_role.LookAt(interactor.gameObject.transform);
		//HighlightTarget(interactor.gameObject, true);
		//Debug.Log("[RoleAI-" + m_role + "] OnInteracting with : " + interactor.gameObject);
	}
	void HighlightTarget(GameObject obj, bool active)
	{
		HighlightableObject target = obj.GetComponent<HighlightableObject>();
		if (target)
		{
			if (active) target.Highlight();
			else target.Dehighlight();
		}
	}
}

// #if UNITY_EDITOR
// [CustomEditor(typeof(RoleAI))]
// public class RoleAIInspector : Editor  
// {
// 	SerializedObject obj;  
// 	RoleAI roleAi;  
// 	SerializedProperty camp;  
// 	SerializedProperty m_Offensive; 

// 	void OnEnable()
// 	{  
//     	obj = new SerializedObject(target);  
//     	m_Offensive = obj.FindProperty("m_Offensive");
// 	}

// 	public override void OnInspectorGUI()
// 	{
// 		base.OnInspectorGUI();
// 		roleAi = (RoleAI)target;
// 		if (roleAi.camp == RoleAI.Camp.ENEMY)
// 		{
// 			EditorGUILayout.PropertyField(m_Offensive);
// 		}
// 	}
// } 
// #endif
