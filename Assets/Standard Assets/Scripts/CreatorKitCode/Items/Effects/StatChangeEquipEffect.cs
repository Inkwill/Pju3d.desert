using System.Collections;
using System.Collections.Generic;
using System.Text;
using CreatorKitCode;
using UnityEngine;

public class StatChangeEquipEffect : Effect
{
    
    public StatSystem.StatModifier StatModifierEquip;
    public override void OnEquip(CharacterData user)
    {
        user.Stats.AddModifier(StatModifierEquip);
    }

    public override void OnUnEquip(CharacterData user)
    {
        user.Stats.RemoveModifier(StatModifierEquip);
    }

    public override string GetDescription()
    {
        string desc = base.GetDescription() + "\n";

        string unit = StatModifierEquip.ModifierMode == StatSystem.StatModifier.Mode.Percentage ? "%" : "";

        if (StatModifierEquip.Stats.strength != 0)
            desc += $"Str : {StatModifierEquip.Stats.strength:+0;-#}{unit}\n"; //format specifier to force the + sign to appear
        if (StatModifierEquip.Stats.agility != 0)
            desc += $"Agi : {StatModifierEquip.Stats.agility:+0;-#}{unit}\n";
        if (StatModifierEquip.Stats.defense != 0)
            desc += $"Def : {StatModifierEquip.Stats.defense:+0;-#}{unit}\n";
        if (StatModifierEquip.Stats.health != 0)
            desc += $"HP : {StatModifierEquip.Stats.health:+0;-#}{unit}\n";

        return desc;
    }
}
