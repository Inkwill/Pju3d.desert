using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;

public class UIInventorySlot : MonoBehaviour
{
	public int InventoryID { get; set; } = -1;

	[SerializeField]
	Image iconItem;
	[SerializeField]
	Sprite defaultSprite;
	[SerializeField]
	AudioClip SlectedClip;
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
		if (SlectedClip && selected) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SlectedClip });
		winInventory.OnSlotSelected(this, selected);
	}

	void OnDisable()
	{
		Toggle tog = GetComponent<Toggle>();
		if (tog && tog.isOn) tog.isOn = false;
	}
}
