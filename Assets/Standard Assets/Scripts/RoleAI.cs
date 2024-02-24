using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using UnityEngine.AI;
using CreatorKitCodeInternal;
using MyBox;

[RequireComponent(typeof(RoleControl))]
public class RoleAI : AIBase
{
	public NavMeshAgent Agent => m_Agent;
	protected NavMeshAgent m_Agent;
	public override float SpeedScale { get { return Agent.velocity.magnitude / m_character.MoveSpeed; } }
	protected Vector3 m_Destination = Vector3.zero;
	protected RoleControl m_role;
	NavMeshPath m_CalculatedPath;


	public override void Init()
	{
		base.Init();
		m_role = GetComponent<RoleControl>();
		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.speed = m_character.MoveSpeed;
		m_Agent.angularSpeed = 360.0f;
		m_CalculatedPath = new NavMeshPath();
	}
	public void Move(Vector3 direction)
	{
		if (direction.magnitude > 0)
		{
			Agent.CalculatePath(transform.position + direction, m_CalculatedPath);
			if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
			{
				Agent.SetPath(m_CalculatedPath);
				m_CalculatedPath.ClearCorners();
				Agent.isStopped = false;
				m_role.SetState(RoleControl.State.MOVE);
			}
		}
	}

	public void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		m_Agent.SetDestination(pos);
		Agent.isStopped = false;
		m_role.SetState(RoleControl.State.MOVE);
	}
	public override void Stop()
	{
		Agent.velocity = Vector3.zero;
		Agent.isStopped = true;
		Agent.ResetPath();
	}

	protected override void OnIdlingAI()
	{
		if (m_character.CurrentEnemy != null) { m_role.SetState(RoleControl.State.PURSUING); return; }
		else if (EnemyDetector.Inners.Count > 0)
		{
			m_character.CurrentEnemy = EnemyDetector.GetNearest().GetComponent<CharacterData>();
			m_role.SetState(RoleControl.State.PURSUING);
			return;
		}
	}

	protected override void OnPursuingAI()
	{
		if (m_character.CurrentEnemy) LookAt(m_character.CurrentEnemy.transform);
		else m_role.SetState(RoleControl.State.IDLE);
	}
	protected override void OnMovingAI()
	{
		if (m_Destination != Vector3.zero && Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
		{
			GetComponent<EventSender>()?.Send(gameObject, "roleAIEvent_OnMoveEnd");
			m_role.SetState(RoleControl.State.IDLE);
			return;
		}
	}

	protected override void OnDeadAI()
	{
		m_role.SetState(RoleControl.State.DEAD);
		Agent.isStopped = true;
		Agent.enabled = false;
	}
}
