using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class NPCAI : RoleAI
{
	float m_IdleDuring;

	protected override void OnStateUpdate(State curState)
	{
		base.OnStateUpdate(curState);
		if (curState == State.IDLE)
		{
			if (m_data.guarder && Vector3.SqrMagnitude(m_character.BirthPos - transform.position) >= 1)
			{
				MoveTo(m_character.BirthPos);
				SetState(State.MOVE);
			}
			m_IdleDuring += Time.deltaTime;
			if (m_IdleDuring > m_data.WanderBeat && m_data.WanderRadius > 0)
			{
				Agent.isStopped = false;
				Wandering();
				m_IdleDuring = 0;
			}
		}
		if (curState == State.PURSUING)
		{
			if (CurrentEnemy && m_data.Offensive && m_character.MoveSpeed > 0)
			{
				Agent.isStopped = false;
				Agent.SetDestination(CurrentEnemy.gameObject.transform.position);
			}
		}
	}
	protected override void OnDamageAI(Damage damage)
	{
		if (!m_data.Offensive) m_data.Offensive = true;
		if (!CurrentEnemy) m_character.SetEnemy(EnemyDetector.GetNearest()?.GetComponent<Character>());
		if (EnemyDetector.Radius < 10) EnemyDetector.Radius = 10;
	}

	void Wandering()
	{
		float randomX = Random.Range(0f, m_data.WanderRadius);
		float randomZ = Random.Range(0f, m_data.WanderRadius);

		Vector3 randomPos = new Vector3(m_character.BirthPos.x + randomX, m_character.BirthPos.y, m_character.BirthPos.z + randomZ);
		MoveTo(randomPos);
	}

	void OnItemEnter(Loot loot)
	{
		if (m_data.itemPick) loot.InteractWith(m_character);
	}

}
