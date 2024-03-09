using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionData", menuName = "Data/ConditionData", order = 15)]
public class ConditionData : ScriptableObject
{
	public enum ConditionType
	{
		INCLUDE_ITEM,
		EXCLUDE_ITEM,
	}
	public ConditionType type;

	public bool Judgment(string[] param)
	{
		bool result = true;
		switch (type)
		{
			case ConditionType.INCLUDE_ITEM:
				foreach (var itemId in param)
				{
					result &= (GameManager.CurHero.Inventory.ItemCount(itemId) > 0);
					result &= GameManager.CurHero.Equipment.IsEquiped(itemId);
				}
				break;
			case ConditionType.EXCLUDE_ITEM:
				foreach (var itemId in param)
				{
					result &= (GameManager.CurHero.Inventory.ItemCount(itemId) == 0);
					result &= !GameManager.CurHero.Equipment.IsEquiped(itemId);
				}
				break;
			default:
				break;
		}
		return result;
	}

	public static bool JudgmentList(List<KeyValueData.KeyValue<ConditionData, string[]>> conditions)
	{
		bool result = true;
		if (conditions != null && conditions.Count > 0)
		{
			foreach (KeyValueData.KeyValue<ConditionData, string[]> condition in conditions)
			{
				result &= condition.Key.Judgment(condition.Value);
			}
		}
		return result;
	}
}
