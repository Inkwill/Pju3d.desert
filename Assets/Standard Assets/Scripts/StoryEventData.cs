using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "StoryEventData", menuName = "Data/StoryEventData", order = 15)]
public class StoryEventData : ScriptableObject
{
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
	public float areaRadius = 1;

	[ConditionalField(nameof(storyType), false, StoryType.Dialogue)]
	public List<KeyValueData.KeyValue<string, string>> Dialogues;
	public List<KeyValueData.KeyValue<ConditionData, string[]>> Conditions;
	public List<KeyValueData.KeyValue<EffectData, string[]>> Effects;

	public StoryEvent Instantiate(GameObject target)
	{
		StoryEvent storyEvent = target.AddComponent<StoryEvent>();
		storyEvent.Init(this);
		return storyEvent;
	}
}
