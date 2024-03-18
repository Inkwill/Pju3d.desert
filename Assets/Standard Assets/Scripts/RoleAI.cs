using System;
using UnityEngine;
using UnityEngine.AI;

public class RoleAI : AIBase
{
	public NavMeshAgent Agent => m_Agent;
	protected NavMeshAgent m_Agent;
	public Action MoveEndAction;
	public override float SpeedScale { get { return Agent.velocity.magnitude / m_character.MoveSpeed; } }
	protected Vector3 m_Destination = Vector3.zero;
	NavMeshPath m_CalculatedPath;


	protected override void OnStart()
	{
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
				SetState(State.MOVE);
			}
		}
	}
	public void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		m_Agent.SetDestination(pos);
		Agent.isStopped = false;
		SetState(State.MOVE);
	}
	public override void Stop()
	{
		Agent.velocity = Vector3.zero;
		Agent.isStopped = true;
		Agent.ResetPath();
	}

	protected override void OnStateUpdate(State curState)
	{
		if (curState == State.MOVE)
		{
			if (m_Destination != Vector3.zero && Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
			{
				MoveEndAction?.Invoke();
				if (m_character.CurrentEnemy) SetState(State.PURSUING);
				else SetState(State.IDLE);
				return;
			}
		}
	}

	protected override void OnDeathAI()
	{
		//Agent.isStopped = true;
		Agent.enabled = false;
	}
}
