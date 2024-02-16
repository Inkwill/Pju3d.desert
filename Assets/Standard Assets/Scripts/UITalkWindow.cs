using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITalkWindow : UIWindow
{
	public Text textNpc;
	protected override void OnOpened()
	{
		if (GameManager.StoryListener.CurrentTeller != null)
		{
			textNpc.text = GameManager.StoryListener.CurrentTeller.CurrentNode.Content;
		}
	}
}
