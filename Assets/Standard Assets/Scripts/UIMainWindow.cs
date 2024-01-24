using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;
using AirFishLab.ScrollingList;

public class UIMainWindow : UIWindow
{
	public Toggle tgDigTool;
	public Button btWeapon;
	public Button btSwitchWeapon;
	public Image iconWeapon;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public UITargetInfo targetUI;

	[SerializeField]
	AudioClip SwitchWeaponClip;


	protected override void Init()
	{
		switch_buttons = new List<Button>();
		targetUI.Init(m_player);
		tgDigTool.onValueChanged.AddListener(SetDigTool);
	}
	protected override void OnOpen()
	{
		//SetButton(btDig, false);
		UpdateWeapon(m_player.Data.Equipment);
		//if (m_player.Data.Equipment.Weapon == null) m_player.Data.Equipment.EquipWeapon();
		//SetDig(m_player.Data.Equipment.Weapon.Stats.Dig > 0);
		//tgDigTool.onValueChanged += value => { m_digtool.BuildModel = value; };
	}

	void UpdateWeapon(EquipmentSystem equipment)
	{
		btWeapon.gameObject.SetActive(equipment.Weapon || equipment.ViceWeapon);
		btSwitchWeapon.gameObject.SetActive(equipment.ViceWeapon != null);
		iconWeapon.enabled = (equipment.Weapon != null);
		if (equipment.Weapon) iconWeapon.sprite = equipment.Weapon.ItemSprite;
		tgDigTool.gameObject.SetActive(equipment.Weapon && equipment.Weapon.Stats.Dig > 0);
		m_digtool.BuildModel = tgDigTool.gameObject.activeSelf && tgDigTool.isOn;
	}
	void FixedUpdate()
	{
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool.SceneBoxInfo(true);
		infoTime.text = m_uiRoot.dayNight.TimeInfo;
		btWeapon.interactable = btSwitchWeapon.interactable = m_player.canWork;
		//if (tgDig.isOn) SetButton(btDig, m_player.canWork && m_digtool.CanDig);
		//else if (btDig.interactable) SetButton(btDig, false);


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
			case "UseWeapon":
				if (m_player.canWork && m_digtool.CanDig && m_digtool.BuildModel) m_digtool.DoCreate("pit");
				break;
			case "SwitchWeapon":
				m_player.Data.Equipment.SwitchWeapon();
				UpdateWeapon(m_player.Data.Equipment);
				if (SwitchWeaponClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SwitchWeaponClip });
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

	// public void SetDig(bool toggle)
	// {
	// 	tgDigTool.gameObject.SetActive(toggle);
	// 	m_digtool.BuildModel = tgDigTool.isOn && toggle;
	// 	// string wpName = toggle ? "wp_rake" : "wp_unarmed";
	// 	// EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	// m_player.Data.Equipment.Equip(wp);
	// }
	public void SetDigTool(bool toggle)
	{
		m_digtool.BuildModel = toggle;
	}
	// public void SetWater(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_water" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	m_player.Data.Equipment.Equip(wp);
	// }
	// public void SetCut(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_axe" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	m_player.Data.Equipment.Equip(wp);
	// }
	// public void SetAttack(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_weapon1" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	m_player.Data.Equipment.Equip(wp);
	// }

	// public void DisplayFocusingContent()
	// {
	// 	var contentID = m_weaponlist.GetFocusingContentID();
	// 	var centeredContent =
	// 		m_weaponlist.ListBank.GetListContent(contentID);
	// 	Debug.Log(
	// 		$"Focusing content: {centeredContent}");
	// }

	// public void OnBoxSelected(ListBox listBox)
	// {
	// 	var content = m_weaponlist.GetListContent(listBox.ContentID);
	// 	Weapon wp = content as Weapon;
	// 	if (wp.Stats.Dig > 0 && m_player.canWork && m_digtool.CanDig) m_digtool.DoCreate("pit");
	// 	// Debug.Log($"Selected content ID: {listBox.ContentID}, Content: {content}");

	// }

	// public void OnFocusingBoxChanged(ListBox prevFocusingBox, ListBox curFocusingBox)
	// {
	// 	// Debug.Log(
	// 	// 	"(Auto updated)\nFocusing content: "
	// 	// 	+ $"{curFocusingBox}");

	// 	UIItemListBox box = (UIItemListBox)curFocusingBox;
	// 	Weapon wp = box?.item as Weapon;
	// 	if (wp && wp != m_player.Data.Equipment.Weapon)
	// 	{
	// 		m_player.Data.Equipment.SwitchWeapon(wp);
	// 	}
	// 	if (wp) SetDig(wp.Stats.Dig > 0);

	// 	// if (wp == null)
	// 	// {
	// 	// 	wp = Resources.Load<Weapon>("wp_unarmed");
	// 	// }
	// 	// else if (wp.WorldObjectPrefab == null)
	// 	// {
	// 	// 	wp.WorldObjectPrefab = Resources.Load("Unarmed") as GameObject;

	// 	// }
	// 	// m_player.Data.Equipment.EquipWeapon(wp);

	// }

	// public void OnMovementEnd()
	// {
	// 	//Debug.Log("Movement Ends");
	// }
}
