using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
	public VariableJoystick JoyStick;

	public Canvas DragCanvas;

	public UIMainWindow WinMain => m_winDic["winMain"] as UIMainWindow;
	Dictionary<string, UIWindow> m_winDic = new Dictionary<string, UIWindow>();

	public UIWindow win_LastOpen;
	public UIWindow win_LastClose;
	float during_check;
	public UIWindow OpenWindow(string winName)
	{
		if (m_winDic.ContainsKey(winName))
		{
			m_winDic[winName].gameObject.SetActive(true);
			win_LastOpen = m_winDic[winName];
			return m_winDic[winName];
		}

		UIWindow newWindow = Resources.Load<UIWindow>(winName);
		if (newWindow)
		{
			newWindow = Instantiate(newWindow, transform);
			newWindow.gameObject.name = newWindow.winName = winName;
			m_winDic.Add(winName, newWindow);
			win_LastOpen = newWindow;
		}
		return newWindow;
	}

	public void CloseAll()
	{
		foreach (var win in m_winDic.Values)
		{
			if (win.gameObject.activeSelf) win.Close();
		}
	}

	public void SwitchWindow(string winName)
	{
		if (m_winDic.ContainsKey(winName))
		{
			if (m_winDic[winName].gameObject.activeSelf) m_winDic[winName].Close();
			else m_winDic[winName].gameObject.SetActive(true);
		}
		else OpenWindow(winName);

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
