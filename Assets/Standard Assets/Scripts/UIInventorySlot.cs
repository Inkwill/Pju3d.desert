using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IPointerClickHandler
{
	public int InventoryID { get; set; } = -1;

	[SerializeField]
	Image iconItem;
	[SerializeField]
	Sprite defaultSprite;
	public Text ItemCount;
	public bool equipment;

	public Toggle tog;
	Item m_item;
	CharacterData m_CharacterData;

	UIInventoryWindow winInventory;

	public Item item
	{
		get { return m_item; }
		set
		{
			m_item = value;
			iconItem.sprite = m_item ? m_item.ItemSprite : defaultSprite;
		}
	}

	void Start()
	{
		tog = GetComponent<Toggle>();
		tog.onValueChanged.AddListener(OnToggled);
		winInventory = tog.group.GetComponent<UIInventoryWindow>();
	}
	public void UpdateEntry(CharacterData data)
	{
		m_CharacterData = data;
		var entry = m_CharacterData.Inventory.Entries[InventoryID];

		//gameObject.SetActive(isEnabled);
		item = entry?.Item;

		if (item && entry?.Count > 1)
		{
			//iconItem.sprite = entry.Item.ItemSprite;

			if (ItemCount) ItemCount.text = entry.Count.ToString();
		}
		else
		{
			if (ItemCount) ItemCount.text = "";
		}
	}


	public void OnToggled(bool selected)
	{
		if (selected) SFXManager.PlayClip("selected");//SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.PickupSound });
		else SFXManager.PlayClip("cancle");
		winInventory.OnSlotSelected(this, selected);
	}

	void OnDisable()
	{
		Toggle tog = GetComponent<Toggle>();
		if (tog && tog.isOn) tog.isOn = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickCount % 2 == 0)
		{
			if (m_item)
			{
				if (equipment) winInventory.OnButtonClick("UnEquip");
				else if (m_item is EquipmentItem) winInventory.OnButtonClick("Equip");
				else Debug.Log("OnDoubleClick item : " + item.ItemName);
				tog.isOn = false;
			}
		}
	}
}
