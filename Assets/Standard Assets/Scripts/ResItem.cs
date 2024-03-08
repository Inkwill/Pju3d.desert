using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "ResItem", menuName = "Data/Res Item", order = -999)]
public class ResItem : Item
{
	public enum ResType
	{
		Wood,
		Water,
	}
	public ResType Type;

	public static ResItem GetResItemByType(ResType restype)
	{
		switch (restype)
		{
			case ResType.Wood:
				return KeyValueData.GetValue<Item>(GameManager.Config.Item, "Wood") as ResItem;
			case ResType.Water:
				return KeyValueData.GetValue<Item>(GameManager.Config.Item, "Water") as ResItem;
			default:
				return null;
		}
	}
}

