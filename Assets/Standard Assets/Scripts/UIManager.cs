using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public VariableJoystick JoyStick;

	public Canvas DragCanvas;

	public List<UIWindow> m_winList;

	public UIWindow win_LastOpen;
	public UIWindow win_LastClose;

	float during_check;

	// void FixedUpdate()
	// {
	// 	if (win_LastOpen)
	// 	{
	// 		during_check += Time.deltaTime;
	// 		if (during_check >= 2.0f)
	// 		{
	// 			if (!win_LastOpen.gameObject.activeSelf)
	// 				win_LastOpen.gameObject.SetActive(true);
	// 			during_check = 0;
	// 		}
	// 	}
	// }
	public UIWindow OpenWindow(string winName)
	{
		//Debug.Log("OpenWindow: " + winName);
		foreach (UIWindow win in m_winList)
		{
			if (win.gameObject.name == winName)
			{
				win.gameObject.SetActive(true);
				win_LastOpen = win;
				return win;
			}
		}

		UIWindow newWindow = Resources.Load<UIWindow>(winName);
		if (newWindow)
		{
			newWindow = Instantiate(newWindow, transform);
			newWindow.gameObject.name = newWindow.winName = winName;
			m_winList.Add(newWindow);
			win_LastOpen = newWindow;
		}
		return newWindow;
	}

	public UIWindow SwitchWindow(string winName)
	{
		if (win_LastOpen)
		{
			win_LastOpen.Close();
			win_LastClose = win_LastOpen;
		}
		return OpenWindow(winName);
	}

	public void BackWindow(UIWindow curWindow)
	{
		if (win_LastClose)
		{
			OpenWindow(win_LastClose.gameObject.name);
		}
		curWindow.Close();
		win_LastClose = curWindow;
	}
	public void BackWindow()
	{
		BackWindow(win_LastOpen);
	}
}
