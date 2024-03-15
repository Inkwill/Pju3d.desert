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
	Transform m_uiGridRoot;
	IInteractable m_interactTarget;

	protected override void OnOpen()
	{
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("down");
		if (m_interactTarget is InteractPit)
		{
			var udgdEntries = GameManager.CurHero.Inventory.Entries.Where(en => en != null && en.Item is UndergroundItem).ToArray();
			var elements = Helpers.AdjustElements<UIElementBase>(m_uiGridRoot, udgdEntries.Length, m_uiGridBox);
			for (int i = 0; i < elements.Length; i++)
			{
				UIElementBase element = elements[i].GetComponent<UIElementBase>();
				UndergroundItem udgdItem = udgdEntries[i].Item as UndergroundItem;
				element.icon.sprite = udgdItem.ItemSprite;
				element.count.text = udgdEntries[i].Count.ToString();
				element.GetComponent<Button>()?.onClick.AddListener(() => Close());
			}
		}
	}

	protected override void OnClose()
	{
		GameManager.GameUI.WinMain.bottomRoot.SetTrigger("up");
	}

	public void Init(IInteractable target, InventorySystem.ItemDemand demand = null)
	{
		m_interactTarget = target;
		Name.text = target.Data.Key;
		icon.sprite = target.Data.icon;
		Desc.text = target.Data.Description;
		if (demand != null) uiDemand?.Show(demand);
	}

}
