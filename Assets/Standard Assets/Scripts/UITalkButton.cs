using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UITalkButton : UIEventSender
{
    string m_content;
    public Text text_content;
    UIMainWindow m_owner;

    public void Show(UIMainWindow owner, string text)
    {
        m_content = text;
        m_owner = owner;
        text_content.text = m_content;
        gameObject.SetActive(true);
    }

    void Start()
    {
        OnClick.AddListener(() => { m_owner.OnTalk(m_content); });
    }
}
