using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{
	public Button btPackage;
	public UISkillButton btWeapon;
	public Button btSwitchWeapon;
	public Image iconWeapon;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public UITargetInfo targetUI;
	public Skill sprintSkill;
	Button[] m_buttons;
	UISkillButton[] m_skillButtons;

	void Start()
	{
		targetUI.Init();
		m_buttons = GetComponentsInChildren<Button>();
		m_skillButtons = GetComponentsInChildren<UISkillButton>();
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.StoryListener.ListenerEvents.AddListener(OnStoryListenEvent);
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

	void OnStoryListenEvent(string eventName)
	{
		if (eventName == "StartListening")
		{
			GameManager.Instance.CameraCtrl.CloseTo();
			foreach (var bt in m_buttons)
			{
				if (bt != btPackage) bt.interactable = false;
			}
			foreach (var bt in m_skillButtons)
			{
				bt.gameObject.SetActive(false);
			}
		}
		if (eventName == "StopListening")
		{
			GameManager.Instance.CameraCtrl.Reset();
			foreach (var bt in m_buttons)
			{
				bt.interactable = true;
			}
			foreach (var bt in m_skillButtons)
			{
				bt.gameObject.SetActive(true);
			}
		}
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
			case "talk":
				GameManager.GameUI.SwitchWindow("winTalk");
				break;
			default:
				break;
		}
	}
}
