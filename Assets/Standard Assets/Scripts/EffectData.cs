using Random = UnityEngine.Random;
using UnityEngine;
using CreatorKitCode;
using MyBox;

[CreateAssetMenu(fileName = "EffectData", menuName = "Data/EffectData", order = 1)]
public class EffectData : ScriptableObject
{
	public enum EffectType
	{
		DAMAGE,
		HPCHANGE,
		SPEEDUP,
		DIG,
		SUMMON,
		DROPBOX,
		ADDGOAL
	}
	public EffectType Type;
	public VFXType TakeVFX = VFXType.NONE;
	public string Description;

	[ConditionalField(nameof(Type), false, EffectType.HPCHANGE)]
	public int EffectAmount;

	[ConditionalField(nameof(Type), false, EffectType.HPCHANGE)]
	public StatSystem.StatModifier.Mode EffectMode;
	[ConditionalField(nameof(Type), false, EffectType.DROPBOX)]
	public DropBox dropBox;

	[ConditionalField(nameof(Type), false, EffectType.DAMAGE)]
	public StatSystem.DamageType damageType;


	public virtual string GetDescription()
	{
		return Description;
	}

	public bool Take(GameObject user, string[] param = null, GameObject target = null)
	{
		bool success = false;
		switch (Type)
		{
			case EffectType.DAMAGE:
				break;
			case EffectType.HPCHANGE:
				CharacterData changer = target ? target.GetComponent<CharacterData>() : user.GetComponent<CharacterData>();
				if (changer == null) break;
				int addMount;
				if (param != null && param.Length > 0 && int.TryParse(param[0], out addMount))
					success = ChangeHealth(changer, EffectAmount + addMount);
				else success = ChangeHealth(changer, EffectAmount);
				break;
			case EffectType.DIG:
				if (target != null)
				{
					int deep = 1;
					if (param != null && param.Length > 0 && int.TryParse(param[0], out deep))
					{
						GameManager.Instance.TerrainTool.LowerTerrain(target.transform.position, deep * 0.00025f, 5, 10);
						success = true;
					}
				}
				break;
			case EffectType.SUMMON:
				Vector3 pos = (target != null) ? target.transform.position : user.transform.position;
				if (param != null && param.Length > 0)
				{
					GameObject pbObj = Resources.Load(param[0]) as GameObject;
					if (pbObj != null) { Instantiate(pbObj, pos, Quaternion.Euler(0, 180, 0)); success = true; }
				}
				break;
			case EffectType.DROPBOX:
				foreach (var drop in dropBox.GetDropItem())
				{
					DropItem(user.transform.position, drop.item, drop.itemNum);
					success = true;
					Helpers.Log(this, "Drop", $"{drop.item.ItemName}x{drop.itemNum}");
				}
				break;
			case EffectType.ADDGOAL:
				if (param != null && param.Length > 0)
					GameManager.GameGoal.AddGoal(GoalData.GetDataByKey(param[0]));
				break;
			default:
				Debug.LogError("Take a illegal Effect:" + this);
				return false;
		}

		if (TakeVFX != VFXType.NONE && success) VFXManager.PlayVFX(TakeVFX, user.transform.position);
		return success;
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

	void DropItem(Vector3 pos, Item item, int num)
	{
		GameObject lootObj = Resources.Load("Loot") as GameObject;
		Vector3 direction = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.right;
		Vector3 spawnPosition = pos + direction * Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			Loot loot = Instantiate(lootObj, spawnPosition, Quaternion.Euler(0, 0, 0)).GetComponent<Loot>();
			loot.Item = item;
		}
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
