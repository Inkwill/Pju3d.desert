using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDigButton : UISkillButton
{
	public Image bg;
	public Image buildToggle;
	bool m_buildToggle;

	protected override void OnStart()
	{
		SetBuildToggle(false);
	}

	protected override void ShowSkillTips()
	{
		SetBuildToggle(!m_buildToggle);
		base.ShowSkillTips();
	}


	void FixedUpdate()
	{
		switch (GameManager.SceneBoxInfo(GameManager.Player.BaseAI.SceneDetector.lastInner, false))
		{
			case "blank":
				bg.color = GameManager.Player.BaseAI.BuildRender.material.color = Color.white;
				break;
			default:
				bg.color = GameManager.Player.BaseAI.BuildRender.material.color = Color.red;
				break;
		}
	}

	void SetBuildToggle(bool value)
	{
		m_buildToggle = value;
		buildToggle.enabled = value;
		GameManager.Player.BaseAI.BuildRender.enabled = value;
	}
}
