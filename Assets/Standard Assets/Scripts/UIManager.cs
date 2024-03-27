using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
	public VariableJoystick JoyStick;
	public Canvas DragCanvas;
	public UIRpgWindow winRpg => m_winDic["winRpg"] as UIRpgWindow;
	Dictionary<string, UIWindow> m_winDic = new Dictionary<string, UIWindow>();

	public Action<UIWindow> winOpenAction;
	public Action<UIWindow> winCloseAction;

	public UIWindow win_LastOpen;
	public UIWindow win_LastClose;
	float during_check;

	public UIWindow GetWindow(string winName)
	{
		if (m_winDic.ContainsKey(winName))
		{
			return m_winDic[winName];
		}

		UIWindow newWindow = Resources.Load<UIWindow>(winName);
		if (newWindow)
		{
			newWindow = Instantiate(newWindow, transform);
			m_winDic.Add(winName, newWindow);
			newWindow.gameObject.SetActive(false);
		}
		return newWindow;
	}
	public UIWindow OpenWindow(string winName)
	{
		win_LastOpen = GetWindow(winName);
		win_LastOpen.Open();
		return win_LastOpen;
	}
	public UIWindow OpenWindow(UIWindow win)
	{
		win_LastOpen = win;
		win_LastOpen.Open();
		if (!m_winDic.ContainsKey(win.winName)) m_winDic.Add(win.winName, win);
		return win_LastOpen;
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
			else m_winDic[winName].Open();
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
