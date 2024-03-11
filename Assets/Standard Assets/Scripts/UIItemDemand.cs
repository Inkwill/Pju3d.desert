using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using MyBox;
using UnityEngine.UI;
using System.Linq;

public class UIItemDemand : MonoBehaviour
{
	public enum ShowType
	{
		List,
		OnlyOne
	}
	public ShowType Type = ShowType.List;

	[ConditionalField(nameof(Type), false, ShowType.List)]
	[SerializeField]
	GameObject uiElement;
	List<UIItemListBox> uiList;
	InventorySystem.ItemDemand m_Demand;

	[ConditionalField(nameof(Type), false, ShowType.OnlyOne)]
	[SerializeField]
	Slider m_demandSlider;
	[ConditionalField(nameof(Type), false, ShowType.OnlyOne)]
	[SerializeField]
	Image m_itemIcon;

	public void Show(InventorySystem.ItemDemand demand)
	{
		m_Demand = demand;
		switch (Type)
		{
			case ShowType.List:
				UIItemListBox[] uiList = GetComponentsInChildren<UIItemListBox>();
				int index = 0;
				UIItemListBox uiItem;
				foreach (var item in m_Demand.Demand)
				{
					if (uiList.Length > index) uiItem = uiList[index];
					else uiItem = Instantiate(uiElement, transform, false).GetComponent<UIItemListBox>();
					uiItem.SetDemandInfo(item, m_Demand.DemandLeft[item.Key]);
					index++;
				}
				break;
			case ShowType.OnlyOne:
				string itemKey = m_Demand.Demand.Keys.FirstOrDefault();
				Item it = KeyValueData.GetValue<Item>(GameManager.Config.Item, itemKey);
				m_itemIcon.sprite = it.ItemSprite;
				m_demandSlider.value = m_Demand.GetProgress(itemKey);
				break;
			default:
				break;
		}

		gameObject.SetActive(true);
	}

	public void Inter()
	{
		GetComponent<Animator>()?.SetTrigger("inter");
	}

	public void Fail()
	{
		GetComponent<Animator>()?.SetTrigger("fail");
	}
}
