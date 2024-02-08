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
		HPCHANGE,
		SPEEDUP,
		DIG,
		SUMMON
	}
	public EffectType Type;
	public string Description;
	[ConditionalField(nameof(Type), false, EffectType.DAMAGE)]
	public StatSystem.DamageType damageType;
	[ConditionalField(nameof(Type), false, EffectType.DAMAGE)]
	public int damageMount;
	[ConditionalField(nameof(Type), false, EffectType.EQUIP)]
	public StatSystem.StatModifier StatModifier;
	public int EffectAmount;
	public StatSystem.StatModifier.Mode EffectMode;

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

	public bool Take(CharacterData user, string[] param = null, GameObject target = null)
	{
		switch (Type)
		{
			case EffectType.DAMAGE:
				return true;
			case EffectType.HPCHANGE:
				CharacterData changer = target ? target.GetComponent<CharacterData>() : user;
				if (changer == null) return false;
				int addMount;
				if (param != null && param.Length > 0 && int.TryParse(param[0], out addMount))
					return ChangeHealth(changer, EffectAmount + addMount);
				else return ChangeHealth(changer, EffectAmount);
			case EffectType.DIG:
				if (target != null)
				{
					int deep = 1;
					if (param != null && param.Length > 0 && int.TryParse(param[0], out deep))
					{
						GameManager.Instance.TerrainTool.LowerTerrain(target.transform.position, deep * 0.00025f, 5, 10);
						return true;
					}
				}
				return false;
			case EffectType.SUMMON:
				Vector3 pos = (target != null) ? target.transform.position : user.transform.position;
				if (param != null && param.Length > 0)
				{
					GameObject pbObj = Resources.Load(param[0]) as GameObject;
					if (pbObj != null) { Instantiate(pbObj, pos, Quaternion.Euler(0, 180, 0)); return true; }
					else return false;
				}
				return false;
			default:
				Debug.LogError("Take a illegal Effect:" + this);
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

	bool ChangeHealth(CharacterData user, int value)
	{
		if (EffectMode == StatSystem.StatModifier.Mode.Absolute)
		{
			user.Stats.ChangeHealth(value);
			return true;
		}
		else if (EffectMode == StatSystem.StatModifier.Mode.Percentage)
		{
			if (user.Stats.CurrentHealth == user.Stats.stats.health && value > 0)
				return true;
			user.Stats.ChangeHealth(Mathf.FloorToInt(value / 100.0f * user.Stats.stats.health));
			return true;
		}
		return false;
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

	// public class AddHealthUsageEffect : EffectData
	// {
	// 	//[FormerlySerializedAs("HealthPurcentageAmount")]
	// 	public int HealthPercentageAmount = 20;

	// 	public bool OnUse(CharacterData user)
	// 	{
	// 		if (user.Stats.CurrentHealth == user.Stats.stats.health)
	// 			return false;

	// 		VFXManager.PlayVFX(VFXType.Healing, user.transform.position);

	// 		user.Stats.ChangeHealth(Mathf.FloorToInt(HealthPercentageAmount / 100.0f * user.Stats.stats.health));

	// 		return true;
	// 	}
	// }

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
