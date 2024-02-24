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
	CharacterData m_curEnemy;

	UILootElement[] uilootList = new UILootElement[maxUILoot];
	public void Init()
	{
		GameManager.Player.BaseAI.InteractDetector.OnEnter.AddListener(OnInteractEnter);
		GameManager.Player.BaseAI.InteractDetector.OnExit.AddListener(OnInteractExit);
		//GameManager.Player.InteractDetector.OnStay.AddListener(OnInteractStay);
		UpdateEnemyInfo();
		for (int i = 0; i < maxUILoot; i++)
		{
			uilootList[i] = Instantiate(lootElement).GetComponent<UILootElement>();
			uilootList[i].gameObject.transform.SetParent(lootRoot);
			uilootList[i].gameObject.SetActive(false);
		}
	}

	void Update()
	{
		m_curEnemy = GameManager.Player.BaseAI.CurrentEnemy;
		UpdateEnemyInfo();
	}

	void UpdateEnemyInfo()
	{
		if (m_curEnemy)
		{
			enemyName.text = m_curEnemy.CharacterName;
			enemyHp.maxValue = m_curEnemy.Stats.stats.health;
			enemyHp.value = m_curEnemy.Stats.CurrentHealth;
			enemyHp.gameObject.SetActive(true);
		}
		else
		{
			enemyName.text = "";
			enemyHp.gameObject.SetActive(false);
		}
	}

	void OnInteractEnter(GameObject enter)
	{
		Loot loot = enter.GetComponentInParent<Loot>();
		if (loot)
		{
			var uiloot = uilootList.Where(ui => ui.loot == null).FirstOrDefault();
			if (uiloot)
			{
				uiloot.Init(loot);
				loot.Highlight();
				StartCoroutine(AutoPick(uiloot));
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

	IEnumerator AutoPick(UILootElement uiloot)
	{
		yield return new WaitForSeconds(0.5f);
		uiloot.Pick();
	}

	void OnInteractExit(GameObject exiter)
	{
		Loot loot = exiter.GetComponentInParent<Loot>();
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
}
