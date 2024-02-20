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
	public override float SpeedScale { get { return Agent.velocity.magnitude / MoveSpeed; } }
	protected Vector3 m_Destination = Vector3.zero;
	NavMeshPath m_CalculatedPath;

	public override void Init()
	{
		base.Init();
		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.speed = MoveSpeed;
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
				m_role.CurState = RoleControl.State.MOVE;
			}
		}
	}

	public void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		m_Agent.SetDestination(pos);
		Agent.isStopped = false;
		m_role.CurState = RoleControl.State.MOVE;
	}
	public override void Stop()
	{
		Agent.velocity = Vector3.zero;
		Agent.isStopped = true;
		Agent.ResetPath();
	}
	protected override void OnMovingAI()
	{
		if (m_Destination != Vector3.zero && Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
		{
			GetComponent<EventSender>()?.Send(gameObject, "roleAIEvent_OnMoveEnd");
			m_role.CurState = RoleControl.State.IDLE;
			return;
		}
	}

	protected override void OnDeadAI()
	{
		Agent.isStopped = true;
		Agent.enabled = false;
	}
}
