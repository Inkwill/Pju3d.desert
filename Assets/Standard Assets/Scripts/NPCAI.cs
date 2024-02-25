using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class NPCAI : RoleAI
{
	public EffectData dropEffect;
	public int CorpseChance;
	[SerializeField] float m_CorpseRetention;
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

	protected override void OnIdlingAI()
	{
		base.OnIdlingAI();
		m_IdleDuring += Time.deltaTime;
		if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
		{
			Agent.isStopped = false;
			Wandering();
			m_IdleDuring = 0;
		}
	}

	protected override void OnPursuingAI()
	{
		base.OnPursuingAI();
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
	protected override void OnDamageAI(Damage damage)
	{
		if (!m_Offensive) m_Offensive = true;
		if (!CurrentEnemy) m_character.SetEnemy(EnemyDetector.GetNearest()?.GetComponent<CharacterData>());
		if (EnemyDetector.Radius < 10) EnemyDetector.Radius = 10;
	}
	protected override void OnDeathAI()
	{
		base.OnDeathAI();
		bool corpse = (Random.Range(1, 101) <= CorpseChance);
		if (corpse)
		{
			m_character.gameObject.layer = LayerMask.NameToLayer("Interactable");
			StartCoroutine(DestroyCorpse(m_CorpseRetention));
		}
		else
		{
			if (dropEffect != null) dropEffect.TakeEffect(m_character.gameObject, m_character.gameObject);
			Helpers.RecursiveLayerChange(m_character.transform, LayerMask.NameToLayer("EnemyCorpse"));
			StartCoroutine(DestroyCorpse(1.0f));
		}
	}

	IEnumerator DestroyCorpse(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		m_character.DestroyCharacter();
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
