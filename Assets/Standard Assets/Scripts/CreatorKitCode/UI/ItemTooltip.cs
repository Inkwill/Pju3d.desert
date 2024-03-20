using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

namespace CreatorKitCodeInternal
{
	public class ItemTooltip : MonoBehaviour
	{
		public TMP_Text Name;
		public Text DescriptionText;
		public Image icon;
		public Button btEquip;
		public Button btUnEquip;
		public Button btDrop;
		public Button btUse;
		public Button btGive;
		public Button btPlace;


		void Start()
		{
			// btEquip.gameObject.SetActive(false);
			// btUnEquip.gameObject.SetActive(false);
		}
		void Update()
		{
			if (!GameManager.CurHero.BaseAI.isIdle) gameObject.SetActive(false);
		}
		public void SetEquip(UIEquipSlot slot)
		{
			if (slot && slot.EquipItem)
			{
				Name.text = slot.EquipItem.ItemName;
				icon.sprite = slot.EquipItem.ItemSprite;
				DescriptionText.text = slot.EquipItem.GetDescription();
				btUnEquip.gameObject.SetActive(true);

				btGive.gameObject.SetActive(false);
				btEquip.gameObject.SetActive(false);
				btDrop.gameObject.SetActive(false);
				btUse.gameObject.SetActive(false);
				btPlace.gameObject.SetActive(false);

				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		public void SetItem(UIInventorySlot slot)
		{
			if (slot && slot.item)
			{
				Name.text = slot.item.ItemName;
				icon.sprite = slot.item.ItemSprite;
				DescriptionText.text = slot.item.GetDescription();
				EquipmentItem equItem = slot.item as EquipmentItem;
				UsableItem useItem = slot.item as UsableItem;
				DeviceItem device = slot.item as DeviceItem;
				if (GameManager.StoryListener.CurrentTeller != null)
				{
					btEquip.gameObject.SetActive(false);
					btUnEquip.gameObject.SetActive(false);
					btDrop.gameObject.SetActive(false);
					btUse.gameObject.SetActive(false);
					btPlace.gameObject.SetActive(false);
					btGive.gameObject.SetActive(!slot.equipment);
				}
				else
				{
					btGive.gameObject.SetActive(false);
					btEquip.gameObject.SetActive(!slot.equipment && equItem);
					btUnEquip.gameObject.SetActive(slot.equipment);
					btDrop.gameObject.SetActive(!slot.equipment && !device);
					btUse.gameObject.SetActive(useItem);
					btPlace.gameObject.SetActive(device && !device.undergroud);
				}
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
