using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{
	public Button btBuild;

	public Button btPlant;

	public Button btTalk;


	protected override void OnOpen()
	{
		switch_buttons = new List<Button>();
		switch_buttons.Add(btPlant);
		switch_buttons.Add(btTalk);

		SetButton(btBuild, true);
		SetButton(btPlant, false);
		SetButton(btTalk, false);
	}
	void FixedUpdate()
	{
		switch (m_digtool?.SceneBoxInfo(false))
		{
			// case null:
			// 	if (m_player.canWork && !btBuild.interactable) SwitchButton(btBuild);
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
			case "build":
				m_uiRoot.SwitchWindow("winBuild");
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
}
