using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UITeamerToggle : MonoBehaviour
{
	public TroopSystem.TeamerType teamerType;
	public Toggle toggle;

	void Awake()
	{
		toggle = GetComponent<Toggle>();
	}

	void Start()
	{
		Character character = GameManager.Lord.rpgTeam.GetTeamer(teamerType);
		toggle.interactable = character != null;

		toggle.onValueChanged.AddListener((value) =>
		{
			if (value)
			{
				UITeamWindow winTeam = GameManager.GameUI.GetWindow<UITeamWindow>();
				winTeam.UpdateInfo(teamerType);
			}
		});
	}
}
