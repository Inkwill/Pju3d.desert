using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildWindow : UIWindow
{

	public Button btConfirm;

	protected override void OnOpen()
	{
		SetButton(btConfirm, false);

	}
	protected override void OnOpened()
	{
		m_digtool.BuildModel = true;
		// objList.SetActive(true);
		// btConfirm.interactable = false;
		base.OnOpened();
	}

	protected override void OnClose()
	{
		m_digtool.BuildModel = false;
	}
	protected override void OnClosed()
	{
		m_digtool.BuildModel = false;
		//SetButton(btConfirm, false);
		base.OnClosed();
	}

	void FixedUpdate()
	{
		switch (m_digtool.SceneBoxInfo(false))
		{
			case "blank":
				if (m_player.canWork && !btConfirm.interactable) SetButton(btConfirm, true);
				break;
			default:
				SetButton(btConfirm, false);
				break;
		}
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "confirm":
				if (m_digtool.CanDig) m_digtool.DoCreate("pit");
				break;
			case "place":
				break;
			case "build":
				break;
			default:
				break;
		}
	}
}
