using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
public class Damage
{
	public Character Target => m_Target;
	public Character Source => m_Source;
	Character m_Target;
	Character m_Source;
	int[] m_Damages = new int[System.Enum.GetValues(typeof(StatSystem.DamageType)).Length];

	public Damage(Character target, Character sourcer = null)
	{
		m_Target = target;
		m_Source = sourcer;
	}

	public void TakeDamage(Character attacker, Character target)
	{
		int totalDamage = GetFullDamage();
		target.Stats.ChangeHealth(-totalDamage, attacker);
		var damagePos = target.transform.position + target.transform.up * 0.5f;
		VFXManager.PlayVFX(VFXType.Hit, damagePos);
		target.DamageEvent?.Invoke(this);
	}

	public void TakeDamage() { TakeDamage(m_Source, m_Target); }

	public int AddDamage(StatSystem.DamageType damageType, int amount)
	{
		int addedAmount = amount;
		//Physical damage are increase by 1% for each point of strength
		if (damageType == StatSystem.DamageType.Physical)
		{
			//source cna be null when it's elemental or effect damage
			if (m_Source != null)
				addedAmount += Mathf.FloorToInt(addedAmount * m_Source.Stats.stats.strength * 0.01f);

			//each poitn of defense remove 1 damage, with a minimum of 0 damage
			if (m_Target != null)
				addedAmount = Mathf.Max(addedAmount - m_Target.Stats.stats.defense, 0);
		}

		//we then add boost per damage type. Not this is called elementalBoost, but physical can also be boosted
		if (m_Source != null)
			addedAmount += addedAmount * Mathf.FloorToInt(m_Source.Stats.stats.damBoosts[(int)damageType] / 100.0f);

		//Then the elemental protection that is a percentage
		addedAmount -= addedAmount * Mathf.FloorToInt(m_Target.Stats.stats.damProtection[(int)damageType] / 100.0f);

		m_Damages[(int)damageType] += addedAmount;

		return addedAmount;
	}
	/// <summary>
	/// Return the current amount of damage of the given type stored in that AttackData. This is the *effective*
	/// amount of damage, boost and defense have already been applied.
	/// </summary>
	/// <param name="damageType">The type of damage</param>
	/// <returns>How much damage of that type is stored in that AttackData</returns>
	public int GetDamage(StatSystem.DamageType damageType)
	{
		return m_Damages[(int)damageType];
	}
	/// <summary>
	/// Return the total amount of damage across all type stored in that AttackData. This is *effective* damage,
	/// that mean all boost/defense was already applied.
	/// </summary>
	/// <returns>The total amount of damage across all type in that Attack Data</returns>
	public int GetFullDamage()
	{
		int totalDamage = 0;
		for (int i = 0; i < m_Damages.Length; ++i)
		{
			totalDamage += m_Damages[i];
		}
		return totalDamage;
	}
}

