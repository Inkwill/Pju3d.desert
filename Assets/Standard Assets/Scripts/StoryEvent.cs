using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvent : MonoBehaviour
{

	void Start()
	{
		GetComponent<InteractOnTrigger>()?.OnEnter.AddListener(OnPlayerEnter);
	}
	public void OnPlayerEnter(GameObject enter)
	{
		var player = enter.GetComponent<CharacterData>();
		if (player == GameManager.CurHero)
		{
			GameManager.StoryMode = true;
			player.BaseAI.Stop();
			player.BaseAI.LookAt(Camera.main.transform);
			UIRoleHud hud = player.GetComponentInChildren<UIRoleHud>();
			StartCoroutine(StartPaly(hud));
		}
	}

	IEnumerator StartPaly(UIRoleHud hud)
	{
		hud.Bubble("麋鹿麋鹿迷了路,");
		yield return new WaitForSeconds(2.0f);
		hud.Bubble("地上一堆小杂物,");
		yield return new WaitForSeconds(2.0f);
		hud.Bubble("捡起杂物找出路！", 1.0f);
		yield return new WaitForSeconds(2.0f);
		GameManager.StoryMode = false;
		GameManager.GameGoal.Init();

	}
}
