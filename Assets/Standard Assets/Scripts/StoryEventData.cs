using MyBox;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "StoryEventData", menuName = "Data/StoryEventData", order = 15)]
public class StoryEventData : ScriptableObject
{
	public string storyEventId;
	public enum StoryType
	{
		Dialogue,
		TellingPanel
	}
	public StoryType storyType;
	public enum TriggerType
	{
		AutoStart,
		EnterArea,
		StayArea,
		Damaged
	}
	public TriggerType triggerType;
	public bool storyMode;
	[ConditionalField(nameof(triggerType), true, TriggerType.AutoStart)]
	public int triggerTimes = 1;
	[ConditionalField(nameof(triggerType), true, TriggerType.AutoStart)]
	public int loopTimes = 1;
	[ConditionalField(nameof(loopTimes), true, 1)]
	public float cd = 30;
	[ConditionalField(nameof(triggerType), false, TriggerType.StayArea)]
	public float stayTime = 1;
	[ConditionalField(nameof(triggerType), false, TriggerType.EnterArea, TriggerType.StayArea)]
	public Vector3 areaPostion;
	[ConditionalField(nameof(triggerType), false, TriggerType.EnterArea, TriggerType.StayArea)]
	public float areaRadius = 1;

	[ConditionalField(nameof(storyType), false, StoryType.Dialogue)]
	public List<KeyValueData.KeyValue<string, string>> Dialogues;
	public List<KeyValueData.KeyValue<ConditionData, string[]>> Conditions;
	public List<KeyValueData.KeyValue<EffectData, string[]>> Effects;

	public StoryEvent Instantiate(GameObject owner)
	{
		if (triggerType == TriggerType.AutoStart || triggerType == TriggerType.Damaged)
		{
			StoryEvent storyEvent = owner.AddComponent<StoryEvent>();
			storyEvent.Init(this);
			return storyEvent;
		}
		else if (triggerType == TriggerType.EnterArea || triggerType == TriggerType.StayArea)
		{
			StoryEvent storyEvent = GameObject.Instantiate(new GameObject()).AddComponent<StoryEvent>();
			storyEvent.transform.position = areaPostion;
			storyEvent.name = storyEventId;
			storyEvent.Init(this);
			return storyEvent;
		}
		return null;
	}
}
