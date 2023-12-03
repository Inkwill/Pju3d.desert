using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class AddHealthEffect : UsableItem.UsageEffect
{
	public int HealthAmount = 10;
	public override bool Use(CharacterData user)
    {
        user.Stats.ChangeHealth(HealthAmount);
		return false;
    }
}
