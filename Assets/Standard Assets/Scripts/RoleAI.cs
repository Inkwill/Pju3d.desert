using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using UnityEngine.AI;
using CreatorKitCodeInternal;
using MyBox;

[RequireComponent(typeof(RoleControl))]
public class RoleAI : MonoBehaviour
{
	[Header("Detector")]
	public InteractOnTrigger SceneDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger SkillDetector;
	public string SceneBox { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, false); } }
	public string SceneBoxName { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, true); } }
	public NavMeshAgent Agent => m_Agent;
	protected NavMeshAgent m_Agent;
	protected RoleControl m_role;
	protected Vector3 m_Destination = Vector3.zero;
	NavMeshPath m_CalculatedPath;

	public virtual void Init()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);

		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.speed = m_role.Speed;
		m_Agent.angularSpeed = 360.0f;
		m_CalculatedPath = new NavMeshPath();

		if (EnemyDetector)
		{
			EnemyDetector.OnEnter.AddListener(OnEnemyEnter);
			EnemyDetector.OnExit.AddListener(OnEnemyExit);
			EnemyDetector.OnEvent.AddListener(OnEnemyEvent);
		}
		if (SkillDetector)
		{
			SkillDetector.OnEnter.AddListener(OnSkillTargetEnter);
			SkillDetector.OnExit.AddListener(OnSkillTargetExit);
			SkillDetector.OnEvent.AddListener(OnSkillTargetEvent);
		}
		if (InteractDetector)
		{
			InteractDetector.OnStay.AddListener(OnInteractStay);
			// m_role.InteractDetector.OnExit.AddListener(OnInteracterExit);
			// m_role.InteractDetector.OnEvent.AddListener(OnInteractorEvent);
		}
	}

	void Start()
	{
		m_role.CurState = RoleControl.State.IDLE;
	}

	void Update()
	{
		if (m_role.isIdle && EnemyDetector.Inners.Count > 0)
			m_role.CurrentEnemy = EnemyDetector.GetNearest().GetComponent<CharacterData>();
	}

	public virtual void LookAt(Transform trans)
	{
		Vector3 forward = (trans.position - m_role.transform.position);
		forward.y = 0;
		forward.Normalize();
		m_role.transform.forward = forward;
	}

	public virtual void Move(Vector3 direction)
	{
		if (direction.magnitude > 0)
		{
			Agent.CalculatePath(transform.position + direction, m_CalculatedPath);
			if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
			{
				Agent.SetPath(m_CalculatedPath);
				m_CalculatedPath.ClearCorners();
				m_role.CurState = RoleControl.State.MOVE;
			}
		}
	}

	public virtual void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		m_role.CurState = RoleControl.State.MOVE;
	}


	protected virtual void OnRoleEvent(GameObject obj, string eventName)
	{
		//MOVE
		if (eventName == "roleEvent_OnMoving")
		{
			if (m_Destination != Vector3.zero && Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
			{
				GetComponent<EventSender>()?.Send(gameObject, "roleAIEvent_OnMoveEnd");
				m_role.CurState = RoleControl.State.IDLE;
				return;
			}
		}
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
	protected virtual void OnInteractStay(GameObject interactor, float during)
	{
		if (interactor.tag != "item") LookAt(interactor.transform);
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
