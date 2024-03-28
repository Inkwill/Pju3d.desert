using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipSlot : MonoBehaviour
{
	public EquipmentItem.EquipmentSlot slotType;
	Sprite m_defaultSprite;
	public Image icon;
	public EquipmentItem EquipItem { get { return m_equipItem; } }
	EquipmentItem m_equipItem;
	Toggle m_toggle;

	void Awake()
	{
		m_toggle = GetComponent<Toggle>();
		m_defaultSprite = icon.sprite;
	}
	void Start()
	{
		GameManager.CurHero.Equipment.OnEquiped += (eq) => { UpdateInfo(); };
		GameManager.CurHero.Equipment.OnUnequip += (eq) => { UpdateInfo(); };
		GameManager.CurHero.Equipment.OnEquipViceWeapon += (eq) => { UpdateInfo(); };
		m_toggle.onValueChanged.AddListener((value) =>
		{
			if (value)
			{
				UIInventoryWindow win = GameManager.GameUI.GetWindow<UIInventoryWindow>("winInventory");
				if (!win.gameObject.activeSelf) win.Open();
				win.OnEquipSelected(this);
			}
		});
		UpdateInfo();
	}

	void UpdateInfo()
	{
		if (slotType == EquipmentItem.EquipmentSlot.ViceWeapon) m_equipItem = GameManager.CurHero.Equipment.ViceWeapon;
		else if (slotType == EquipmentItem.EquipmentSlot.Weapon) m_equipItem = GameManager.CurHero.Equipment.Weapon;
		else m_equipItem = GameManager.CurHero.Equipment.GetItem(slotType);
		icon.sprite = m_equipItem != null ? m_equipItem.ItemSprite : m_defaultSprite;
		m_toggle.interactable = m_equipItem != null;
	}
}
