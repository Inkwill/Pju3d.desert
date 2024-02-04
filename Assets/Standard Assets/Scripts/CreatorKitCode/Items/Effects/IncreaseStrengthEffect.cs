using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

public class IncreaseStrengthEffect : Effect
{
    public float Duration = 10.0f;
    public int StrengthChange = 5;
    public Sprite EffectSprite;
    
    public override bool OnUse(CharacterData user)
    {
        StatSystem.StatModifier modifier = new StatSystem.StatModifier();
        modifier.ModifierMode = StatSystem.StatModifier.Mode.Absolute;
        modifier.Stats.strength = StrengthChange;
        
        VFXManager.PlayVFX(VFXType.Stronger, user.transform.position);
        
        user.Stats.AddTimedModifier(modifier, Duration, "StrengthAdd", EffectSprite);
        
        return true;
    }
}
