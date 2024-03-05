using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class NPCAI : RoleAI
{
	[SerializeField] bool m_Offensive;
	[SerializeField] bool m_guarder;
	[SerializeField] bool m_itemPick;
	[SerializeField] float m_WanderRadius;
	[SerializeField] float m_WanderBeat = 3.0f;
	float m_IdleDuring;

	protected override void OnStateUpdate(State curState)
	{
		base.OnStateUpdate(curState);
		if (curState == State.IDLE)
		{
			if (m_guarder && Vector3.SqrMagnitude(m_character.BirthPos - transform.position) >= 1)
			{
				MoveTo(m_character.BirthPos);
				SetState(State.MOVE);
			}
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
	}

	void OnItemEnter(Loot loot)
	{
		if (m_itemPick) loot.InteractWith(m_character);
	}

}
