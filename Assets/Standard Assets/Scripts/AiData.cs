using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AiData", menuName = "Data/AiData", order = 600)]
public class AiData : ScriptableObject
{
	public bool Offensive;
	public bool guarder;
	public bool itemPick;
	public float WanderRadius;
	public float WanderBeat = 3.0f;

	public static AiData GetAiDataByKey(string key)
	{
		return KeyValueData.GetValue<AiData>(GameManager.Config.NpcAi, key);
	}
}
