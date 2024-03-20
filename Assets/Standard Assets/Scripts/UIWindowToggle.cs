using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowToggle : MonoBehaviour
{
	public string winName;
	public Sprite sprite_isOpen;
	public Sprite sprite_isClose;
	Button m_button;
	Image m_image;

	void Awake()
	{
		m_button = GetComponent<Button>();
		m_image = GetComponent<Image>();
	}

	void Start()
	{
		GameManager.GameUI.winOpenAction += (win) => { if (win.winName == winName) m_image.sprite = sprite_isOpen; };
		GameManager.GameUI.winCloseAction += (win) => { if (win.winName == winName) m_image.sprite = sprite_isClose; };

		m_button.onClick.AddListener(() =>
		{
			GameManager.GameUI.SwitchWindow(winName);
		});
	}

}
