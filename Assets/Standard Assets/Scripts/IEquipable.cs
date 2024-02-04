using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public interface IEquipable
{
    
    //StatSystem.StatModifier StatModifierEquip;
    public void OnEquip(CharacterData user);
    public void OnUnEquip(CharacterData user);
}
