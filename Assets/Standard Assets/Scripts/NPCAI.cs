using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class NPCAI : RoleAI
{
	public enum Camp
	{
		ENEMY,
		NEUTRAL
	}
	public Camp camp;
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
	public override void Init()
	{
		base.Init();
		switch (camp)
		{
			case NPCAI.Camp.ENEMY:
				m_role.gameObject.layer = LayerMask.NameToLayer("Enemy");
				m_role.EnemyDetector.layers = LayerMask.GetMask("Player");
				m_role.InteractDetector.layers = LayerMask.GetMask("Interactable");
				break;
			case NPCAI.Camp.NEUTRAL:
				m_role.gameObject.layer = LayerMask.NameToLayer("Neutral");
				m_role.EnemyDetector.layers = LayerMask.GetMask("Noting");
				m_role.InteractDetector.layers = LayerMask.GetMask("Interactable", "Player");
				m_Offensive = false;
				break;
			default:
				break;
		}
	}
	protected override void OnRoleEvent(GameObject obj, string eventName)
	{
		if (eventName == "roleEvent_OnIdling")
		{
			m_IdleDuring += Time.deltaTime;
			if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
			{
				Wandering();
				m_IdleDuring = 0;
			}
			if (!m_itemPick) PickItem = true;
		}
		if (eventName == "roleEvent_OnDamage")
		{
			if (!m_Offensive) m_Offensive = true;
			if (!m_role.CurrentEnemy) m_role.CurrentEnemy = m_role.EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
			if (m_role.EnemyDetector.Radius < 10) m_role.EnemyDetector.Radius = 10;
		}
		if (eventName == "roleEvent_OnPursuing" && m_Offensive)
		{
			m_role.Pursuing();
		}
		if (eventName == "roleEvent_OnState_DEAD")
		{
			bool corpse = (Random.Range(1, 101) <= CorpseChance);
			if (corpse)
			{
				Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Interactable"));
				StartCoroutine(DestroyCorpse(m_CorpseRetention));
			}
			else
			{
				if (dropEffect != null) dropEffect.Take(m_role.gameObject);
				Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
				StartCoroutine(DestroyCorpse(1.0f));
			}
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
		m_role.MoveTo(randomPos);
		m_role.eventSender.Send(m_role.gameObject, "aiEvent_wandering");
	}

	void OnItemEnter(Loot loot)
	{
		if (m_itemPick) loot.InteractWith(m_role.Data);
	}

}
