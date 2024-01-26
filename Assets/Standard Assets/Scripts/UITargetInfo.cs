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
	GameObject lootRoot;
	[SerializeField]
	GameObject lootElement;

	[SerializeField]
	List<Loot> lootList;
	[SerializeField]
	List<UILootElement> uilootList;
	public void Init()
	{
		GameManager.Player.eventSender.events.AddListener(OnPlayerEvent);
		GameManager.Player.Detector_item.OnEnter.AddListener(OnItemEnter);
		GameManager.Player.Detector_item.OnExit.AddListener(OnItemExit);
		enemyHud.Show(GameManager.Player.CurrentEnemy);
		lootList = new List<Loot>();
	}

	void OnPlayerEvent(GameObject obj, string eventName)
	{
		infoText.text = eventName;
		switch (eventName)
		{
			case "playerEvent_OnSetCurrentEnemy":
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
			if (lootList.Count <= uilootList.Count) uilootList[lootList.Count].Init(this, loot);
		}
	}

	void OnItemExit(GameObject obj)
	{
		Loot loot = obj.GetComponentInParent<Loot>();
		if (lootList.Contains(loot))
		{
			lootList.Remove(loot);
			loot.Dehighlight();
			foreach (var item in uilootList)
			{
				if (item.loot == loot) item.gameObject.SetActive(false);
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
			foreach (var item in uilootList)
			{
				if (item.loot == loot) item.gameObject.SetActive(false);
			}
		}
		else
		{
			Debug.LogError("Looted unexpected loot :" + loot);
		}
	}
}