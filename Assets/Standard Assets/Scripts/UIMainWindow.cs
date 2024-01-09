using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIMainWindow : UIWindow
{
	public Text infoPos;
	public Text infoTerrian;

	public Button btDig;

	public Button btPlant;

	public Button btTalk;

	List<Button> switch_buttons;


	public override void OnOpen()
	{
		switch_buttons = new List<Button>();
		switch_buttons.Add(btDig);
		switch_buttons.Add(btPlant);
		switch_buttons.Add(btTalk);

		SetButton(btDig, false);
		SetButton(btPlant, false);
		SetButton(btTalk, false);
	}
	void FixedUpdate()
	{
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool?.SceneBoxInfo(true);
		switch (m_digtool?.SceneBoxInfo(false))
		{
			case null:
				if (m_player.canWork && !btDig.interactable) SwitchButton(btDig);
				break;
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

	public void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "dig":
				m_digtool.DoCreate("pit");
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

	void SetButton(Button bt, bool active)
	{
		if (bt.interactable == active) return;

		bt.interactable = active;
		string anim = active ? "move_up" : "move_down";
		Animator animator = bt.GetComponent<Animator>();
		if (animator) animator.Play(anim);

		// Image[] imgs = bt.gameObject.GetComponentsInChildren<Image>();
		// foreach (Image img in imgs)
		// {
		// 	img.color = active ? Color.red : Color.gray;
		// }
	}

	void SwitchButton(Button bt)
	{
		foreach (Button button in switch_buttons)
		{
			SetButton(button, bt == button);
		}
	}
}
