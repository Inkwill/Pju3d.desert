using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class EffectData : ScriptableObject
{
	public string Description;
	public StatSystem.StatModifier Modifier;

	//return the amount of physical damage. If no change, just return physicalDamage passed as parameter
	public virtual void OnAttack(CharacterData target, CharacterData user, ref DamageEffect effect) { }

	//called after all weapon effect where applied, allow to react to the total amount of damage applied
	public virtual void OnPostAttack(CharacterData target, CharacterData user, DamageEffect effect) { }
	public virtual bool OnUse(CharacterData user) { return false; }
	public virtual void OnEquip(CharacterData user) { }
	public virtual void OnUnEquip(CharacterData user) { }
	public virtual string GetDescription()
	{
		return Description;
	}

	// public void Damage(Weapon.AttackData attackData)
	// {
	// 	if (HitClip.Length != 0)
	// 	{
	// 		SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
	// 		{
	// 			Clip = HitClip[Random.Range(0, HitClip.Length)],
	// 			PitchMax = 1.1f,
	// 			PitchMin = 0.8f,
	// 			Position = transform.position
	// 		});
	// 	}

	// 	Stats.Damage(attackData);

	// 	OnDamage?.Invoke();
	// }

}
