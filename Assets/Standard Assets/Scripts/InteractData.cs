using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData", order = 200)]
public class InteractData : ScriptableObject
{
	public string Key;
	public string interactAnim;
	public Sprite icon;
	public string Description;

	public static InteractData GetInteractDataByKey(string key)
	{
		return KeyValueData.GetValue<InteractData>(GameManager.Config.Interactor, key);
	}
}
