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
		ADDGOAL,
		LORDMONEY,
		LORDEXP
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

	public bool TakeEffect(GameObject user, GameObject target, string[] param = null)
	{
		bool success = false;
		switch (Type)
		{
			case EffectType.DAMAGE:
				break;
			case EffectType.HPCHANGE:
				int addMount = 0;
				if (param != null && param.Length > 0 && int.TryParse(param[0], out addMount))
					success = ChangeHealth(user.GetComponent<CharacterData>(), target.GetComponent<CharacterData>(), EffectAmount + addMount);
				else success = ChangeHealth(user.GetComponent<CharacterData>(), target.GetComponent<CharacterData>(), EffectAmount);
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
				Transform trans = (target != null) ? target.transform : user.transform;
				if (param != null && param.Length > 0)
				{
					GameObject pbObj = Resources.Load(param[0]) as GameObject;
					if (pbObj != null) { Instantiate(pbObj, trans.position, trans.rotation); success = true; }
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
				{
					GameManager.GameGoal.AddGoal(GoalData.GetDataByKey(param[0]));
					success = true;
				}
				break;
			case EffectType.LORDMONEY:
				int money = 0;
				if (param != null && param.Length > 0 && int.TryParse(param[0], out money))
				{
					GameManager.Lord.AddMoney(money);
					success = true;
				}
				break;
			case EffectType.LORDEXP:
				int exp = 0;
				if (param != null && param.Length > 0 && int.TryParse(param[0], out exp))
				{
					GameManager.Lord.AddExp(exp);
					success = true;
				}
				break;
			default:
				Debug.LogError("Take a illegal Effect:" + this);
				return false;
		}

		if (TakeVFX != VFXType.NONE && success) VFXManager.PlayVFX(TakeVFX, user.transform.position);
		return success;
	}

	bool ChangeHealth(CharacterData attacker, CharacterData target, int value)
	{
		if (EffectMode == StatSystem.StatModifier.Mode.Absolute)
		{
			target.Stats.ChangeHealth(value, attacker);
			return true;
		}
		else if (EffectMode == StatSystem.StatModifier.Mode.Percentage)
		{
			if (target.Stats.CurrentHealth == target.Stats.stats.health && value > 0)
				return true;
			target.Stats.ChangeHealth(Mathf.FloorToInt(value / 100.0f * target.Stats.stats.health), attacker);
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

		public void OnPostAttack(CharacterData target, CharacterData attacker, Damage damage)
		{
			int amount = Mathf.FloorToInt(damage.GetDamage(StatSystem.DamageType.Physical) * (PercentageHealthStolen / 100.0f));
			target.Stats.ChangeHealth(amount, attacker);
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
