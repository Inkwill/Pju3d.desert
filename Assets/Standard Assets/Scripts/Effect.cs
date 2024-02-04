using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public abstract class Effect : ScriptableObject
{
	public string Description;
	public StatSystem.StatModifier Modifier;

	//return the amount of physical damage. If no change, just return physicalDamage passed as parameter
	public virtual void OnAttack(CharacterData target, CharacterData user, ref Weapon.AttackData data) { }

	//called after all weapon effect where applied, allow to react to the total amount of damage applied
	public virtual void OnPostAttack(CharacterData target, CharacterData user, Weapon.AttackData data) { }
	public virtual bool OnUse(CharacterData user) { return false; }
	public virtual void OnEquip(CharacterData user) { }
	public virtual void OnUnEquip(CharacterData user) { }
	public virtual string GetDescription()
	{
		return Description;
	}
}
