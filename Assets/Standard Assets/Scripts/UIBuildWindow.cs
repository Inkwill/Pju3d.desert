using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildWindow : UIWindow
{
	public Text infoPos;
	public Text infoTerrian;
	public Button btDig;

	protected override void OnOpen()
	{
		m_digtool.BuildModel = true;
	}

	protected override void OnClose()
	{
		m_digtool.BuildModel = false;
	}

	void FixedUpdate()
	{
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool.SceneBoxInfo(true);
		switch (m_digtool.SceneBoxInfo(false))
		{
			case null:
				if (m_player.canWork && !btDig.interactable) SetButton(btDig, true);
				break;
			default:
				SetButton(btDig, false);
				break;
		}
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "dig":
				m_digtool.DoCreate("pit");
				break;
			default:
				break;
		}
	}
}
