using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDigButton : UIEventSender
{
	public Skill digSkill;
	public Image bg;
	public Image buildToggle;
	bool m_buildToggle;

	void Start()
	{
		SetBuildToggle(false);
		GameManager.Player.SkillUser.AddSkill(digSkill);
		OnLonePress.AddListener(() => SetBuildToggle(!m_buildToggle));
		OnClick.AddListener(() =>
				{
					if (GameManager.SceneBoxInfo(false) == "blank")
					{
						GameManager.Player.SkillUser.UseSkill(digSkill, GameManager.Player.BaseAI.SceneDetector.gameObject);
						SetBuildToggle(false);
					}
				});
	}
	void FixedUpdate()
	{
		switch (GameManager.SceneBoxInfo(false))
		{
			case "blank":
				bg.color = GameManager.Player.BaseAI.BuildRender.material.color = Color.green;
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
