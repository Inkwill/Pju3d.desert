using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
public class Damage
{
	public CharacterData Target => m_Target;
	public CharacterData Source => m_Source;
	CharacterData m_Target;
	CharacterData m_Source;
	int[] m_Damages = new int[System.Enum.GetValues(typeof(StatSystem.DamageType)).Length];

	public Damage(CharacterData target, CharacterData sourcer = null)
	{
		m_Target = target;
		m_Source = sourcer;
	}

	public void Take(CharacterData target)
	{
		int totalDamage = GetFullDamage();
		target.Stats.ChangeHealth(-totalDamage);
		var damagePos = target.transform.position + target.transform.up * 0.5f;
		if (target.HitClip.Length != 0)
		{
			SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
			{
				Clip = target.HitClip[Random.Range(0, target.HitClip.Length)],
				PitchMax = 1.1f,
				PitchMin = 0.8f,
				Position = damagePos
			});
		}
		VFXManager.PlayVFX(VFXType.Hit, damagePos);
		//SFXManager.PlaySound(m_Source.AudioPlayer.UseType, new SFXManager.PlayData() { Clip = m_Source.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = damagePos });
		target.OnDamage?.Invoke(this);
	}

	public void Take() { Take(m_Target); }

	public int AddDamage(StatSystem.DamageType damageType, int amount)
	{
		int addedAmount = amount;
		//Physical damage are increase by 1% for each point of strength
		if (damageType == StatSystem.DamageType.Physical)
		{
			//source cna be null when it's elemental or effect damage
			if (m_Source != null)
				addedAmount += Mathf.FloorToInt(addedAmount * m_Source.Stats.stats.strength * 0.01f);

			//each poitn of defense remove 1 damage, with a minimum of 1 damage
			if (m_Target != null)
				addedAmount = Mathf.Max(addedAmount - m_Target.Stats.stats.defense, 1);
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

