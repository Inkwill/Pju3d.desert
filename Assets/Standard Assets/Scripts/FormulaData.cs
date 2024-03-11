using System.Linq;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

[CreateAssetMenu(fileName = "FormulaData", menuName = "Data/FormulaData", order = 100)]
public class FormulaData : ScriptableObject
{
	public Item product;
	public List<KeyValueData.KeyValue<Item, int>> requireList;

	public void Craft(InventorySystem inventory)
	{
		var demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(requireList));
		if (canCraft(inventory))
		{
			demand.Fulfill(inventory);
			inventory.AddItem(product);
		}
	}

	public bool canCraft(InventorySystem inventory)
	{
		var demand = new InventorySystem.ItemDemand(KeyValueData.ToDic(requireList));
		int submittableCount = demand.Submittable(inventory).Values.Sum();
		int requireCount = demand.DemandLeft.Values.Sum();
		return submittableCount >= requireCount;
	}
}
