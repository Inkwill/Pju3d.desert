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
	public UITalkButton[] btTalks = new UITalkButton[2];
	public Image iconWeapon;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public Text textTellContent;
	public GameObject tellContent;
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
		UpdateTalkButton(null);
		tellContent.SetActive(false);
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
			GameManager.StoryListener.CurrentTeller.tellerEvent.AddListener(OnStoryTellerEvent);
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
			GameManager.StoryListener.CurrentTeller.tellerEvent.RemoveListener(OnStoryTellerEvent);
			foreach (var bt in m_buttons)
			{
				bt.interactable = true;
			}
			foreach (var bt in m_skillButtons)
			{
				bt.gameObject.SetActive(true);
			}
			UpdateTalkButton(null);
			tellContent.SetActive(false);
		}
	}

	void OnStoryTellerEvent(StoryTeller teller, string eventName)
	{
		Helpers.Log(this, "StoryTellerEvent", "event= " + eventName);
		textTellContent.text = teller.CurrentNode.Content[0].Key;
		tellContent.SetActive(true);
		UpdateTalkButton(teller);
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

	void UpdateTalkButton(StoryTeller teller)
	{
		if (teller != null)
		{
			string[] talkContents = teller.CurrentNode.GetListenerTalk();
			btTalks[0].Show(this, talkContents[0]);
			btTalks[1].Show(this, talkContents[1]);
		}
		else
		{
			btTalks[0].gameObject.SetActive(false);
			btTalks[1].gameObject.SetActive(false);
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

	public void OnTalk(string content)
	{
		Helpers.Log(this, "OnTalk", "content= " + content);
	}
}
