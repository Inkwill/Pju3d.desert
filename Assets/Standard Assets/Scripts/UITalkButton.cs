using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UITalkButton : UIEventSender
{
	string m_content;
	public Text text_content;

	public void Show(string text)
	{
		m_content = text;
		text_content.text = m_content;
		gameObject.SetActive(true);
	}

	void Start()
	{
		OnClickEvent.AddListener(() => { GameManager.StoryListener.Ask(m_content); });
	}
}
