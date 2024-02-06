using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

public class VampiricWeaponEffect : EffectData
{
    public int PercentageHealthStolen;
    
    public override string GetDescription()
    {
        return $"Convert {PercentageHealthStolen}% of physical damage into Health";
    }

    public override void OnPostAttack(CharacterData target, CharacterData user, Effect effect)
    {
        int amount = Mathf.FloorToInt(effect.GetDamage(StatSystem.DamageType.Physical) * (PercentageHealthStolen / 100.0f));
        user.Stats.ChangeHealth(amount);
    }
}
