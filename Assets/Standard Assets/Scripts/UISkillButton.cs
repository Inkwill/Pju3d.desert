using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISkillButton : UIEventSender
{
    public Skill skill;
    public Text textName;
    public Image imageCd;
    SkillUser.SkillEntry m_entry;

    void Start()
    {
        OnLonePress.AddListener(ShowSkillTips);
        OnClick.AddListener(() => { GameManager.Player.SkillUser.UseSkill(m_entry.skill); });
        if (skill != null) Init(skill);
        OnStart();
    }

    protected virtual void OnStart() { }
    public void Init(Skill sk)
    {
        skill = sk;
        GameManager.Player.SkillUser.AddSkill(skill);
        textName.text = skill.SkillName;
        m_entry = GameManager.Player.SkillUser.GetEntry(skill.SkillName);
    }
    protected virtual void ShowSkillTips()
    {
        Debug.Log("Show skill tips: " + skill.SkillName);
    }

    protected override void OnUpdate()
    {
        if (m_entry == null) return;
        if (m_entry.cd > 0)
        {
            canClick = false;
            imageCd.fillAmount = m_entry.cd / m_entry.skill.CD;
            textName.text = $"{m_entry.cd:F0}";
        }
        else
        {
            canClick = true;
            imageCd.fillAmount = 0;
            textName.text = skill.SkillName;
        }
    }
}
