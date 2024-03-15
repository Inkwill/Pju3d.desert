using System.Collections.Generic;
using MyBox;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "InteractData", menuName = "Data/InteractData", order = 200)]
public class InteractData : ScriptableObject
{
	public enum InteractType
	{
		DeviceCreater,
		DropBox
	}
	public InteractType Type;
	public string Key;
	public string interactAnim;
	public int maxActorCount = 1;

	[ConditionalField(nameof(Type), false, InteractType.DeviceCreater)]
	public List<DeviceItem> devices;
	[ConditionalField(nameof(Type), false, InteractType.DeviceCreater)]
	public List<DemandData> demands;

	public void InteractBehave(Transform trans, Item item)
	{
		switch (Type)
		{
			case InteractType.DeviceCreater:
				DeviceItem device = item as DeviceItem;
				if (device != null && device.prefab != null)
					Instantiate(device.prefab, trans.position, trans.rotation);
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
