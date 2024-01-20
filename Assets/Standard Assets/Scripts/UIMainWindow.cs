using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;
using AirFishLab.ScrollingList;

public class UIMainWindow : UIWindow
{
	public Toggle tgDig;
	public Toggle tgDigTool;
	public Toggle tgWater;
	public Toggle tgCut;
	public Toggle tgAttack;

	public Button btDig;

	public Text infoPos;
	public Text infoTerrian;

	[SerializeField]
	private CircularScrollingList m_weaponlist;
	public UICharacterHud monsterHud;

	protected override void OnOpen()
	{
		switch_buttons = new List<Button>();
		m_player.GetComponent<EventSender>()?.events.AddListener(OnPlayerEvent);
		SetButton(btDig, false);
		monsterHud.Show(null);

		tgDig.onValueChanged.AddListener(SetDig);
		tgDigTool.onValueChanged.AddListener(SetDigTool);
		tgWater.onValueChanged.AddListener(SetWater);
		tgCut.onValueChanged.AddListener(SetCut);
		tgAttack.onValueChanged.AddListener(SetAttack);
		//tgDigTool.onValueChanged += value => { m_digtool.BuildModel = value; };
	}

	void OnPlayerEvent(GameObject obj, string eventName)
	{
		if (eventName == "onTarget")
		{
			monsterHud.Show(m_player.CurrentTarget);
		}
	}
	void FixedUpdate()
	{
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool.SceneBoxInfo(true);
		if (tgDig.isOn) SetButton(btDig, m_player.canWork && m_digtool.CanDig);
		else if (btDig.interactable) SetButton(btDig, false);


		// switch (m_digtool?.SceneBoxInfo(false))
		// {
		// 	case "blank":
		// 		if (m_player.canWork && tgDig.isOn) SetButton(btDig, true);
		// 		break;
		// case "pit":
		// 	if (m_player.canWork && !btPlant.interactable) SwitchButton(btPlant);
		// 	break;
		// case "npc":
		// 	if (m_player.canWork && !btTalk.interactable) SwitchButton(btTalk);
		// 	break;
		// default:
		// 		SwitchButton(null);
		// break;
		if (Input.GetMouseButton(1))
		{
			Debug.Log("OnClick winMain!");
		}
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "dig":
				if (m_player.canWork && m_digtool.CanDig) m_digtool.DoCreate("pit");
				break;
			case "switch":
				//if (scroll) scroll.normalizedPosition = scroll.normalizedPosition.x < 0.1 ? new Vector2(1, 0) : new Vector2(0, 0);
				break;
			case "camCtrl":
				m_uiRoot.cameraCtrl.SwitchModel();
				break;
			case "package":
				m_uiRoot.SwitchWindow("winInventory");
				break;
			default:
				break;
		}
	}

	public void SetDig(bool toggle)
	{
		tgDigTool.gameObject.SetActive(toggle);
		m_digtool.BuildModel = tgDigTool.isOn && toggle;
		// string wpName = toggle ? "wp_rake" : "wp_unarmed";
		// EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		// m_player.Data.Equipment.Equip(wp);
	}
	public void SetDigTool(bool toggle)
	{
		m_digtool.BuildModel = toggle;
	}
	public void SetWater(bool toggle)
	{
		string wpName = toggle ? "wp_water" : "wp_unarmed";
		EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		m_player.Data.Equipment.Equip(wp);
	}
	public void SetCut(bool toggle)
	{
		string wpName = toggle ? "wp_axe" : "wp_unarmed";
		EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		m_player.Data.Equipment.Equip(wp);
	}
	public void SetAttack(bool toggle)
	{
		string wpName = toggle ? "wp_weapon1" : "wp_unarmed";
		EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		m_player.Data.Equipment.Equip(wp);
	}

	// public void DisplayFocusingContent()
	// {
	// 	var contentID = m_weaponlist.GetFocusingContentID();
	// 	var centeredContent =
	// 		m_weaponlist.ListBank.GetListContent(contentID);
	// 	Debug.Log(
	// 		$"Focusing content: {centeredContent}");
	// }

	public void OnBoxSelected(ListBox listBox)
	{
		var content = m_weaponlist.ListBank.GetListContent(listBox.ContentID);
		Weapon wp = content as Weapon;
		if (wp.Stats.Dig > 0 && m_player.canWork && m_digtool.CanDig) m_digtool.DoCreate("pit");
		// Debug.Log($"Selected content ID: {listBox.ContentID}, Content: {content}");

	}

	public void OnFocusingBoxChanged(ListBox prevFocusingBox, ListBox curFocusingBox)
	{
		// Debug.Log(
		// 	"(Auto updated)\nFocusing content: "
		// 	+ $"{curFocusingBox}");
		UIItemListBox box = (UIItemListBox)curFocusingBox;
		Weapon wp = box?.item as Weapon;
		if (wp == null)
		{
			wp = Resources.Load<Weapon>("wp_unarmed");
		}
		else if (wp.WorldObjectPrefab == null)
		{
			wp.WorldObjectPrefab = Resources.Load("Unarmed") as GameObject;

		}
		m_player.Data.Equipment.Equip(wp);
		SetDig(wp.Stats.Dig > 0);
	}

	public void OnMovementEnd()
	{
		//Debug.Log("Movement Ends");
	}
}
