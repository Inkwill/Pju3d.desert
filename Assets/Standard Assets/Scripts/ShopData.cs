using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Data/ShopData", order = 700)]
public class ShopData : ScriptableObject
{
	public string ShopId;
	public ResItem currency;
	public List<KeyValueData.KeyValue<Item, int>> commodities;

	public InventorySystem.ItemDemand PriceDemand(Item item)
	{
		var demand = new InventorySystem.ItemDemand(new Dictionary<string, int>()
			{
				{currency.ItemName,item.value / currency.value}
			});
		return demand;
	}
}
