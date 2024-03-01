using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class NPCAI : RoleAI
{
	[SerializeField] bool m_Offensive;
	[SerializeField] float m_WanderRadius;
	[SerializeField] float m_WanderBeat = 3.0f;
	float m_IdleDuring;

	public bool PickItem
	{
		get { return m_itemPick; }
		set
		{
			m_itemPick = value;
		}
	}
	bool m_itemPick;

	protected override void OnStateUpdate(State curState)
	{
		base.OnStateUpdate(curState);
		if (curState == State.IDLE)
		{
			m_IdleDuring += Time.deltaTime;
			if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
			{
				Agent.isStopped = false;
				Wandering();
				m_IdleDuring = 0;
			}
		}
		if (curState == State.PURSUING)
		{
			if (CurrentEnemy && m_Offensive && m_character.MoveSpeed > 0)
			{
				Agent.isStopped = false;
				Agent.SetDestination(CurrentEnemy.gameObject.transform.position);
			}
			else
			{
				Helpers.Log(this, "OnPursuingAI", "enemy= " + CurrentEnemy);
			}
		}
	}
	protected override void OnDamageAI(Damage damage)
	{
		if (!m_Offensive) m_Offensive = true;
		if (!CurrentEnemy) m_character.SetEnemy(EnemyDetector.GetNearest()?.GetComponent<CharacterData>());
		if (EnemyDetector.Radius < 10) EnemyDetector.Radius = 10;
	}

	void Wandering()
	{
		float randomX = Random.Range(0f, m_WanderRadius);
		float randomZ = Random.Range(0f, m_WanderRadius);

		Vector3 randomPos = new Vector3(m_character.BirthPos.x + randomX, m_character.BirthPos.y, m_character.BirthPos.z + randomZ);
		MoveTo(randomPos);
		m_character.GetComponent<EventSender>()?.Send(m_character.gameObject, "aiEvent_wandering");
	}

	void OnItemEnter(Loot loot)
	{
		if (m_itemPick) loot.InteractWith(m_character);
	}

}
