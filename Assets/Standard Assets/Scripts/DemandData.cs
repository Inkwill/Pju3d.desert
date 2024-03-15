using CreatorKitCode;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemandData", menuName = "Data/DemandData", order = 300)]
public class DemandData : ScriptableObject
{
	public List<KeyValueData.KeyValue<Item, int>> itemDemand;

	public InventorySystem.ItemDemand CreatItemDemand()
	{
		return new InventorySystem.ItemDemand(KeyValueData.ToDic(itemDemand));
	}
}
