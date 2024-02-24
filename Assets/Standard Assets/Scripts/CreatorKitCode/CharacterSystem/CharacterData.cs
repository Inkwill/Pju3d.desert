using System;
using System.Collections.Generic;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// This defines a character in the game. The name Character is used in a loose sense, it just means something that
/// can be attacked and have some stats including health. It could also be an inanimate object like a breakable box.
/// </summary>
public class CharacterData : HighlightableObject
{
	public enum Camp
	{
		PLAYER,
		ALLY,
		ENEMY,
		NEUTRAL
	}
	public Camp camp;
	public string CharacterName;
	public Weapon DefaultWeapon;
	public Transform WeaponLocator;
	public float MoveSpeed = 3.0f;
	public StatSystem Stats;
	public InventorySystem Inventory = new InventorySystem();
	public EquipmentSystem Equipment = new EquipmentSystem();
	public Action<Damage> OnDamage { get; set; }
	public UnityEvent<CharacterData> OnDeath;
	protected Vector3 m_BirthPos;
	public Vector3 BirthPos => m_BirthPos;
	public AIBase BaseAI => m_Ai;
	protected AIBase m_Ai;
	public SkillUser SkillUser => m_SkillUser;
	SkillUser m_SkillUser;
	float m_AttackCoolDown;

	void Awake()
	{
		InitHighlight();
		Stats.Init(this);
		m_BirthPos = transform.position;
		switch (camp)
		{
			case CharacterData.Camp.PLAYER:
				gameObject.layer = LayerMask.NameToLayer("Player");
				break;
			case CharacterData.Camp.ENEMY:
				gameObject.layer = LayerMask.NameToLayer("Enemy");
				break;
			case CharacterData.Camp.ALLY:
				gameObject.layer = LayerMask.NameToLayer("Player");
				break;
			case CharacterData.Camp.NEUTRAL:
				gameObject.layer = LayerMask.NameToLayer("Neutral");
				break;
			default:
				break;
		}
	}
	public void Active()
	{
		Inventory.Init(this);
		Equipment.Init(this);

		if (DefaultWeapon == null) DefaultWeapon = KeyValueData.GetValue<Item>(GameManager.Config.Item, "wp_unarmed") as Weapon;
		Equipment.InitWeapon(DefaultWeapon);

		m_SkillUser = GetComponent<SkillUser>();
		m_Ai = GetComponent<AIBase>();
		m_Ai.Init(this);

		OnDamage += (damage) =>
		{
			DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
		};

		Equipment.OnEquiped += item =>
		{
			if (item.Slot == EquipmentItem.EquipmentSlot.Weapon)
			{
				Weapon wp = item as Weapon;
				BaseAI.EnemyDetector.Radius = System.Math.Max(wp.Stats.MaxRange, BaseAI.EnemyDetector.Radius);
				wp.bulletTrans = WeaponLocator;
				if (WeaponLocator && item.WorldObjectPrefab)
					Instantiate(item.WorldObjectPrefab, WeaponLocator, false);

				//Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
			}
		};

		Equipment.OnUnequip += item =>
		{
			if (item.Slot == EquipmentItem.EquipmentSlot.Weapon)
			{
				foreach (Transform t in WeaponLocator)
					Destroy(t.gameObject);
			}
		};
	}

	void Update()
	{
		Stats.Tick();

		if (m_AttackCoolDown > 0.0f)
			m_AttackCoolDown -= Time.deltaTime;
	}

	public bool CanAttackReach(CharacterData target)
	{
		return Equipment.Weapon.CanHit(this, target);
	}

	public bool CanAttackTarget(CharacterData target)
	{
		if (target.Stats.CurrentHealth == 0)
			return false;

		if (!CanAttackReach(target))
			return false;

		if (m_AttackCoolDown > 0.0f)
			return false;

		return true;
	}

	public void Attack(CharacterData target)
	{
		if (Equipment.Weapon)
		{
			Equipment.Weapon.Attack(this, target);
			if (Equipment.Weapon.Stats.AdditionalTargets > 0)
			{
				List<GameObject> targets = BaseAI.EnemyDetector.Inners;
				int addNum = 0;
				foreach (GameObject t in targets)
				{
					if (t != target.gameObject && addNum < Equipment.Weapon.Stats.AdditionalTargets)
					{
						Equipment.Weapon.Attack(this, t.GetComponent<CharacterData>());
						addNum++;
					}
				}
			}
		}
		else Debug.LogError("Character missing weapon : " + gameObject);
	}

	public void AttackTriggered()
	{
		//Agility reduce by 0.5% the cooldown to attack (e.g. if agility = 50, 25% faster to attack)
		m_AttackCoolDown = Math.Max(0.1f, Equipment.Weapon.Stats.Speed - (Stats.stats.agility * 0.5f * 0.001f * Equipment.Weapon.Stats.Speed));
	}

	public void DestroyCharacter()
	{
		VFXManager.PlayVFX(VFXType.Death, transform.position);
		Destroy(gameObject);
	}
}
