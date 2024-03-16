using System.Collections.Generic;
using TMPro;
using System.Linq;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

public class UIInteractWindow : UIWindow
{
	public Text Name;
	public Image icon;
	public Text Desc;
	public Button bt_Confirm;
	public UIItemDemand uiDemand;
	[SerializeField]
	GameObject m_uiGridBox;
	[SerializeField]
	ToggleGroup m_uiGridRoot;
	[SerializeField]
	TMP_Text promptText;
	IInteractable m_interactor;
	List<InventorySystem.ItemDemand> m_demands;

	public int SelectedIndex { get { return m_selectedIndex; } }
	int m_selectedIndex;

	public void Init(IInteractable interactor, List<InventorySystem.ItemDemand> demands)
	{
		m_interactor = interactor;
		m_demands = demands;
	}
	protected override void OnOpen()
	{
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("down");
		if (m_interactor.Data.Type == InteractData.InteractType.DeviceFixer || m_interactor.Data.Type == InteractData.InteractType.DeviceCreater)
		{
			var devices = m_interactor.Data.devices.Value;
			var elements = Helpers.AdjustElements<UIElementBase>(m_uiGridRoot.transform, devices.Length, m_uiGridBox);
			for (int i = 0; i < elements.Length; i++)
			{
				UIElementBase element = elements[i].GetComponent<UIElementBase>();
				element.icon.sprite = devices[i].ItemSprite;
				element.toggle.group = m_uiGridRoot;
				element.toggle.onValueChanged.AddListener((value) => { if (value) UpdateInfo(i); });
			}
			if (elements.Length > 0) UpdateInfo(0);
		}
	}

	public void UpdateInfo(int selected)
	{
		m_selectedIndex = selected;
		var item = m_interactor.Data.devices.Value[selected];
		var demand = m_demands[selected];
		Name.text = item.ItemName;
		icon.sprite = item.ItemSprite;
		Desc.text = item.Description;
		promptText.text = demand.canCompleted(GameManager.CurHero.Inventory) ? m_interactor.Data.BehavePrompt : "Submit";
		bt_Confirm.interactable = demand.Submittable(GameManager.CurHero.Inventory).Values.Sum() > 0;
		uiDemand.Show(demand);
	}

	protected override void OnClose()
	{
		bt_Confirm.onClick.RemoveAllListeners();
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("up");
	}
}
