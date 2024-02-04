using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;

namespace CreatorKitCodeInternal
{
	public class ItemTooltip : MonoBehaviour
	{
		public Text Name;
		public Text DescriptionText;
		public Button btEquip;
		public Button btUnEquip;
		public Button btDrop;
		public Button btUse;


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
				btEquip.gameObject.SetActive(!slot.equipment && equItem);
				btUnEquip.gameObject.SetActive(slot.equipment);
				btDrop.gameObject.SetActive(!slot.equipment);
				btUse.gameObject.SetActive(useItem);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
