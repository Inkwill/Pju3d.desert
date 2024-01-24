using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UIInventoryWindow : UIWindow
{
	public class DragData
	{
		public UIInventorySlot DraggedEntry;
		public RectTransform OriginalParent;
	}

	public RectTransform ItemSlots;

	public Item testItem;
	public Item testItem1;

	public ItemEntryUI ItemEntryPrefab;
	public ItemTooltip Tooltip;

	public UIInventorySlot HeadSlot;
	public UIInventorySlot TorsoSlot;
	public UIInventorySlot LegsSlot;
	public UIInventorySlot FeetSlot;
	public UIInventorySlot WeaponSlot_1;
	public UIInventorySlot WeaponSlot_2;

	public DragData CurrentlyDragged { get; set; }
	public CanvasScaler DragCanvasScaler { get; private set; }

	public CharacterData Character
	{
		get { return m_player.Data; }
	}

	UIInventorySlot[] m_ItemEntries;
	UIInventorySlot m_SelectedSlot;

	protected override void Init()
	{
		CurrentlyDragged = null;

		DragCanvasScaler = m_DragCanvas.GetComponentInParent<CanvasScaler>();
		m_ItemEntries = ItemSlots.GetComponentsInChildren<UIInventorySlot>();

		// m_ItemEntries = new UIInventorySlot[ItemSlots.Length];

		for (int i = 0; i < m_ItemEntries.Length; ++i)
		{
			// m_ItemEntries[i] = Instantiate(ItemEntryPrefab, ItemSlots[i]);
			// m_ItemEntries[i].gameObject.SetActive(false);
			//m_ItemEntries[i].Owner = this;
			m_ItemEntries[i].InventoryID = i;
		}

		if (testItem) m_player.Data.Inventory.AddItem(testItem);
		if (testItem1) m_player.Data.Inventory.AddItem(testItem1);
	}

	protected override void OnOpen()
	{
		m_SelectedSlot = null;
		Tooltip.gameObject.SetActive(false);
		Load();
	}
	public void Load()
	{
		UpdateEquipment(m_player.Data.Equipment, m_player.Data.Stats);
		UpdateWeapon(m_player.Data.Equipment);
		for (int i = 0; i < m_ItemEntries.Length; ++i)
		{
			m_ItemEntries[i].UpdateEntry(m_player.Data);
		}
	}

	public void ObjectDoubleClicked(InventorySystem.InventoryEntry usedItem)
	{
		// if (m_Data.Inventory.UseItem(usedItem))
		// 	SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = usedItem.Item is EquipmentItem ? SFXManager.ItemEquippedSound : SFXManager.ItemUsedSound });

		// ObjectHoverExited(m_HoveredItem);
		// Load(m_Data);
	}

	public void EquipmentDoubleClicked(EquipmentItem equItem)
	{
		// m_Data.Equipment.Unequip(equItem.Slot);
		// ObjectHoverExited(m_HoveredItem);
		// Load(m_Data);
	}

	public void OnSlotSelected(UIInventorySlot slot, bool selected)
	{
		m_SelectedSlot = selected ? slot : null;
		Tooltip.SetItem(m_SelectedSlot);
		// Item itemUsed = m_HoveredItem.InventoryID != -1 ? m_Data.Inventory.Entries[m_HoveredItem.InventoryID].Item : m_HoveredItem.EquipmentItem;
	}

	public void OnClickItemEvent(string eventName)
	{
		EquipmentItem equip = m_SelectedSlot.item as EquipmentItem;
		Weapon wp = equip as Weapon;
		switch (eventName)
		{
			case "Equip":
				if (equip)
				{
					if (wp) m_player.Data.Equipment.EquipWeapon(wp);
					else m_player.Data.Equipment.Equip(equip);
					m_player.Data.Inventory.RemoveItem(m_SelectedSlot.InventoryID);
					m_SelectedSlot.tog.isOn = false;
					Load();
				}
				break;
			case "UnEquip":
				if (equip)
				{
					if (wp) m_player.Data.Equipment.UnWeapon(wp);
					else m_player.Data.Equipment.Unequip(equip.Slot);
					//m_player.Data.Inventory.AddItem(m_SelectedSlot.item);
					m_SelectedSlot.tog.isOn = false;
					Load();
				}
				break;
			default:
				break;
		}

	}

	public void ObjectHoverExited(ItemEntryUI exited)
	{
		// if (m_HoveredItem == exited)
		// {
		// 	m_HoveredItem = null;
		// 	Tooltip.gameObject.SetActive(false);
		// }
	}

	public void HandledDroppedEntry(Vector3 position)
	{
		// for (int i = 0; i < ItemSlots.Length; ++i)
		// {
		// 	RectTransform t = ItemSlots[i];

		// 	if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
		// 	{
		// 		if (m_ItemEntries[i] != CurrentlyDragged.DraggedEntry)
		// 		{
		// 			var prevItem = m_Data.Inventory.Entries[CurrentlyDragged.DraggedEntry.InventoryID];
		// 			m_Data.Inventory.Entries[CurrentlyDragged.DraggedEntry.InventoryID] = m_Data.Inventory.Entries[i];
		// 			m_Data.Inventory.Entries[i] = prevItem;

		// 			CurrentlyDragged.DraggedEntry.UpdateEntry();
		// 			m_ItemEntries[i].UpdateEntry();
		// 		}
		// 	}
		// }
	}

	public void UpdateEquipment(EquipmentSystem equipment, StatSystem system)
	{
		HeadSlot.item = equipment.GetItem(EquipmentItem.EquipmentSlot.Head);
		TorsoSlot.item = equipment.GetItem(EquipmentItem.EquipmentSlot.Torso);
		LegsSlot.item = equipment.GetItem(EquipmentItem.EquipmentSlot.Legs);
		FeetSlot.item = equipment.GetItem(EquipmentItem.EquipmentSlot.Feet);
	}

	public void UpdateWeapon(EquipmentSystem equipment)
	{
		WeaponSlot_1.item = equipment.Weapon;
		WeaponSlot_2.item = equipment.ViceWeapon;
	}
}

