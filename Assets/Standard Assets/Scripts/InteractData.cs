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
		DropBox
	}
	public InteractType Type;
	public string Key;
	public string BehavePrompt;
	public string interactAnim;
	public int maxActorCount = 1;
	[ConditionalField(nameof(Type), false, InteractType.DropBox)]
	public int actTimes = 1;
	[ConditionalField(nameof(actTimes), true, 1)]
	public float actCd;
	public float BehaveDuring;
	[ConditionalField(nameof(Type), false, InteractType.DropBox)]
	public bool autoDestroy = true;
	public CollectionWrapper<DeviceItem> devices;
	public CollectionWrapper<DemandData> demands;

	public void InteractBehave(Transform trans, int index)
	{
		switch (Type)
		{
			case InteractType.DeviceFixer:
				DeviceItem device = devices.Value[index];
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
