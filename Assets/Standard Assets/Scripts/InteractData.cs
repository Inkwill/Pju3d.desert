using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData", order = 200)]
public class InteractData : ScriptableObject
{
	public enum InteractType
	{
		CharacterCreater,
		ItemCreater,
		DropBox
	}
	public InteractType Type;
	public string Key;
	public string interactAnim;
	public Sprite icon;
	public string Description;

	[ConditionalField(nameof(Type), false, InteractType.CharacterCreater)]
	public List<Character> characters;

	public static InteractData GetInteractDataByKey(string key)
	{
		return KeyValueData.GetValue<InteractData>(GameManager.Config.Interactor, key);
	}
}
