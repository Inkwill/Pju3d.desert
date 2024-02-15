using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using TMPro;

public class UITargetInfo : MonoBehaviour
{
	[SerializeField]
	Text infoText;
	[SerializeField]
	TMP_Text enemyName;
	[SerializeField]
	Slider enemyHp;
	[SerializeField]
	RectTransform lootRoot;
	const int maxUILoot = 7;
	[SerializeField]
	GameObject lootElement;
	[SerializeField]
	Animator btTalk;

	UILootElement[] uilootList = new UILootElement[maxUILoot];
	public void Init()
	{
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.Player.Interactor.OnItemEnter.AddListener(OnItemEnter);
		GameManager.Player.Interactor.OnItemExit.AddListener(OnItemExit);
		GameManager.Player.Interactor.OnInteracting.AddListener(OnInteracting);
		SetEnemy();
		for (int i = 0; i < maxUILoot; i++)
		{
			uilootList[i] = Instantiate(lootElement).GetComponent<UILootElement>();
			uilootList[i].gameObject.transform.SetParent(lootRoot);
			uilootList[i].gameObject.SetActive(false);
		}
	}

	void SetEnemy()
	{
		if (GameManager.Player.CurrentEnemy)
		{
			RoleControl enemy = GameManager.Player.CurrentEnemy.GetComponent<RoleControl>();
			enemy.eventSender.events.AddListener(OnEnemyEvent);
			enemyName.text = GameManager.Player.CurrentEnemy.CharacterName;
			enemyHp.maxValue = GameManager.Player.CurrentEnemy.Stats.stats.health;
			enemyHp.value = GameManager.Player.CurrentEnemy.Stats.CurrentHealth;
			enemyHp.gameObject.SetActive(true);
		}
		else
		{
			enemyName.text = "";
			enemyHp.gameObject.SetActive(false);
		}
	}
	void OnPlayerEvent(GameObject obj, string eventName)
	{
		infoText.text = eventName;
		switch (eventName)
		{
			case "roleEvent_OnSetCurrentEnemy":
				SetEnemy();
				break;
			default:
				break;
		}
	}

	void OnEnemyEvent(GameObject obj, string eventName)
	{
		switch (eventName)
		{
			case "roleEvent_OnDamage":
				CharacterData data = obj.GetComponent<CharacterData>();
				enemyHp.value = data != null ? data.Stats.CurrentHealth : 0;
				break;
			default:
				break;
		}
	}
	void OnItemEnter(Loot loot)
	{
		if (loot)
		{
			var uiloot = uilootList.Where(ui => ui.loot == null).FirstOrDefault();
			if (uiloot)
			{
				uiloot.Init(loot);
				loot.Highlight();
			}
			// for (int i = 0; i < maxUILoot; i++)
			// {
			// 	if (uilootList[i].loot == null)
			// 	{
			// 		uilootList[i].Init(loot);
			// 		loot.Highlight();
			// 	}
			// }

			// if (!lootList.Contains(loot) && lootList.Count < maxUILoot)
			// {
			// 	lootList.Add(loot);
			// 	loot.Highlight();
			// 	if (lootList.Count <= uilootList.Length) uilootList[lootList.Count - 1].Init(this, loot);
			// }
		}
	}

	void OnItemExit(Loot loot)
	{
		if (loot == null) return;
		var uiloot = uilootList.Where(ui => ui.loot == loot).FirstOrDefault();
		if (uiloot)
		{
			uiloot.loot = null;
			loot.Dehighlight();
			uiloot.gameObject.SetActive(false);
		}
		//else Debug.LogError("Exit unexpected loot :" + loot);
	}

	void OnInteracting(InteractHandle interactor, string eventName)
	{
		if (eventName == "Enter" || eventName == "Interacting") btTalk.SetTrigger("show");
		if (eventName == "Stop") btTalk.SetTrigger("hide");
	}
}
