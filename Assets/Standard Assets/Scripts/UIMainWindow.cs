using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{

	public Button btWeapon;
	public Button btSwitchWeapon;
	public Image iconWeapon;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public UITargetInfo targetUI;
	public Skill sprintSkill;

	void Start()
	{
		switch_buttons = new List<Button>();
		targetUI.Init();
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.Player.SkillUser.AddSkill(sprintSkill);
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
		btWeapon.gameObject.SetActive(equipment.Weapon || equipment.ViceWeapon);
		btSwitchWeapon.gameObject.SetActive(equipment.ViceWeapon != null);
		iconWeapon.enabled = (equipment.Weapon != null);
		if (equipment.Weapon) iconWeapon.sprite = equipment.Weapon.ItemSprite;
	}
	void FixedUpdate()
	{
		infoPos.text = GameManager.Player.gameObject.transform.position.ToString();
		infoTerrian.text = GameManager.SceneBoxInfo(true);
		infoTime.text = GameManager.Instance.DayNight.TimeInfo;
		btWeapon.interactable = btSwitchWeapon.interactable = GameManager.Player.isIdle;
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "UseWeapon":
				//GameManager.Player.UseSkill(digSkill);
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
			case "roleSkill":
				GameManager.Player.SkillUser.UseSkill(sprintSkill, GameManager.Player.gameObject);
				break;
			default:
				break;
		}
	}
}
