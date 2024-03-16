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
	public string BehavePrompt;
	public string interactAnim;
	public int maxActorCount = 1;
	public int actTimes = 1;
	public float actCd;
	public float BehaveDuring;
	public bool autoDestroy;
	public DeviceItem[] devices;
	public DemandData[] demands;

	public void InteractBehave(Transform trans, int index)
	{
		switch (Type)
		{
			case InteractType.DeviceCreater:
				DeviceItem device = devices[index];
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
