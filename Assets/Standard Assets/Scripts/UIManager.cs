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
	public UIRpgWindow winRpg => GetWindow<UIRpgWindow>();
	public Action<UIWindow> winOpenAction;
	public Action<UIWindow> winCloseAction;
	float during_check;

	public T GetWindow<T>(string winName = "", bool open = false, Action<T> action = null)
	{
		T w = GetComponentInChildren<T>(true);
		if (w == null && winName != "")
		{
			UIWindow window = GetWindow(winName);
			if (window != null) w = window.GetComponent<T>();
		}
		if (w != null)
		{
			if (action != null) action.Invoke(w);
			if (open)
			{
				UIWindow window = w as UIWindow;
				window?.Open();
			}
		}
		return w;
	}

	public UIWindow GetWindow(string winName)
	{
		var wins = GetComponentsInChildren<UIWindow>(true);
		UIWindow win = wins.Where(w => w.winName == winName).FirstOrDefault();
		if (win == null)
		{
			GameObject winPrefab = Resources.Load(winName) as GameObject;
			if (winPrefab == null) Helpers.LogError(this, "GetWindow", "missing path: " + winName);
			else win = Instantiate(winPrefab, transform).GetComponent<UIWindow>();
			win?.gameObject.SetActive(false);
		}
		return win;
	}

	public void SwitchWindow(string winName)
	{
		var win = GetWindow(winName);
		if (win != null && win.gameObject.activeSelf) win.Close();
		else win?.Open();
	}
}
