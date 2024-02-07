using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using MyBox;

[CreateAssetMenu(fileName = "EffectData", menuName = "Data/EffectData", order = 1)]
public class EffectData : ScriptableObject
{
	public enum EffectType
	{
		DAMAGE,
		EQUIP,
		HPCHANGE
	}
	public EffectType Type;
	public string Description;
	[ConditionalField(nameof(Type), false, EffectType.DAMAGE)]
	public StatSystem.DamageType damageType;
	[ConditionalField(nameof(Type), false, EffectType.DAMAGE)]
	public int damageMount;
	[ConditionalField(nameof(Type), false, EffectType.EQUIP)]
	public StatSystem.StatModifier StatModifier;
	[ConditionalField(nameof(Type), false, EffectType.HPCHANGE)]
	public int HealthAmount;

	public virtual string GetDescription()
	{
		switch (Type)
		{
			case EffectType.EQUIP:
				string desc = Description + "\n";

				string unit = StatModifier.ModifierMode == StatSystem.StatModifier.Mode.Percentage ? "%" : "";

				if (StatModifier.Stats.strength != 0)
					desc += $"Str : {StatModifier.Stats.strength:+0;-#}{unit}\n"; //format specifier to force the + sign to appear
				if (StatModifier.Stats.agility != 0)
					desc += $"Agi : {StatModifier.Stats.agility:+0;-#}{unit}\n";
				if (StatModifier.Stats.defense != 0)
					desc += $"Def : {StatModifier.Stats.defense:+0;-#}{unit}\n";
				if (StatModifier.Stats.health != 0)
					desc += $"HP : {StatModifier.Stats.health:+0;-#}{unit}\n";

				return desc;
			default:
				return Description;
		}
	}

	public bool Take(CharacterData user, string[] para, CharacterData target = null)
	{
		switch (Type)
		{
			case EffectType.DAMAGE:
				return true;
			case EffectType.HPCHANGE:
				int mount = para.Length > 0 ? int.Parse(para[0]) : 0;
				user.Stats.ChangeHealth(mount);
				return true;
			default:
				return false;
		}
	}

	public void Equip(CharacterData user)
	{
		if (Type != EffectType.EQUIP) return;
		if (StatModifier != null) user.Stats.AddModifier(StatModifier);
	}

	public void UnEquip(CharacterData user)
	{
		if (Type != EffectType.EQUIP) return;
		if (StatModifier != null) user.Stats.RemoveModifier(StatModifier);
	}

	public class VampiricWeaponEffect : EffectData
	{
		public int PercentageHealthStolen;

		public override string GetDescription()
		{
			return $"Convert {PercentageHealthStolen}% of physical damage into Health";
		}

		public void OnPostAttack(CharacterData target, CharacterData user, Damage damage)
		{
			int amount = Mathf.FloorToInt(damage.GetDamage(StatSystem.DamageType.Physical) * (PercentageHealthStolen / 100.0f));
			user.Stats.ChangeHealth(amount);
		}
	}

	public class AddHealthUsageEffect : EffectData
	{
		//[FormerlySerializedAs("HealthPurcentageAmount")]
		public int HealthPercentageAmount = 20;

		public bool OnUse(CharacterData user)
		{
			if (user.Stats.CurrentHealth == user.Stats.stats.health)
				return false;

			VFXManager.PlayVFX(VFXType.Healing, user.transform.position);

			user.Stats.ChangeHealth(Mathf.FloorToInt(HealthPercentageAmount / 100.0f * user.Stats.stats.health));

			return true;
		}
	}

	public class IncreaseStrengthEffect : EffectData
	{
		public float Duration = 10.0f;
		public int StrengthChange = 5;
		public Sprite EffectSprite;

		public bool OnUse(CharacterData user)
		{
			StatSystem.StatModifier modifier = new StatSystem.StatModifier();
			modifier.ModifierMode = StatSystem.StatModifier.Mode.Absolute;
			modifier.Stats.strength = StrengthChange;

			VFXManager.PlayVFX(VFXType.Stronger, user.transform.position);

			user.Stats.AddTimedModifier(modifier, Duration, "StrengthAdd", EffectSprite);

			return true;
		}
	}

	public class ApplyBurnWeaponEffect : EffectData
	{
		public float PercentageChance;
		public int Damage;
		public float Time;

		public void OnAttack(CharacterData target, CharacterData user, ref Damage damage)
		{
			if (UnityEngine.Random.value < (PercentageChance / 100.0f))
			{
				ElementalEffect effect = new ElementalEffect(Time, this, 1.0f);

				target.Stats.AddElementalEffect(effect);
			}
		}
	}
}
