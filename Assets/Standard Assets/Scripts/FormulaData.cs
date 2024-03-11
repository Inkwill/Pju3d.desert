using System.Linq;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

[CreateAssetMenu(fileName = "FormulaData", menuName = "Data/FormulaData", order = 100)]
public class FormulaData : ScriptableObject
{
	public Item product;
	public List<KeyValueData.KeyValue<Item, int>> requireList;
	InventorySystem.ItemDemand m_Demand;

	public void Craft(InventorySystem inventory)
	{
		m_Demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(requireList));
		if (canCraft(inventory))
		{
			m_Demand.Fulfill(inventory);
			inventory.AddItem(product);
		}
	}

	public bool canCraft(InventorySystem inventory)
	{
		int submittableCount = m_Demand.Submittable(inventory).Values.Sum();
		int requireCount = m_Demand.DemandLeft.Values.Sum();
		return submittableCount >= requireCount;
	}
}
