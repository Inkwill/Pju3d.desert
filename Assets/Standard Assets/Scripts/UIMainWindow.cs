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

	void Start()
	{
		switch_buttons = new List<Button>();
		targetUI.Init();
		tgDigTool.onValueChanged.AddListener(SetDigTool);
		// GameManager.Player.Detector_item.OnEnter.AddListener(obj => { Debug.Log("OnItemEnter"); });
		// GameManager.Player.Detector_item.OnExit.AddListener(obj => { Debug.Log("OnItemExit"); });
	}
	protected override void OnOpen()
	{
		//SetButton(btDig, false);
		UpdateWeapon(GameManager.Player.Data.Equipment);
		//if (GameManager.Player.Data.Equipment.Weapon == null) GameManager.Player.Data.Equipment.EquipWeapon();
		//SetDig(GameManager.Player.Data.Equipment.Weapon.Stats.Dig > 0);
		//tgDigTool.onValueChanged += value => { GameManager.Player.DigTool.BuildModel = value; };
	}

	void UpdateWeapon(EquipmentSystem equipment)
	{
		btWeapon.gameObject.SetActive(equipment.Weapon || equipment.ViceWeapon);
		btSwitchWeapon.gameObject.SetActive(equipment.ViceWeapon != null);
		iconWeapon.enabled = (equipment.Weapon != null);
		if (equipment.Weapon) iconWeapon.sprite = equipment.Weapon.ItemSprite;
		tgDigTool.gameObject.SetActive(equipment.Weapon && equipment.Weapon.Stats.Dig > 0);
		GameManager.Player.DigTool.BuildModel = tgDigTool.gameObject.activeSelf && tgDigTool.isOn;
	}
	void FixedUpdate()
	{
		infoPos.text = GameManager.Player.gameObject.transform.position.ToString();
		infoTerrian.text = GameManager.Player.DigTool.SceneBoxInfo(true);
		infoTime.text = GameManager.Instance.DayNight.TimeInfo;
		btWeapon.interactable = btSwitchWeapon.interactable = GameManager.Player.canWork;
		//if (tgDig.isOn) SetButton(btDig, GameManager.Player.canWork && GameManager.Player.DigTool.CanDig);
		//else if (btDig.interactable) SetButton(btDig, false);


		// switch (GameManager.Player.DigTool?.SceneBoxInfo(false))
		// {
		// 	case "blank":
		// 		if (GameManager.Player.canWork && tgDig.isOn) SetButton(btDig, true);
		// 		break;
		// case "pit":
		// 	if (GameManager.Player.canWork && !btPlant.interactable) SwitchButton(btPlant);
		// 	break;
		// case "npc":
		// 	if (GameManager.Player.canWork && !btTalk.interactable) SwitchButton(btTalk);
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
				if (GameManager.Player.canWork && GameManager.Player.DigTool.CanDig && GameManager.Player.DigTool.BuildModel) GameManager.Player.DigTool.DoCreate("pit");
				break;
			case "SwitchWeapon":
				GameManager.Player.Data.Equipment.SwitchWeapon();
				UpdateWeapon(GameManager.Player.Data.Equipment);
				SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = SFXManager.ItemEquippedSound });
				break;
			case "camCtrl":
				GameManager.Instance.CameraCtrl.SwitchModel();
				break;
			case "package":
				GameManager.GameUI.SwitchWindow("winInventory");
				break;
			default:
				break;
		}
	}

	// public void SetDig(bool toggle)
	// {
	// 	tgDigTool.gameObject.SetActive(toggle);
	// 	GameManager.Player.DigTool.BuildModel = tgDigTool.isOn && toggle;
	// 	// string wpName = toggle ? "wp_rake" : "wp_unarmed";
	// 	// EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	// GameManager.Player.Data.Equipment.Equip(wp);
	// }
	public void SetDigTool(bool toggle)
	{
		GameManager.Player.DigTool.BuildModel = toggle;
	}
	// public void SetWater(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_water" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	GameManager.Player.Data.Equipment.Equip(wp);
	// }
	// public void SetCut(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_axe" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	GameManager.Player.Data.Equipment.Equip(wp);
	// }
	// public void SetAttack(bool toggle)
	// {
	// 	string wpName = toggle ? "wp_weapon1" : "wp_unarmed";
	// 	EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
	// 	GameManager.Player.Data.Equipment.Equip(wp);
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
	// 	if (wp.Stats.Dig > 0 && GameManager.Player.canWork && GameManager.Player.DigTool.CanDig) GameManager.Player.DigTool.DoCreate("pit");
	// 	// Debug.Log($"Selected content ID: {listBox.ContentID}, Content: {content}");

	// }

	// public void OnFocusingBoxChanged(ListBox prevFocusingBox, ListBox curFocusingBox)
	// {
	// 	// Debug.Log(
	// 	// 	"(Auto updated)\nFocusing content: "
	// 	// 	+ $"{curFocusingBox}");

	// 	UIItemListBox box = (UIItemListBox)curFocusingBox;
	// 	Weapon wp = box?.item as Weapon;
	// 	if (wp && wp != GameManager.Player.Data.Equipment.Weapon)
	// 	{
	// 		GameManager.Player.Data.Equipment.SwitchWeapon(wp);
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
	// 	// GameManager.Player.Data.Equipment.EquipWeapon(wp);

	// }

	// public void OnMovementEnd()
	// {
	// 	//Debug.Log("Movement Ends");
	// }
}
