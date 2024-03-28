using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UITrooperHead : MonoBehaviour
{
	public Image headIcon;
	public TroopSystem.TrooperType trooperType;
	TroopSystem.Troop m_troop;

	public void SetTroop(TroopSystem.Troop troop)
	{
		m_troop = troop;
		m_troop.changeAction += UpdateInfo;
		UpdateInfo(trooperType);
	}

	void UpdateInfo(TroopSystem.TrooperType type)
	{
		if (trooperType == type)
		{
			var trooper = m_troop.GetTrooper(type);
			if (trooper)
			{
				headIcon.sprite = trooper.Data.headIcon;
				UICharacterHp uiHp = GetComponentInChildren<UICharacterHp>();
				if (uiHp) uiHp.SetCharacter(trooper);
			}
		}
	}

}
