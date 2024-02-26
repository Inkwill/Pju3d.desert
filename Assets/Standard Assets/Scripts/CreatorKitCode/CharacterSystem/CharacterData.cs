using System;
using System.Collections;
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
		NEUTRAL,
		BUILDING,
		ENEMYBUILDING
	}
	public Camp camp;
	public string CharacterName;
	public Weapon DefaultWeapon;
	public Transform WeaponLocator;
	public float MoveSpeed = 3.0f;
	public int CorpseChance;
	[SerializeField] float m_CorpseRetention;
	public StatSystem Stats;
	public InventorySystem Inventory = new InventorySystem();
	public EquipmentSystem Equipment = new EquipmentSystem();
	public Action<Damage> OnDamage { get; set; }
	public UnityEvent<CharacterData> OnDeath;
	public List<KeyValueData.KeyValue<EffectData, string[]>> dropEffects;
	public Action<CharacterData> OnAttack { get; set; }
	public Action<CharacterData> OnKillEnemy { get; set; }
	Vector3 m_BirthPos;
	public Vector3 BirthPos => m_BirthPos;
	CharacterData m_Enemy;
	public CharacterData CurrentEnemy { get { return m_Enemy; } }
	public AIBase BaseAI => m_Ai;
	AIBase m_Ai;
	float m_AttackCoolDown;
	StatisticsHandle m_recorder;

	void Awake()
	{
		InitHighlight();
		Stats.Init(this);
		Inventory.Init(this);
		Equipment.Init(this);
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
			case CharacterData.Camp.BUILDING:
				gameObject.layer = LayerMask.NameToLayer("Building");
				break;
			default:
				break;
		}
	}
	public void Active()
	{
		GetComponentInChildren<AnimationDispatcher>()?.AttackStep.AddListener(AttackFrame);

		if (DefaultWeapon == null) DefaultWeapon = KeyValueData.GetValue<Item>(GameManager.Config.Item, "wp_unarmed") as Weapon;
		Equipment.InitWeapon(DefaultWeapon);

		m_Ai = GetComponent<AIBase>();
		m_Ai.Init(this);

		m_recorder = GetComponent<StatisticsHandle>();
		m_recorder?.Init(this);

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

	public void SetEnemy(CharacterData enemy)
	{
		if (enemy != m_Enemy) m_Enemy = enemy;
	}

	public bool CanAttackReach(CharacterData target)
	{
		if (target == null) return false;
		return Equipment.Weapon.CanHit(this, target);
	}
	public bool CanAttackReach() { return CanAttackReach(m_Enemy); }
	public bool CanAttackTarget(CharacterData target)
	{
		if (target == null) return false;
		if (target.Stats.CurrentHealth == 0)
			return false;

		if (!CanAttackReach(target))
			return false;

		if (m_AttackCoolDown > 0.0f)
			return false;

		return true;
	}
	public bool CanAttackTarget() { return CanAttackTarget(m_Enemy); }
	public void CheckAttack()
	{
		if (CanAttackTarget())
		{
			BaseAI.Stop();
			BaseAI.LookAt(CurrentEnemy.transform);
			AttackTriggered();
			OnAttack?.Invoke(this);
		}
	}
	void AttackFrame()
	{
		//if we can't reach the target anymore when it's time to damage, then that attack miss.
		if (CurrentEnemy && CanAttackReach(CurrentEnemy))
		{
			Attack(CurrentEnemy);
		}
		else if (CurrentEnemy) Helpers.Log(this, "AttackMiss: ", $"{CharacterName}->{CurrentEnemy.CharacterName}");
		else Helpers.Log(this, "AttackMiss: ", $"{CharacterName}->(Enemy Missed)");
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

	public void Dead()
	{
		OnDeath?.Invoke(this);
		bool corpse = (Random.Range(1, 101) <= CorpseChance);
		if (corpse)
		{
			gameObject.layer = LayerMask.NameToLayer("Interactable");
			StartCoroutine(DestroyCorpse(m_CorpseRetention));
		}
		else
		{
			gameObject.layer = (camp == Camp.PLAYER) ? LayerMask.NameToLayer("PlayerCorpse") : LayerMask.NameToLayer("Corpse");
			if (dropEffects != null && dropEffects.Count > 0) EffectData.TakeEffects(dropEffects, gameObject, gameObject);
			StartCoroutine(DestroyCorpse(1.0f));
		}

	}
	IEnumerator DestroyCorpse(float time)
	{
		yield return new WaitForSeconds(time);
		VFXManager.PlayVFX(VFXType.Death, transform.position);
		Destroy(gameObject);
	}
}
