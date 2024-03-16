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
		Demand,
		OnlyOne,
		Require
	}
	public ShowType Type = ShowType.Demand;

	[ConditionalField(nameof(Type), false, ShowType.Demand)]
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

	void Start()
	{
		ItemDemander demander = GetComponentInParent<ItemDemander>();
		if (demander != null)
		{
			demander.demandEvent += (demand, eventName) =>
			{
				if (eventName == "Fail") GetComponent<Animator>()?.SetTrigger("fail");
				else if (eventName == "Complete") gameObject.SetActive(false);
				else Show(demand);
			};
			demander.GetComponent<InteractHandle>()?.EnterEvent.AddListener(() => GetComponent<Animator>()?.SetTrigger("inter"));
		}
	}
	public void Show(InventorySystem.ItemDemand demand)
	{
		m_Demand = demand;
		UIItemListBox[] uiList = GetComponentsInChildren<UIItemListBox>();
		int index = 0;
		UIItemListBox uiItem;
		switch (Type)
		{
			case ShowType.Demand:
				foreach (var de in m_Demand.Demand)
				{
					if (uiList.Length > index) uiItem = uiList[index];
					else uiItem = Instantiate(uiElement, transform, false).GetComponent<UIItemListBox>();
					uiItem.SetDemandInfo(de, m_Demand.DemandLeft[de.Key]);
					index++;
				}
				break;
			case ShowType.OnlyOne:
				string itemKey = m_Demand.Demand.Keys.FirstOrDefault();
				Item it = KeyValueData.GetValue<Item>(GameManager.Config.Item, itemKey);
				m_itemIcon.sprite = it.ItemSprite;
				m_demandSlider.value = m_Demand.GetProgress(itemKey);
				break;
			case ShowType.Require:
				foreach (var de in m_Demand.Demand)
				{
					if (uiList.Length > index) uiItem = uiList[index];
					else uiItem = Instantiate(uiElement, transform, false).GetComponent<UIItemListBox>();
					uiItem.SetRequiredInfo(de.Key, m_Demand.DemandLeft[de.Key]);
					index++;
				}
				break;
			default:
				break;
		}
		gameObject.SetActive(true);
	}
}
