using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{

	public UISkillButton btWeapon;
	public Button btSwitchWeapon;
	public Image iconWeapon;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public UITargetInfo targetUI;
	public Skill sprintSkill;

	void Start()
	{
		targetUI.Init();
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
	}
	protected override void OnOpen()
	{
		UpdateWeapon(GameManager.Player.Data.Equipment);
		//if (GameManager.Player.Data.Equipment.Weapon == null) GameManager.Player.Data.Equipment.EquipWeapon();
	}

	void OnPlayerEvent(GameObject obj, string eventName)
	{
		//btDig.interactable = (eventName == "roleEvent_OnState_IDLE");
	}

	void UpdateWeapon(EquipmentSystem equipment)
	{
		btWeapon.gameObject.SetActive(equipment.Weapon && equipment.Weapon != GameManager.Player.DefaultWeapon);
		btSwitchWeapon.gameObject.SetActive(equipment.ViceWeapon != null);
		iconWeapon.enabled = (btWeapon.gameObject.activeSelf);
		if (btWeapon.gameObject.activeSelf)
		{
			btWeapon.Init(equipment.Weapon.WeaponSkill);
			iconWeapon.sprite = equipment.Weapon.ItemSprite;
		}
	}
	void FixedUpdate()
	{
		infoPos.text = GameManager.Player.gameObject.transform.position.ToString();
		infoTerrian.text = GameManager.Player.BaseAI.SceneBoxName;
		infoTime.text = GameManager.Instance.DayNight.TimeInfo;
		btSwitchWeapon.interactable = GameManager.Player.isStandBy;
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
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
}
