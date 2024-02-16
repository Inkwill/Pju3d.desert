using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEntrustElement : MonoBehaviour
{
    public Image iconType;
    public Text textFriendlyRequest;
    public GameObject activeInfo;
    public GameObject disableInfo;
    Entrustment m_entrustment;
    UITalkWindow m_owner;

    public void Init(UITalkWindow owner, Entrustment data)
    {
        m_owner = owner;
        m_entrustment = data;
        activeInfo.SetActive(m_entrustment.FriendlyRequest <= GameManager.StoryListener.GetFriendlyValue());
        disableInfo.SetActive(!activeInfo.activeSelf);
        if (disableInfo.activeSelf) textFriendlyRequest.text = $"{GameManager.StoryListener.GetFriendlyValue()}/{m_entrustment.FriendlyRequest}";
    }

    public void ShowInfo()
    {
        m_owner.winEntrustInfo.Show(m_entrustment);
    }
}
