using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITalkWindow : UIWindow
{
	public Text textNpc;
	public Text textFriendlyValue;
	public GameObject uiEntrustElement;
	public Transform entrustRoot;
	public UIEntrustInfo winEntrustInfo;
	List<UIEntrustElement> uiEntrustList;


	protected override void OnOpen()
	{
		if (GameManager.StoryListener.CurrentTeller != null)
		{
			textNpc.text = GameManager.StoryListener.CurrentTeller.CurrentNode.Content[0].Key;
			textFriendlyValue.text = GameManager.StoryListener.GetFriendlyValue().ToString();
		}
		if (uiEntrustList == null) uiEntrustList = new List<UIEntrustElement>();

		List<Entrustment> entrusts = GameManager.StoryListener.CurrentTeller.entrustments;
		for (int i = 0; i < entrusts.Count; i++)
		{
			if (uiEntrustList.Count <= i)
			{
				UIEntrustElement element = Instantiate(uiEntrustElement).GetComponent<UIEntrustElement>();
				element.gameObject.transform.SetParent(entrustRoot);
				element.Init(this, entrusts[i]);
				uiEntrustList.Add(element);
			}
			else uiEntrustList[i].Init(this, entrusts[i]);
		}
	}

	protected override void OnClose()
	{
		winEntrustInfo.Close();
	}

}
