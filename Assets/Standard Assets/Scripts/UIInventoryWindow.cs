using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UIInventoryWindow : UIWindow
{
	// public class DragData
	// {
	// 	public UIInventorySlot DraggedEntry;
	// 	public RectTransform OriginalParent;
	// }
	public GameObject EquipRoot;
	public RectTransform ItemSlots;
	public Text textStats;
	public ItemEntryUI ItemEntryPrefab;
	public ItemTooltip Tooltip;

	public UIInventorySlot HeadSlot;
	public UIInventorySlot TorsoSlot;
	public UIInventorySlot LegsSlot;
	public UIInventorySlot FeetSlot;
	public UIInventorySlot WeaponSlot_1;
	public UIInventorySlot WeaponSlot_2;

	//public DragData CurrentlyDragged { get; set; }
	public CanvasScaler DragCanvasScaler { get; private set; }

	UIInventorySlot[] m_ItemEntries;
	UIInventorySlot m_SelectedSlot;

	void Awake()
	{
		//CurrentlyDragged = null;

		DragCanvasScaler = GameManager.GameUI.DragCanvas.GetComponentInParent<CanvasScaler>();
		m_ItemEntries = ItemSlots.GetComponentsInChildren<UIInventorySlot>();
		GameManager.CurHero.Inventory.ItemAction += (item, eventName, num) => { Load(); };

		// m_ItemEntries = new UIInventorySlot[ItemSlots.Length];

		for (int i = 0; i < m_ItemEntries.Length; ++i)
		{
			// m_ItemEntries[i] = Instantiate(ItemEntryPrefab, ItemSlots[i]);
			// m_ItemEntries[i].gameObject.SetActive(false);
			//m_ItemEntries[i].Owner = this;
			m_ItemEntries[i].InventoryID = i;
		}
	}

	protected override void OnOpen()
	{
		m_SelectedSlot = null;
		Tooltip.gameObject.SetActive(false);
		EquipRoot.SetActive(GameManager.StoryListener.CurrentTeller == null);
		GameManager.Instance.CameraCtrl.SetMode(CameraController.Mode.INVENTORY);
		Load();
	}

	protected override void OnClose()
	{
		GameManager.Instance.CameraCtrl.SetMode(CameraController.Mode.RPG);
	}
	public void Load()
	{
		UpdateEquipment(GameManager.CurHero.Equipment, GameManager.CurHero.Stats);
		UpdateWeapon(GameManager.CurHero.Equipment);
		for (int i = 0; i < m_ItemEntries.Length; ++i)
		{
			m_ItemEntries[i].UpdateEntry(GameManager.CurHero);
		}
		StatSystem.Stats stats = GameManager.CurHero.Stats.stats;
		textStats.text = $"Str : {stats.strength} Def : {stats.defense} Agi : {stats.agility}  Spr : {stats.spirit}";
	}

	public void OnSlotSelected(UIInventorySlot slot, bool selected)
	{
		m_SelectedSlot = selected ? slot : null;
		Tooltip.SetItem(m_SelectedSlot);
		// Item itemUsed = m_HoveredItem.InventoryID != -1 ? m_Data.Inventory.Entries[m_HoveredItem.InventoryID].Item : m_HoveredItem.EquipmentItem;
	}

	public override void OnButtonClick(string eventName)
	{
		EquipmentItem equip = m_SelectedSlot?.item as EquipmentItem;
		switch (eventName)
		{
			case "Equip":
				if (equip)
				{
					GameManager.CurHero.Inventory.EquipItem(equip);
					m_SelectedSlot.tog.isOn = false;
					Load();

				}
				break;
			case "UnEquip":
				if (equip)
				{
					GameManager.CurHero.Inventory.UnEquipItem(equip);
					m_SelectedSlot.tog.isOn = false;
					Load();
				}
				break;
			case "Drop":
				GameObject lootObj = Resources.Load("Loot") as GameObject;
				if (lootObj)
				{
					GameManager.CurHero.Inventory.MinusItem(m_SelectedSlot.InventoryID, 1);
					Loot loot = Instantiate(lootObj, GameManager.CurHero.transform.position + new Vector3(2, 0, 0), Quaternion.Euler(0, 0, 0)).GetComponent<Loot>();
					loot.Item = m_SelectedSlot.item;
					m_SelectedSlot.tog.isOn = false;
					Load();
					//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
				}
				break;
			case "Use":
				UsableItem item = m_SelectedSlot.item as UsableItem;
				if (item.UsedBy(GameManager.CurHero))
				{
					GameManager.CurHero.Inventory.MinusItem(m_SelectedSlot.InventoryID, 1);
					m_SelectedSlot.tog.isOn = GameManager.CurHero.Inventory.ItemCount(item.ItemName) > 1;
					Load();
				}
				break;
			case "Add":
				GameManager.GameUI.OpenWindow("winConfirm");
				break;
			case "Give":
				if (GameManager.StoryListener.CurrentTeller.GiveItem(m_SelectedSlot.item))
					GameManager.CurHero.Inventory.MinusItem(m_SelectedSlot.InventoryID, 1);
				m_SelectedSlot.tog.isOn = false;
				Load();
				break;
			case "Craft":
				GameManager.GameUI.SwitchWindow("winCraft");
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

