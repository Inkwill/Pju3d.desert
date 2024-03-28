using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class UITeamWindow : UIWindow
{
	protected override void OnOpened()
	{
		UITeamerHead[] uielements = GetComponentsInChildren<UITeamerHead>();
		uielements.ForEach(element => element.SetTroop(GameManager.Lord.rpgTeam));
	}
}
