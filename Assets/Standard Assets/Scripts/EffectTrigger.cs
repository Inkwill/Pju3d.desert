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
}
