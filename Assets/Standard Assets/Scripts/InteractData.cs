using System.Collections.Generic;
using MyBox;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData", order = 200)]
public class InteractData : ScriptableObject
{
	public enum InteractType
	{
		DeviceFixer,
		DeviceCreater,
		ResourceOutputer,
		DropBox
	}
	public InteractType Type;
	public string Key;
	public string BehavePrompt;
	public string interactAnim;
	public int maxActorCount = 1;
	[ConditionalField(nameof(Type), false, InteractType.ResourceOutputer, InteractType.DropBox)]
	public int maxActTimes = 1;
	[ConditionalField(nameof(maxActTimes), true, 1)]
	public float actCd;
	[ConditionalField(nameof(interactAnim), false, "")]
	public float BehaveDuring;
	[ConditionalField(nameof(Type), false, InteractType.DropBox)]
	public bool autoDestroy = true;
	[ConditionalField(nameof(Type), false, InteractType.DeviceFixer, InteractType.DeviceCreater)]
	public CollectionWrapper<DeviceItem> devices;
	[ConditionalField(nameof(Type), false, InteractType.DeviceFixer, InteractType.DeviceCreater)]
	public CollectionWrapper<DemandData> demands;
	[ConditionalField(nameof(Type), false, InteractType.ResourceOutputer)]
	public ResItem resItem;

	public void InteractBehave(Transform trans, int index = 0)
	{
		switch (Type)
		{
			case InteractType.DeviceFixer:
			case InteractType.DeviceCreater:
				DeviceItem device = devices.Value[index];
				if (device != null && device.prefab != null)
					Instantiate(device.prefab, trans.position, trans.rotation);
				break;
			case InteractType.ResourceOutputer:
				if (resItem != null)
				{
					EffectData.DropItem(trans.position, resItem, 1);
				}
				break;
			default:
				break;
		}
	}

	public static InteractData GetInteractDataByKey(string key)
	{
		return KeyValueData.GetValue<InteractData>(GameManager.Config.Interactor, key);
	}
}
