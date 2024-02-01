using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildWindow : UIWindow
{

	// public Button btConfirm;

	// protected override void OnOpen()
	// {
	// 	SetButton(btConfirm, false);

	// }
	// protected override void OnOpened()
	// {
	// 	GameManager.Player.DigTool.BuildModel = true;
	// 	// objList.SetActive(true);
	// 	// btConfirm.interactable = false;
	// 	base.OnOpened();
	// }

	// protected override void OnClose()
	// {
	// 	GameManager.Player.DigTool.BuildModel = false;
	// }

	// void FixedUpdate()
	// {
	// 	switch (GameManager.Player.DigTool.SceneBoxInfo(false))
	// 	{
	// 		case "blank":
	// 			if (GameManager.Player.canWork && !btConfirm.interactable) SetButton(btConfirm, true);
	// 			break;
	// 		default:
	// 			SetButton(btConfirm, false);
	// 			break;
	// 	}
	// }

	// public override void OnButtonClick(string eventName)
	// {
	// 	switch (eventName)
	// 	{
	// 		case "confirm":
	// 			if (GameManager.Player.DigTool.CanDig) GameManager.Player.DigTool.DoCreate("pit");
	// 			break;
	// 		case "place":
	// 			break;
	// 		case "build":
	// 			break;
	// 		default:
	// 			break;
	// 	}
	// }
}
