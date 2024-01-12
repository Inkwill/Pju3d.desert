using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildWindow : UIWindow
{
	public Text infoPos;
	public Text infoTerrian;
	public Button btPit;

	protected override void OnOpened()
	{
		m_digtool.BuildModel = true;
		// objList.SetActive(true);
		// btConfirm.interactable = false;
		SetButton(btPit, true);
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
		infoPos.text = m_player.gameObject.transform.position.ToString();
		infoTerrian.text = m_digtool.SceneBoxInfo(true);
		// switch (m_digtool.SceneBoxInfo(false))
		// {
		// 	case null:
		// 		if (m_player.canWork && !btConfirm.interactable) SetButton(btConfirm, true);
		// 		break;
		// 	default:
		// 		SetButton(btConfirm, false);
		// 		break;
		// }
	}

	public override void OnButtonClick(string eventName)
	{
		switch (eventName)
		{
			case "pit":
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
