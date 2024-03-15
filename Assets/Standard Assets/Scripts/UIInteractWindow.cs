using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractWindow : UIWindow
{
	public Button bt_interact;
	public Text Name;
	public Image icon;
	public Text Desc;
	public UIItemDemand uiDemand;
	[SerializeField]
	GameObject m_uiGridBox;
	[SerializeField]
	ToggleGroup m_uiGridRoot;
	[SerializeField]
	Button bt_Confirm;
	IInteractable m_interactor;
	public Item SelectedItem { get { return m_selected; } }
	Item m_selected;

	protected override void OnOpen()
	{
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("down");
		if (m_interactor.Data.Type == InteractData.InteractType.DeviceCreater)
		{
			var dvEntries = GameManager.CurHero.Inventory.Entries.Where(en => en != null && en.Item is DeviceItem).ToArray();
			var elements = Helpers.AdjustElements<UIElementBase>(m_uiGridRoot.transform, dvEntries.Length, m_uiGridBox);
			for (int i = 0; i < elements.Length; i++)
			{
				UIElementBase element = elements[i].GetComponent<UIElementBase>();
				DeviceItem device = dvEntries[i].Item as DeviceItem;
				element.icon.sprite = device.ItemSprite;
				element.count.text = dvEntries[i].Count.ToString();
				element.toggle.group = m_uiGridRoot;
				element.toggle.onValueChanged.AddListener((value) => { if (value) UpdateInfo(device, m_interactor.Data.demands[0].Instance()); });
				element.GetComponent<Button>()?.onClick.AddListener(() => Close());
			}
		}
	}

	public void UpdateInfo(DeviceItem device, InventorySystem.ItemDemand demand)
	{
		m_selected = device;
		Name.text = m_selected.ItemName;
		icon.sprite = m_selected.ItemSprite;
		Desc.text = m_selected.Description;
		bt_Confirm.interactable = true;
		uiDemand.Show(demand);
	}

	protected override void OnClose()
	{
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("up");
	}

	public void Init(IInteractable interactor)
	{
		m_interactor = interactor;
	}

}
