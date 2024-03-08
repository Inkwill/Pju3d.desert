using System.Collections;
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
	public Slider lordExp;
	public Text lordLv;
	public Text lordMoney;
	public Text infoPos;
	public Text infoTerrian;
	public Text infoTime;
	public Text textTellContent;
	public GameObject tellContent;
	public UITargetInfo targetUI;
	public Skill sprintSkill;
	public UIGoalInfo uiGoalInfo;
	public Image iconExp;
	public Slider sliderWater;
	Button[] m_buttons;
	UISkillButton[] m_skillButtons;

	void Start()
	{
		targetUI.Init();
		m_buttons = GetComponentsInChildren<Button>();
		m_skillButtons = GetComponentsInChildren<UISkillButton>();
		GameManager.CurHero.Equipment.OnEquiped += (equip) => { UpdateWeapon(GameManager.CurHero.Equipment); };
		GameManager.CurHero.Equipment.OnUnequip += (equip) => { UpdateWeapon(GameManager.CurHero.Equipment); };
		GameManager.CurHero.Equipment.OnEquipViceWeapon += (equip) => { UpdateWeapon(GameManager.CurHero.Equipment); };
		GameManager.StoryListener.storyListenerEvents.AddListener(OnStoryListenerEvent);
		GameManager.GameGoal.GameGoalAction += UpdateGoalInfo;
	}
	protected override void OnOpen()
	{
		UpdateWeapon(GameManager.CurHero.Equipment);
		CloseTalkButton();
		tellContent.SetActive(false);
		uiGoalInfo.SetGoal(GameManager.GameGoal.CurrentGoal);
	}

	void OnStoryListenerEvent(string eventName, string content)
	{
		if (eventName == "listenerEvent_Start")
		{
			GameManager.Instance.CameraCtrl.SetMode(CameraController.Mode.STORY);
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
		if (eventName == "listenerEvent_Stop")
		{
			GameManager.Instance.CameraCtrl.SetMode(CameraController.Mode.RPG);
			GameManager.StoryListener.CurrentTeller.tellerEvent.RemoveListener(OnStoryTellerEvent);
			foreach (var bt in m_buttons)
			{
				bt.interactable = true;
			}
			foreach (var bt in m_skillButtons)
			{
				bt.gameObject.SetActive(true);
			}
			CloseTalkButton();
			tellContent.SetActive(false);
		}
		if (eventName == "listenerEvent_Ask")
		{
			CloseTalkButton();
			Helpers.Log(this, eventName, "ask: " + content);
		}
	}

	void OnStoryTellerEvent(StoryTeller teller, string content)
	{
		Helpers.Log(this, "StoryTellerEvent", "content= " + content);
		textTellContent.text = content;
		tellContent.SetActive(true);
		UpdateTalkButton(teller.CurrentNode, content);
	}

	void UpdateWeapon(EquipmentSystem equipment)
	{
		btWeapon.gameObject.SetActive(equipment.Weapon && equipment.Weapon != GameManager.CurHero.DefaultWeapon && equipment.Weapon.WeaponSkill != null);
		btSwitchWeapon.gameObject.SetActive(equipment.ViceWeapon != null);
		iconWeapon.enabled = (btWeapon.gameObject.activeSelf);
		if (btWeapon.gameObject.activeSelf)
		{
			btWeapon.Init(equipment.Weapon.WeaponSkill);
			iconWeapon.sprite = equipment.Weapon.ItemSprite;
		}
	}

	void UpdateGoalInfo(GameGoalSystem.GameGoal goal, string eventName)
	{
		if (eventName == "AddGoal")
		{
			uiGoalInfo.SetGoal(goal);
		}
		if (eventName == "UpdateGoal")
		{
			uiGoalInfo.UpdateGoal();
		}
		if (eventName == "AchieveGoal")
		{
			uiGoalInfo.SetGoal(null);
		}
	}

	void UpdateTalkButton(StoryNode node, string content = "")
	{
		if (node != null)
		{
			string[] talkContents = node.GetListenerTalk(content, GameManager.StoryListener.LastStoryAsk(node));
			btTalks[0].Show(talkContents[0]);
			btTalks[1].Show(talkContents[1]);
		}
		else
		{
			CloseTalkButton();
		}

	}

	void CloseTalkButton()
	{
		btTalks[0].gameObject.SetActive(false);
		btTalks[1].gameObject.SetActive(false);
	}
	void Update()
	{
		lordExp.value = GameManager.Lord.ExpPercent;
		lordLv.text = $"Lv.{GameManager.Lord.Lv}";
		lordMoney.text = GameManager.Lord.Money.ToString();
		infoPos.text = GameManager.CurHero.gameObject.transform.position.ToString();
		infoTerrian.text = GameManager.CurHero.BaseAI.SceneBoxName;
		infoTime.text = GameManager.Instance.DayNight.TimeInfo;
		btSwitchWeapon.interactable = GameManager.CurHero.BaseAI.isStandBy;
		sliderWater.value = GameManager.Lord.waterBottle.Volume;
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "SwitchWeapon":
				GameManager.CurHero.Equipment.SwitchWeapon();
				//UpdateWeapon(GameManager.CurHero.Equipment);
				break;
			case "package":
				GameManager.GameUI.SwitchWindow("winInventory");
				break;
			case "talk":
				GameManager.GameUI.SwitchWindow("winTalk");
				break;
			case "camMode":
				GameManager.BuildMode = !GameManager.BuildMode;
				break;
			default:
				break;
		}
	}
}
