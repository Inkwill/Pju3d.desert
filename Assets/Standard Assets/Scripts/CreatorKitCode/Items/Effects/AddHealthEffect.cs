using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class AddHealthEffect : EffectData
{
	public int HealthAmount = 10;
	public override bool OnUse(CharacterData user)
	{
		user.Stats.ChangeHealth(HealthAmount);
		return true;
	}
}
