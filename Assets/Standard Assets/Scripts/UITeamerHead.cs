using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UITeamerHead : MonoBehaviour
{
	public Image headIcon;
	public TroopSystem.TeamerType teamerType;
	TroopSystem.Team m_team;

	public void SetTroop(TroopSystem.Team troop)
	{
		m_team = troop;
		m_team.changeAction += UpdateInfo;
		UpdateInfo(teamerType);
	}

	void UpdateInfo(TroopSystem.TeamerType type)
	{
		if (teamerType == type)
		{
			var trooper = m_team.GetTrooper(type);
			if (trooper)
			{
				headIcon.sprite = trooper.Data.headIcon;
				UICharacterHp uiHp = GetComponentInChildren<UICharacterHp>();
				if (uiHp) uiHp.SetCharacter(trooper);
			}
		}
	}

}
