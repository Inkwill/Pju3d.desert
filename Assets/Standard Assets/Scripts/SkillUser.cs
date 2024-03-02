using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class SkillUser : MonoBehaviour
{
	public class SkillEntry
	{
		public Skill skill;
		public float cd;
		public int mp;
		public List<GameObject> targets;
	}

	public SkillEntry CurSkill => m_UseEntry;
	SkillEntry m_UseEntry;
	List<SkillEntry> m_SkillEntries;
	CharacterData m_character;
	Animator m_Animator;
	float m_During;

	void Start()
	{
		m_SkillEntries = new List<SkillEntry>();
		m_character = GetComponent<CharacterData>();
		CharacterAnimHandle animHandle = GetComponentInChildren<CharacterAnimHandle>();
		if (animHandle) { animHandle.SkillStep.AddListener(SkillStep); m_Animator = animHandle.GetComponent<Animator>(); }
		else m_Animator = GetComponentInChildren<Animator>();
	}

	void Update()
	{
		foreach (SkillEntry entry in m_SkillEntries)
		{
			if (entry.cd > 0) entry.cd -= Time.deltaTime;
		}
		if (m_UseEntry != null)
		{
			if (m_During < m_UseEntry.skill.Duration)
			{
				if (m_Animator) m_Animator.SetTrigger(m_UseEntry.skill.SkillAnim);
				m_UseEntry.skill.Operating(m_character, m_UseEntry.targets);
				m_character.SkillAction?.Invoke(m_UseEntry.skill, "Operat");
				m_During += Time.deltaTime;
			}
			else
			{
				m_UseEntry.skill.Implement(m_character, m_UseEntry.targets);
				m_character.SkillAction?.Invoke(m_UseEntry.skill, "Implement");
				m_UseEntry = null;
				m_During = 0;
				m_character.BaseAI.SkillDetector.layers = LayerMask.GetMask("Nothing");
			}
		}
	}

	public SkillEntry GetEntry(string index)
	{
		if (m_SkillEntries.Count < 1) return null;
		return m_SkillEntries.Where(sk => sk.skill.SkillName == index).FirstOrDefault();
	}

	public void AddSkill(Skill skill)
	{
		SkillEntry entry = GetEntry(skill.SkillName);
		if (entry != null) return;
		entry = new SkillEntry();
		entry.skill = skill;
		entry.cd = entry.skill.CD;
		m_SkillEntries.Add(entry);
		Helpers.Log(this, "AddSkill", ":" + skill.SkillName);
	}


	public bool UseSkill(Skill skill)
	{
		switch (skill.TType)
		{
			case Skill.TargetType.SELF:
				return UseSkill(skill, m_character.gameObject);
			case Skill.TargetType.SCENEBOX:
				return UseSkill(skill, m_character.BaseAI.SceneDetector?.gameObject);
			case Skill.TargetType.CURRENT:
				return UseSkill(skill, m_character.BaseAI.CurrentEnemy?.gameObject);
			default:
				return UseSkill(skill, null);
		}

	}
	public bool UseSkill(Skill skill, GameObject target)
	{
		if (m_UseEntry != null) return false;
		SkillEntry entry = GetEntry(skill.SkillName);
		if (entry != null)
		{
			if (entry.cd > 0 || entry.mp < entry.skill.MP)
			{
				Debug.Log("Can't UseSkill:" + skill.SkillName + "cd =" + entry.cd);
				return false;
			}
			if (entry.skill.TType == Skill.TargetType.CURRENT && target == null)
			{
				Debug.Log("Can't UseSkill:" + skill.SkillName + "target =" + m_character.BaseAI.CurrentEnemy);
				return false;
				// if (m_role.CurrentEnemy == null || (entry.skill.layers & (1 << m_role.CurrentEnemy.gameObject.layer)) == 0)
			}
			if (entry.skill.TType == Skill.TargetType.SCENEBOX && m_character.BaseAI.SceneBox != "blank")
			{
				Debug.Log("Can't UseSkill:" + skill.SkillName + "target =" + m_character.BaseAI.SceneBox);
				return false;
			}
			Helpers.Log(this, "UseSkill", $"{skill.SkillName}-CD:{entry.cd})");
			m_character.BaseAI.SkillDetector.layers = entry.skill.layers;
			m_character.BaseAI.SkillDetector.Radius = entry.skill.radius;
			entry.targets = new List<GameObject>();
			if (target != null) entry.targets.Add(target);
			entry.cd = entry.skill.CD;
			entry.mp = 0;
			m_UseEntry = entry;
			return true;
		}
		else
		{
			// entry = new SkillEntry();
			// entry.skill = skill;
			// entry.cd = entry.skill.CD;
			// if(!once)	m_SkillEntries.Add(entry);
			// m_UseEntry = entry;
			// Debug.Log("UseSkill:" + skill.SkillName + "once =" + once);
			Debug.LogError("Can't Use NoOwner Skill:" + skill.SkillName);
			return false;
		}
	}
	void SkillStep()
	{
		m_UseEntry?.skill.StepEffect(m_character, m_UseEntry.targets);
	}

	public void AddTarget(GameObject target)
	{
		if (m_UseEntry != null && m_UseEntry.targets != null)
		{
			if (!m_UseEntry.targets.Contains(target) && m_UseEntry.targets.Count < 1 + m_UseEntry.skill.AddTargets)
				m_UseEntry.targets.Add(target);
		}
	}

	public void RemoveTarget(GameObject target)
	{
		if (m_UseEntry != null && m_UseEntry.targets != null)
		{
			if (m_UseEntry.targets.Contains(target))
				m_UseEntry.targets.Remove(target);
		}
	}
}
