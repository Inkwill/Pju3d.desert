using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
	[SerializeField]
	List<KeyValueData.KeyValue<EffectData, string[]>> Effects;

	public void TakEffects(GameObject user, GameObject target)
	{
		EffectData.TakeEffects(Effects, user, target);
	}

	public void TakeEffects(GameObject user)
	{
		EffectData.TakeEffects(Effects, user, user);
	}
	public void TakeEffects(CharacterData character)
	{
		TakeEffects(character.gameObject);
	}

	public void TakeEffects(Damage damage)
	{
		EffectData.TakeEffects(Effects, damage.Source.gameObject, damage.Target.gameObject);
	}
}
