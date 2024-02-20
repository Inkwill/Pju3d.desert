using System;
using System.Collections.Generic;
using CreatorKitCodeInternal;
using UnityEngine;

using Random = UnityEngine.Random;

namespace CreatorKitCode
{
	/// <summary>
	/// This defines a character in the game. The name Character is used in a loose sense, it just means something that
	/// can be attacked and have some stats including health. It could also be an inanimate object like a breakable box.
	/// </summary>
	public class CharacterData : HighlightableObject
	{
		public string CharacterName;
		public Weapon DefaultWeapon;

		public StatSystem Stats;
		/// <summary>
		/// The starting weapon equipped when the Character is created. Set through the Unity Editor.
		/// </summary>
		public InventorySystem Inventory = new InventorySystem();
		public EquipmentSystem Equipment = new EquipmentSystem();

		public AudioClip[] HitClip;
		// public CharacterAudio AudioPlayer => m_CharacterAudio;
		// protected CharacterAudio m_CharacterAudio;

		/// <summary>
		/// Callback for when that CharacterData receive damage. E.g. used by the player character to trigger the right
		/// animation
		/// </summary>
		public Action<Damage> OnDamage { get; set; }

		/// <summary>
		/// Will return true if the attack cooldown have reached 0. False otherwise.
		/// </summary>
		public bool CanAttack
		{
			get { return m_AttackCoolDown <= 0.0f; }
		}

		float m_AttackCoolDown;

		public void Init()
		{
			Stats.Init(this);
			Inventory.Init(this);
			Equipment.Init(this);
			if (DefaultWeapon == null) DefaultWeapon = KeyValueData.GetValue<Item>(GameManager.Config.Item, "wp_unarmed") as Weapon;
			Equipment.InitWeapon(DefaultWeapon);
		}

		void Awake()
		{
			// Animator anim = GetComponentInChildren<Animator>();
			// if (anim != null)
			// 	SceneLinkedSMB<CharacterData>.Initialise(anim, this);
			//m_CharacterAudio = GetComponentInChildren<CharacterAudio>();
		}

		// Update is called once per frame
		void Update()
		{
			Stats.Tick();

			if (m_AttackCoolDown > 0.0f)
				m_AttackCoolDown -= Time.deltaTime;
		}

		/// <summary>
		/// Will check if that CharacterData can reach the given target with its currently equipped weapon. Will rarely
		/// be called, as the function CanAttackTarget will call this AND also check if the cooldown is finished.
		/// </summary>
		/// <param name="target">The CharacterData you want to reach</param>
		/// <returns>True if you can reach the target, False otherwise</returns>
		public bool CanAttackReach(CharacterData target)
		{
			return Equipment.Weapon.CanHit(this, target);
		}

		/// <summary>
		/// Will check if the target is attackable. This in effect check :
		/// - If the target is in range of the weapon
		/// - If this character attack cooldown is finished
		/// - If the target isn't already dead
		/// </summary>
		/// <param name="target">The CharacterData you want to reach</param>
		/// <returns>True if the target can be attacked, false if any of the condition isn't met</returns>
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

		/// <summary>
		/// Call when the character die (health reach 0).
		/// </summary>
		public void Death()
		{
			Stats.Death();
		}

		/// <summary>
		/// Attack the given target. NOTE : this WON'T check if the target CAN be attacked, you should make sure before
		/// with the CanAttackTarget function.
		/// </summary>
		/// <param name="target">The CharacterData you want to attack</param>
		public void Attack(CharacterData target)
		{
			if (Equipment.Weapon)
			{
				Equipment.Weapon.Attack(this, target);
				if (Equipment.Weapon.Stats.AdditionalTargets > 0)
				{
					List<GameObject> targets = GetComponent<RoleControl>()?.BaseAI.EnemyDetector.Inners;
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

		/// <summary>
		/// This need to be called as soon as an attack is triggered, it will start the cooldown. This is separate from
		/// the actual Attack function as AttackTriggered will be called at the beginning of the animation while the
		/// Attack function (doing the actual attack and damage) will be called by an animation event to match the animation
		/// </summary>
		public void AttackTriggered()
		{
			//Agility reduce by 0.5% the cooldown to attack (e.g. if agility = 50, 25% faster to attack)
			m_AttackCoolDown = Math.Max(0.1f, Equipment.Weapon.Stats.Speed - (Stats.stats.agility * 0.5f * 0.001f * Equipment.Weapon.Stats.Speed));
		}
	}
}