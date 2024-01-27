using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UITargetInfo : MonoBehaviour
{
	[SerializeField]
	Text infoText;
	[SerializeField]
	UICharacterHud enemyHud;
	[SerializeField]
	RectTransform lootRoot;
	const int maxUILoot = 7;
	[SerializeField]
	GameObject lootElement;

	List<Loot> lootList;
	UILootElement[] uilootList = new UILootElement[maxUILoot];
	public void Init()
	{
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.Player.Detector_item.OnEnter.AddListener(OnItemEnter);
		GameManager.Player.Detector_item.OnExit.AddListener(OnItemExit);
		enemyHud.Show(GameManager.Player.CurrentEnemy);
		lootList = new List<Loot>();
		for (int i = 0; i < maxUILoot; i++)
		{
			uilootList[i] = Instantiate(lootElement).GetComponent<UILootElement>();
			uilootList[i].gameObject.transform.parent = lootRoot;
			uilootList[i].gameObject.SetActive(false);
		}
	}

	void OnPlayerEvent(GameObject obj, string eventName)
	{
		infoText.text = eventName;
		switch (eventName)
		{
			case "roleEvent_OnSetCurrentEnemy":
				enemyHud.Show(GameManager.Player.CurrentEnemy);
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
			lootList.Add(loot);
			loot.Highlight();
			if (lootList.Count <= uilootList.Length) uilootList[lootList.Count - 1].Init(this, loot);
		}
	}

	void OnItemExit(GameObject obj)
	{
		Loot loot = obj.GetComponentInParent<Loot>();
		if (lootList.Contains(loot))
		{
			lootList.Remove(loot);
			loot.Dehighlight();
			for (int i = 0; i < uilootList.Length; i++)
			{
				if (uilootList[i].loot == loot) uilootList[i].gameObject.SetActive(false);
			}
		}
		else
		{
			Debug.LogError("Exit unexpected loot :" + loot);
		}
	}
	public void OnLooted(Loot loot)
	{
		if (lootList.Contains(loot))
		{
			lootList.Remove(loot);
			for (int i = 0; i < uilootList.Length; i++)
			{
				if (uilootList[i] == loot) uilootList[i].gameObject.SetActive(false);
			}
		}
		else
		{
			Debug.LogError("Looted unexpected loot :" + loot);
		}
	}
}