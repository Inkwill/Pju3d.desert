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
		m_IdleDuring += Time.deltaTime;
		if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
		{
			Wandering();
			m_IdleDuring = 0;
		}
	}

	protected override void OnPursuingAI()
	{
		if (m_role.CurrentEnemy && m_Offensive && MoveSpeed > 0)
			Agent.SetDestination(m_role.CurrentEnemy.gameObject.transform.position);
		else
		{
			Helpers.Log(this, "OnPursuingAI", "enemy= " + m_role.CurrentEnemy);
		}
	}
	protected override void OnDamageAI()
	{
		if (!m_Offensive) m_Offensive = true;
		if (!m_role.CurrentEnemy) m_role.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
		if (EnemyDetector.Radius < 10) EnemyDetector.Radius = 10;
	}
	protected override void OnDeadAI()
	{
		base.OnDeadAI();
		bool corpse = (Random.Range(1, 101) <= CorpseChance);
		if (corpse)
		{
			m_role.gameObject.layer = LayerMask.NameToLayer("Interactable");
			StartCoroutine(DestroyCorpse(m_CorpseRetention));
		}
		else
		{
			if (dropEffect != null) dropEffect.Take(m_role.gameObject);
			Helpers.RecursiveLayerChange(m_role.transform, LayerMask.NameToLayer("EnemyCorpse"));
			StartCoroutine(DestroyCorpse(1.0f));
		}
	}

	IEnumerator DestroyCorpse(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		VFXManager.PlayVFX(VFXType.Death, transform.position);
		Destroy(gameObject);
	}
	void Wandering()
	{
		float randomX = Random.Range(0f, m_WanderRadius);
		float randomZ = Random.Range(0f, m_WanderRadius);

		Vector3 randomPos = new Vector3(m_role.BirthPos.x + randomX, m_role.BirthPos.y, m_role.BirthPos.z + randomZ);
		MoveTo(randomPos);
		m_role.eventSender.Send(m_role.gameObject, "aiEvent_wandering");
	}

	void OnItemEnter(Loot loot)
	{
		if (m_itemPick) loot.InteractWith(m_role.Data);
	}

}
