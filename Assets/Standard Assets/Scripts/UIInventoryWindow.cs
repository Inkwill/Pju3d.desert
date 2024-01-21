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

	public ItemEntryUI ItemEntryPrefab;
	public ItemTooltip Tooltip;

	public UIInventorySlot HeadSlot;
	public UIInventorySlot TorsoSlot;
	public UIInventorySlot LegsSlot;
	public UIInventorySlot FeetSlot;

	public DragData CurrentlyDragged { get; set; }
	public CanvasScaler DragCanvasScaler { get; private set; }

	public CharacterData Character
	{
		get { return m_Data; }
	}

	UIInventorySlot[] m_ItemEntries;
	UIInventorySlot m_HoveredItem;
	CharacterData m_Data;

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

		//EquipementUI.Init(this);
	}

	protected override void OnOpen()
	{
		m_HoveredItem = null;
		Tooltip.gameObject.SetActive(false);
		Load(m_player.Data);
	}

	public void Load(CharacterData data)
	{
		m_Data = data;
		UpdateEquipment(m_Data.Equipment, m_Data.Stats);
		if (testItem) m_Data.Inventory.AddItem(testItem);
		for (int i = 0; i < m_ItemEntries.Length; ++i)
		{
			m_ItemEntries[i].UpdateEntry(data);
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

	public void ObjectHoveredEnter(ItemEntryUI hovered)
	{
		// m_HoveredItem = hovered;

		// Tooltip.gameObject.SetActive(true);

		// Item itemUsed = m_HoveredItem.InventoryID != -1 ? m_Data.Inventory.Entries[m_HoveredItem.InventoryID].Item : m_HoveredItem.EquipmentItem;

		// Tooltip.Name.text = itemUsed.ItemName;
		// Tooltip.DescriptionText.text = itemUsed.GetDescription();
	}

	public void ObjectHoverExited(ItemEntryUI exited)
	{
		if (m_HoveredItem == exited)
		{
			m_HoveredItem = null;
			Tooltip.gameObject.SetActive(false);
		}
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
}

