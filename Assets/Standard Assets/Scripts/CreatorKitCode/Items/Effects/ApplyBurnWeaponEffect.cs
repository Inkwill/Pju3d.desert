﻿using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

public class ApplyBurnWeaponEffect : EffectData
{
    public float PercentageChance;
    public int Damage;
    public float Time;
    
    public override void OnAttack(CharacterData target, CharacterData user, ref Effect attackEffect)
    {
        if (Random.value < (PercentageChance / 100.0f))
        {
            ElementalEffect effect = new ElementalEffect(Time, StatSystem.DamageType.Fire, Damage, 1.0f);

            target.Stats.AddElementalEffect(effect);
        }
    }
}
