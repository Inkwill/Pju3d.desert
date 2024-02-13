using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using MyBox;

[RequireComponent(typeof(RoleControl))]
public class RoleAI : MonoBehaviour
{
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger SceneDetector;
	public InteractOnTrigger SkillDetector;

	protected RoleControl m_role;
	GameObject m_interactTarget;

	public virtual void Init(RoleControl role)
	{
		m_role = role;
		m_role.eventSender.events.AddListener(OnRoleEvent);

		if (EnemyDetector)
		{
			EnemyDetector.OnEnter.AddListener(OnEnemyEnter);
			EnemyDetector.OnExit.AddListener(OnEnemyExit);
			EnemyDetector.OnEvent.AddListener(OnEnemyEvent);
		}
		if (InteractDetector)
		{
			InteractDetector.OnEnter.AddListener(OnInteracterEnter);
			InteractDetector.OnExit.AddListener(OnInteracterExit);
			InteractDetector.OnEvent.AddListener(OnInteracterEvent);
		}
		if (SkillDetector)
		{
			SkillDetector.OnEnter.AddListener(OnSkillTargetEnter);
			SkillDetector.OnExit.AddListener(OnSkillTargetExit);
			SkillDetector.OnEvent.AddListener(OnSkillTargetEvent);
		}
		if (SceneDetector)
		{
			SceneDetector.layers = LayerMask.GetMask("Scene");
		}
	}

	void Update()
	{
		if (m_role.isIdle && EnemyDetector.Inners.Count > 0)
			m_role.CurrentEnemy = EnemyDetector.GetNearest().GetComponent<CharacterData>();
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
			m_role.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
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

	void OnInteracterEnter(GameObject enter)
	{

	}

	void OnInteracterExit(GameObject exiter)
	{
		if (m_interactTarget && m_interactTarget == exiter) m_interactTarget = InteractDetector.GetNearest();
	}

	void OnInteracterEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD" && m_interactTarget == sender)
		{
			m_interactTarget = InteractDetector.GetNearest();
		}
		if (eventMessage == "roleEvent_OnState_IDLE" && m_interactTarget == null && m_role.isIdle)
		{
			m_interactTarget = sender;
			GetComponentInChildren<UIRoleHud>()?.Bubble(sender.GetComponent<RoleControl>()?.Data.CharacterName);
			m_role.LookAt(sender.transform);
		}
		//Debug.Log("OnInteracterEvent: InteracterTarget= " + sender + "event= " + eventMessage);
	}

	void OnSkillTargetEnter(GameObject enter)
	{
		if (m_role.SkillUser && m_role.SkillUser.CurSkill != null) m_role.SkillUser.AddTarget(enter);
		Debug.Log("OnSkillTargetEnter: enter= " + enter);
	}

	void OnSkillTargetExit(GameObject exiter)
	{
		if (m_role.SkillUser && m_role.SkillUser.CurSkill != null) m_role.SkillUser.RemoveTarget(exiter);
		Debug.Log("OnSkillTargetExit: exiter= " + exiter);
	}
	void OnSkillTargetEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			Debug.Log("OnSkillTargetEvent: target= " + sender + "event= " + eventMessage);
		}
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
