using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class UIItemDemandHud : MonoBehaviour
{
	public List<UIItemListBox> uiList;
	[SerializeField]
	GameObject uiElement;
	InventorySystem.ItemDemand m_Demand;
	public void Init(InventorySystem.ItemDemand demand)
	{
		m_Demand = demand;
		foreach (var item in m_Demand.Demand)
		{
			UIItemListBox uiItem = Instantiate(uiElement, transform, false).GetComponent<UIItemListBox>();
			uiItem.Init(item);
		}
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
	}
}
