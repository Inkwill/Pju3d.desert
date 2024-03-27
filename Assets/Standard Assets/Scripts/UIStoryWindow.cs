using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIStoryWindow : UIWindow
{
	public TMP_Text story;
	protected override void OnOpen()
	{
		StartCoroutine(ShowText(story.text));
	}

	IEnumerator ShowText(string content)
	{
		for (int i = 0; i < content.Length; i++)
		{
			story.text = content.Substring(0, i + 1);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		Close();
	}
}
