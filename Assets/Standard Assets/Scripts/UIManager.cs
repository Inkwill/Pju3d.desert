using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.UI;
using System.Linq;
using System;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
	public VariableJoystick JoyStick;
	public Canvas DragCanvas;
	public UIRpgWindow winRpg => GetWindow<UIRpgWindow>("winRpg");
	public Action<UIWindow> winOpenAction;
	public Action<UIWindow> winCloseAction;
	Dictionary<string, UIWindow> m_winDIc = new Dictionary<string, UIWindow>();
	float during_check;

	public T GetWindow<T>(string winName)
	{
		if (m_winDIc.ContainsKey(winName)) return m_winDIc[winName].GetComponent<T>();
		else
		{
			T win = GetWindow<T>();
			if (win == null) return AddWindow<T>(winName);
			else return win;
		}
	}

	public T GetWindow<T>()
	{
		return GetComponentInChildren<T>(true);
	}

	UIWindow GetWindow(string winName)
	{
		if (m_winDIc.ContainsKey(winName)) return m_winDIc[winName];
		else return AddWindow<UIWindow>(winName);
	}

	T AddWindow<T>(string winName)
	{
		GameObject winPrefab = Resources.Load(winName) as GameObject;
		winPrefab.SetActive(false);
		if (winPrefab == null) { Helpers.LogError(this, "AddWindow", "missing resource path: " + winName); return default(T); }
		else
		{
			T openWin = Instantiate(winPrefab, transform).GetComponent<T>();
			UIWindow win = openWin as UIWindow;
			m_winDIc.Add(win.winName, win);
			return openWin;
		}
	}

	public bool OpenWindow(string winName)
	{
		if (m_winDIc.ContainsKey(winName)) { m_winDIc[winName].Open(); return true; }

		UIWindow openWin = AddWindow<UIWindow>(winName);
		openWin?.Open();
		return openWin != null;
	}

	public void SwitchWindow(string winName)
	{
		var win = GetWindow(winName);
		if (win != null && win.gameObject.activeSelf) win.Close();
		else win?.Open();
	}
}
