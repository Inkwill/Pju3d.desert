using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryEventData", menuName = "Data/StoryEventData", order = 15)]
public class StoryEventData : ScriptableObject
{
	public bool storyMode;
	public int triggerTimes = 1;
	public int loopTimes = 1;
	public float stayTime = 1;
	public float cd = 30;
	public List<KeyValueData.KeyValue<ConditionData, string[]>> Conditions;
	public List<KeyValueData.KeyValue<string, string>> Dialogues;
}
