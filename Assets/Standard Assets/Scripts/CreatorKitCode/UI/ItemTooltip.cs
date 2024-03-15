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

		public void SetItem(UIInventorySlot slot)
		{
			if (slot && slot.item)
			{
				gameObject.SetActive(true);
				Name.text = slot.item.ItemName;
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
				btUnEquip.interactable = GameManager.CurHero.BaseAI.isIdle;
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
