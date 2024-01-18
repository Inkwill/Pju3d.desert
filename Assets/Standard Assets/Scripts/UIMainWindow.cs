using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{
	public Toggle tgDig;
	public Toggle tgDigTool;
	public Toggle tgWater;

	public Button btDig;

	public Text infoPos;
	public Text infoTerrian;

	public ScrollRect scroll;

	protected override void OnOpen()
	{
		switch_buttons = new List<Button>();
		//switch_buttons.Add(btPlant);
		//switch_buttons.Add(btTalk);

		SetButton(btDig, false);

		tgDig.onValueChanged.AddListener(SetDig);
		tgDigTool.onValueChanged.AddListener(SetDigTool);
		tgWater.onValueChanged.AddListener(SetWater);
		//tgDigTool.onValueChanged += value => { m_digtool.BuildModel = value; };
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
				if (scroll) scroll.normalizedPosition = scroll.normalizedPosition.x < 0.1 ? new Vector2(1, 0) : new Vector2(0, 0);
				break;
			case "camCtrl":
				m_uiRoot.cameraCtrl.SwitchModel();
				break;
			case "talk":
				m_uiRoot.SwitchWindow("winTalk");
				break;
			default:
				break;
		}
	}

	public void SetDig(bool toggle)
	{
		tgDigTool.gameObject.SetActive(toggle);
		m_digtool.BuildModel = tgDigTool.isOn && toggle;
		string wpName = toggle ? "wp_rake" : "wp_unarmed";
		EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		m_player.Data.Equipment.Equip(wp);
	}
	public void SetDigTool(bool toggle)
	{
		m_digtool.BuildModel = toggle;
	}
	public void SetWater(bool toggle)
	{
		string wpName = toggle ? "wp_unarmed" : "wp_rake";
		EquipmentItem wp = Resources.Load<EquipmentItem>(wpName);
		m_player.Data.Equipment.Equip(wp);
	}
}
