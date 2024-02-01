using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class UIItemDemand : MonoBehaviour
{
	public List<UIItemListBox> uiList;
	[SerializeField]
	GameObject uiElement;
	InventorySystem.ItemDemand m_Demand;
	public void Show(InventorySystem.ItemDemand demand)
	{
		m_Demand = demand;
		UIItemListBox[] uiList = GetComponentsInChildren<UIItemListBox>();
		int index = 0;
		UIItemListBox uiItem;
		foreach (var item in m_Demand.Demand)
		{
			if (uiList.Length > index) uiItem = uiList[index];
			else uiItem = Instantiate(uiElement, transform, false).GetComponent<UIItemListBox>();
			uiItem.UpdateInfo(item, m_Demand.DemandLeft[item.Key]);
			index++;
		}
		gameObject.SetActive(true);
	}
}
