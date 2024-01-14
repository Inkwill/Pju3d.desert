using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{
	public Button btDig;
	public Toggle tgDigTool;

	public Button btPlant;

	public Button btTalk;

	public Text infoPos;
	public Text infoTerrian;


	protected override void OnOpen()
	{
		switch_buttons = new List<Button>();
		switch_buttons.Add(btPlant);
		switch_buttons.Add(btTalk);

		SetButton(btPlant, false);
		SetButton(btTalk, false);

		tgDigTool.onValueChanged.AddListener(SetDigTool);
	}
	void FixedUpdate()
	{
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool.SceneBoxInfo(true);
		SetButton(btDig, m_digtool.CanDig);
		switch (m_digtool?.SceneBoxInfo(false))
		{
			// case "blank":
			// 	if (m_player.canWork && !btDig.interactable) SwitchButton(btDig);
			// 	break;
			case "pit":
				if (m_player.canWork && !btPlant.interactable) SwitchButton(btPlant);
				break;
			case "npc":
				if (m_player.canWork && !btTalk.interactable) SwitchButton(btTalk);
				break;
			default:
				SwitchButton(null);
				break;
		}
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "dig":
				if (m_digtool.CanDig) m_digtool.DoCreate("pit");
				break;
			case "plant":
				m_uiRoot.SwitchWindow("winPlant");
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

	public void SetDigTool(bool toggle)
	{
		m_digtool.BuildModel = toggle;
	}
}
