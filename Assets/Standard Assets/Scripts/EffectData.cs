using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
public abstract class EffectData : ScriptableObject
{
	public string Description;

	public virtual bool OnUse(CharacterData user)
	{
		return false;
	}
	public virtual string GetDescription()
	{
		return Description;
	}


	[CreateAssetMenu(fileName = "DemoData", menuName = "Data/DamageEffect", order = 2)]
	public class DamageEffect : EffectData
	{
		public StatSystem.DamageType damageType;
		public int damageMount;
	}

	[CreateAssetMenu(fileName = "DemoData", menuName = "Data/EquipEffect", order = 3)]
	public class EquipEffect : EffectData
	{
		public StatSystem.StatModifier Modifier;
		public override string GetDescription()
		{
			string desc = Description + "\n";

			string unit = Modifier.ModifierMode == StatSystem.StatModifier.Mode.Percentage ? "%" : "";

			if (Modifier.Stats.strength != 0)
				desc += $"Str : {Modifier.Stats.strength:+0;-#}{unit}\n"; //format specifier to force the + sign to appear
			if (Modifier.Stats.agility != 0)
				desc += $"Agi : {Modifier.Stats.agility:+0;-#}{unit}\n";
			if (Modifier.Stats.defense != 0)
				desc += $"Def : {Modifier.Stats.defense:+0;-#}{unit}\n";
			if (Modifier.Stats.health != 0)
				desc += $"HP : {Modifier.Stats.health:+0;-#}{unit}\n";

			return desc;
		}
	}

	[CreateAssetMenu(fileName = "DemoData", menuName = "Data/HpEffect", order = 4)]
	public class HpEffect : EffectData
	{
		public int HealthAmount;
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
