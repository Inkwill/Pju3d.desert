using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using System.Collections.Generic;

public class UIShopWindow : UIWindow
{
	[SerializeField]
	ShopData m_data;
	[SerializeField]
	GameObject uicommodityElement;
	[SerializeField]
	ToggleGroup uicommodityRoot;
	[SerializeField]
	UIItemListBox m_currencyItemBoxs;
	[SerializeField]
	TMP_Text commodity_Name;
	[SerializeField]
	Text commodity_Desc;
	[SerializeField]
	Text requireLv;
	[SerializeField]
	Button bt_Buy;
	int m_selectIndex;


	void Awake()
	{
		GameManager.CurHero.Inventory.ItemAction += OnItemAction;
	}

	protected override void OnOpen()
	{
		var elements = Helpers.AdjustElements<UIElementBase>(uicommodityRoot.transform, m_data.commodities.Count, uicommodityElement);

		for (int i = 0; i < elements.Length; i++)
		{
			UIElementBase element = elements[i].GetComponent<UIElementBase>();
			Item commodity = m_data.commodities[i].Key;
			element.icon.sprite = commodity.ItemSprite;
			element.toggle.group = uicommodityRoot;
			element.index = i;
			element.currencyIcon.sprite = m_data.currency.ItemSprite;
			element.currencyCount.text = m_data.commodities[i].Value.ToString();
			element.toggle.onValueChanged.AddListener((value) => { if (value) m_selectIndex = element.index; UpdateInfo(); });
		}
		UpdateInfo();
	}
	void UpdateInfo()
	{
		commodity_Name.text = m_data.commodities[m_selectIndex].Key.ItemName;
		commodity_Desc.text = m_data.commodities[m_selectIndex].Key.Description;
		requireLv.text = $"Lv:{m_data.commodities[m_selectIndex].Value}";
		requireLv.color = GameManager.Lord.Lv >= m_data.commodities[m_selectIndex].Value ? Color.green : Color.red;
		bt_Buy.interactable = GameManager.CurHero.Inventory.ItemCount(m_data.currency.ItemName) >= m_data.commodities[m_selectIndex].Value &&
								GameManager.Lord.Lv >= m_data.commodities[m_selectIndex].Value;
		m_currencyItemBoxs.SetRequiredInfo(m_data.currency, m_data.commodities[m_selectIndex].Value);
	}

	public void Buy()
	{
		var commodity = m_data.commodities[m_selectIndex].Key;
		if (GameManager.CurHero.Inventory.AddItem(commodity))
			m_data.PriceDemand(commodity).Fulfill(GameManager.CurHero.Inventory);
	}

	void OnItemAction(Item item, string actionName, int num)
	{
		UpdateInfo();
	}
}
