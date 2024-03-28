using System.Collections;
using UnityEngine;
using System.Linq;
using MyBox;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UITeamWindow : UIWindow
{
	public Transform infoRoot;
	public TMP_Text lbStats;
	UITeamerToggle[] m_toggles;
	protected override void OnOpen()
	{
		UITeamerHead[] uielements = GetComponentsInChildren<UITeamerHead>();
		uielements.ForEach(element => element.SetTeam(GameManager.Lord.rpgTeam));
		m_toggles = infoRoot.GetComponentsInChildren<UITeamerToggle>();
		infoRoot.gameObject.SetActive(false);
	}

	public void UpdateInfo(TroopSystem.TeamerType teamerType)
	{
		Character character = GameManager.Lord.rpgTeam.GetTeamer(teamerType);
		if (character)
		{
			StatSystem.Stats stats = GameManager.CurHero.Stats.stats;
			lbStats.text = $"Str : {stats.strength} Def : {stats.defense} Agi : {stats.agility}  Spr : {stats.spirit}";
		}
	}

	public void ShowInfo(TroopSystem.TeamerType teamerType)
	{
		var teamerToggle = m_toggles.Where(tog => tog.teamerType == teamerType).FirstOrDefault();
		teamerToggle.toggle.isOn = true;
		infoRoot.gameObject.SetActive(true);
		UpdateInfo(teamerType);
	}

	void Update()
	{
		if (!GameManager.CurHero.BaseAI.isIdle && infoRoot.gameObject.activeSelf) infoRoot.gameObject.SetActive(false);
	}
}
