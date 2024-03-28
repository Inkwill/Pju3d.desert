using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;
using DG.Tweening;
using System;

public class UIInventoryWindow : UIWindow
{
	public RectTransform ItemSlots;
	public Text textStats;
	public ItemEntryUI ItemEntryPrefab;
	public ItemTooltip Tooltip;
	public CanvasScaler DragCanvasScaler { get; private set; }
	public Button btConfirm;

	UIInventorySlot[] m_ItemEntries;
	UIInventorySlot m_SelectedSlot;
	UIEquipSlot m_SelectedEquipSlot;
	UIInventorySlot m_curDeviceSlot;
	GameObject m_deviceModle;

	void Awake()
	{
		//CurrentlyDragged = null;

		DragCanvasScaler = GameManager.GameUI.DragCanvas.GetComponentInParent<CanvasScaler>();
		m_ItemEntries = ItemSlots.GetComponentsInChildren<UIInventorySlot>();
		GameManager.CurHero.Inventory.ItemAction += (item, eventName, num) => { Load(); };
		GameManager.CurHero.Equipment.OnEquiped += (eq) => { Load(); };
		GameManager.CurHero.Equipment.OnEquipViceWeapon += (eq) => { Load(); };
		GameManager.CurHero.Equipment.OnUnequip += (eq) => { Load(); };

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
		btConfirm.gameObject.SetActive(false);
		GameManager.GameUI.winRpg.bottomRoot.DOMove(GameManager.GameUI.winRpg.bottomRoot.position + new Vector3(0, 150, 0), 0.2f);
		//GameManager.Instance.CameraCtrl.SetMode(CameraController.Mode.INVENTORY);
		Load();
	}

	protected override void OnClose()
	{
		GameManager.GameUI.winRpg.bottomRoot.DOMove(GameManager.GameUI.winRpg.bottomRoot.position - new Vector3(0, 150, 0), 0.2f);
		if (m_curDeviceSlot != null)
		{
			CanclePlace();
		}
	}

	void Update()
	{
		if (m_SelectedSlot != null && !GameManager.CurHero.BaseAI.isIdle) m_SelectedSlot.GetComponent<Toggle>().isOn = false;
	}
	public void Load()
	{
		Tooltip.gameObject.SetActive(false);
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
		if (m_curDeviceSlot != null && slot != m_curDeviceSlot && selected)
		{
			CanclePlace();
		}
		// Item itemUsed = m_HoveredItem.InventoryID != -1 ? m_Data.Inventory.Entries[m_HoveredItem.InventoryID].Item : m_HoveredItem.EquipmentItem;
	}

	public void OnEquipSelected(UIEquipSlot slot)
	{
		m_SelectedEquipSlot = slot;
		Tooltip.SetEquip(slot);
	}

	public override void OnButtonClick(string eventName)
	{
		EquipmentItem equip = null;
		if (m_SelectedEquipSlot) equip = m_SelectedEquipSlot.EquipItem;
		if (equip == null && m_SelectedSlot != null) equip = m_SelectedSlot?.item as EquipmentItem;
		switch (eventName)
		{
			case "Equip":
				if (equip)
				{
					GameManager.CurHero.Inventory.EquipItem(equip);
					if (m_SelectedSlot) m_SelectedSlot.tog.isOn = false;
					//Load();

				}
				break;
			case "UnEquip":
				if (equip)
				{
					GameManager.CurHero.Inventory.UnEquipItem(equip);
					if (m_SelectedSlot) m_SelectedSlot.tog.isOn = false;
					//Load();
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
			case "Place":
				m_curDeviceSlot = m_SelectedSlot;
				DeviceItem device = m_curDeviceSlot.item as DeviceItem;
				btConfirm.gameObject.SetActive(true);
				btConfirm.onClick.AddListener(PlaceDevice);
				if (GameManager.CurHero.BaseAI.SceneDetector && device.prefab)
				{
					m_deviceModle = Instantiate(device.modle, GameManager.CurHero.BaseAI.SceneDetector.transform, false);
					m_deviceModle.transform.localPosition = Vector3.zero;
				}
				Tooltip.gameObject.SetActive(false);
				break;
			default:
				break;
		}

	}

	void PlaceDevice()
	{
		DeviceItem device = m_curDeviceSlot.item as DeviceItem;
		Instantiate(device.prefab, GameManager.CurHero.BaseAI.SceneDetector.transform.position, GameManager.CurHero.BaseAI.SceneDetector.transform.rotation);
		GameManager.CurHero.Inventory.MinusItem(m_curDeviceSlot.InventoryID, 1);
		CanclePlace();
		Load();
	}

	public void CanclePlace()
	{
		btConfirm.gameObject.SetActive(false);
		btConfirm.onClick.RemoveListener(PlaceDevice);
		Destroy(m_deviceModle);
		m_curDeviceSlot = null;
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
}

