using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelect : UIWindow
{
	public List<LevelData> levels;
	public void OnClick(string eventName)
	{
		if (eventName == "newbie")
		{
			GameManager.GameLevel.StartLevel(levels[0], GameManager.CurHero);
		}
		else if (eventName == "td")
		{
			GameManager.GameLevel.StartLevel(levels[1], GameManager.CurHero);
		}
		else if (eventName == "slg")
		{
			GameManager.GameLevel.StartLevel(levels[2], GameManager.CurHero);
		}
		Close();
	}
}
