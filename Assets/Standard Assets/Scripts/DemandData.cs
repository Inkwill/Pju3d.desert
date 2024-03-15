using CreatorKitCode;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemandData", menuName = "Data/DemandData", order = 300)]
public class DemandData : ScriptableObject
{
	public List<KeyValueData.KeyValue<Item, int>> itemDemand;
	InventorySystem.ItemDemand m_instance;

	public InventorySystem.ItemDemand Instance()
	{
		if (m_instance != null) return m_instance;
		else return new InventorySystem.ItemDemand(KeyValueData.ToDic(itemDemand));
	}
}
