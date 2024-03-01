using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGoalData", menuName = "Data/GoalData", order = 15)]
public class GoalData : ScriptableObject
{
	public string goalId;
	public string describe;
	public string summary;
	public bool forcedToAdd;
	public int LordExp;
	public GameGoalSystem.GoalType type;
	public List<KeyValueData.KeyValue<string, string[]>> param;
	public List<KeyValueData.KeyValue<EffectData, string[]>> rewardEffects;

	public Dictionary<string, int> Conditions
	{
		get
		{
			var result = new Dictionary<string, int>();
			switch (type)
			{
				case GameGoalSystem.GoalType.AddItem:
				case GameGoalSystem.GoalType.ItemDemand:
				case GameGoalSystem.GoalType.KillEnemy:
					foreach (var para in param)
					{
						int value;
						if (int.TryParse(para.Value[0], out value))
						{
							result.Add(para.Key, value);
						}
					}
					break;
				default:
					break;
			}
			return result;
		}
	}

	public static GoalData GetDataByKey(string key)
	{
		return KeyValueData.GetValue<GoalData>(GameManager.Config.GameGoal, key);
	}
}
