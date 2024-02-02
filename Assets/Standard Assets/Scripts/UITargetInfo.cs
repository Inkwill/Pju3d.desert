using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;
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

	UILootElement[] uilootList = new UILootElement[maxUILoot];
	public void Init()
	{
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.Player.BaseAI.InteractDetector.OnEnter.AddListener(OnItemEnter);
		GameManager.Player.BaseAI.InteractDetector.OnExit.AddListener(OnItemExit);
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
				enemyHp.value = GameManager.Player.CurrentEnemy.Stats.CurrentHealth;
				break;
			default:
				break;
		}
	}
	void OnItemEnter(GameObject obj)
	{
		Loot loot = obj.GetComponentInParent<Loot>();
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

	void OnItemExit(GameObject obj)
	{
		Loot loot = obj.GetComponentInParent<Loot>();
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
	// public void OnLooted(Loot loot)
	// {
	// 	var uiloot = uilootList.Where(ui => ui.loot == loot).FirstOrDefault();
	// 	if (uiloot)	
	// 		if (lootList.Contains(loot)) 
	// 		else Debug.LogError("Looted unexpected loot :" + loot);
	// }
}
