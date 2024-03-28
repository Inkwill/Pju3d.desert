using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UITeamerHead : UIEventSender
{
	public Image headIcon;
	public TroopSystem.TeamerType teamerType;
	TroopSystem.Team m_team;

	void Start()
	{
		OnClickEvent.AddListener(() =>
		{
			UITeamWindow winTeam = GameManager.GameUI.GetWindow<UITeamWindow>();
			winTeam.ShowInfo(teamerType);
		});
	}
	public void SetTeam(TroopSystem.Team team)
	{
		m_team = team;
		m_team.changeAction += UpdateInfo;
		UpdateInfo(teamerType);
	}

	void UpdateInfo(TroopSystem.TeamerType type)
	{
		if (teamerType == type)
		{
			var trooper = m_team.GetTeamer(type);
			if (trooper)
			{
				headIcon.sprite = trooper.Data.headIcon;
				UICharacterHp uiHp = GetComponentInChildren<UICharacterHp>();
				if (uiHp) uiHp.SetCharacter(trooper);
				gameObject.SetActive(true);
			}
			else gameObject.SetActive(false);
		}
	}

}
