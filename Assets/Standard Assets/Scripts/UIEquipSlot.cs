using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipSlot : MonoBehaviour
{
	public EquipmentItem.EquipmentSlot slotType;
	public Sprite defaultSprite;
	public Image icon;
	public EquipmentItem EquipItem { get { return m_equipItem; } }
	EquipmentItem m_equipItem;
	Button m_button;

	void Awake()
	{
		m_button = GetComponent<Button>();
	}
	void Start()
	{
		GameManager.CurHero.Equipment.OnEquiped += (eq) => { UpdateInfo(); };
		GameManager.CurHero.Equipment.OnUnequip += (eq) => { UpdateInfo(); };
		GameManager.CurHero.Equipment.OnEquipViceWeapon += (eq) => { UpdateInfo(); };
		m_button.onClick.AddListener(() =>
		{
			UIInventoryWindow win = GameManager.GameUI.GetWindow("winInventory") as UIInventoryWindow;
			win.OnEquipSelected(this);
		});
		UpdateInfo();
	}

	void UpdateInfo()
	{
		if (slotType == EquipmentItem.EquipmentSlot.ViceWeapon) m_equipItem = GameManager.CurHero.Equipment.ViceWeapon;
		else if (slotType == EquipmentItem.EquipmentSlot.Weapon) m_equipItem = GameManager.CurHero.Equipment.Weapon;
		else m_equipItem = GameManager.CurHero.Equipment.GetItem(slotType);
		icon.sprite = m_equipItem != null ? m_equipItem.ItemSprite : defaultSprite;
		m_button.interactable = m_equipItem != null;
	}
}
