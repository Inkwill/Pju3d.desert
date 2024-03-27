using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowToggle : MonoBehaviour
{
	public string winName;
	public Image iconOpen;
	public Image iconClose;
	Button m_button;

	void Awake()
	{
		m_button = GetComponent<Button>();
		iconOpen.enabled = false;
		iconClose.enabled = true;
	}

	void Start()
	{
		GameManager.GameUI.winOpenAction += (win) => { if (win.winName == winName) { iconOpen.enabled = true; iconClose.enabled = false; } };
		GameManager.GameUI.winCloseAction += (win) => { if (win.winName == winName) { iconClose.enabled = true; iconOpen.enabled = false; } };

		m_button.onClick.AddListener(() =>
		{
			GameManager.GameUI.SwitchWindow(winName);
		});
	}

}
