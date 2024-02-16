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
	RoleControl m_role;
	Animator m_Animator;
	EventSender m_eventSender;
	float m_During;

	void Start()
	{
		m_SkillEntries = new List<SkillEntry>();
		m_role = GetComponent<RoleControl>();
		m_eventSender = GetComponent<EventSender>();
		AnimationDispatcher dispatcher = GetComponentInChildren<AnimationDispatcher>();
		if (dispatcher) { dispatcher.SkillStep.AddListener(SkillStep); m_Animator = dispatcher.GetComponent<Animator>(); }
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
				m_UseEntry.skill.Operating(m_role, m_UseEntry.targets);
				m_eventSender?.Send(gameObject, "skillEvent_OnOperat");
				m_During += Time.deltaTime;
			}
			else
			{
				m_UseEntry.skill.Implement(m_role, m_UseEntry.targets);
				m_eventSender?.Send(gameObject, "skillEvent_OnImplement");
				m_UseEntry = null;
				m_During = 0;
				m_role.SkillDetector.layers = LayerMask.GetMask("Nothing");
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
		Debug.Log("AddSkill:" + skill.SkillName);
	}


	public bool UseSkill(Skill skill)
	{
		switch (skill.TType)
		{
			case Skill.TargetType.SELF:
				return UseSkill(skill, m_role.gameObject);
			case Skill.TargetType.SCENEBOX:
				return UseSkill(skill, m_role.SceneDetector?.gameObject);
			case Skill.TargetType.CURRENT:
				return UseSkill(skill, m_role.CurrentEnemy?.gameObject);
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
				Debug.Log("Can't UseSkill:" + skill.SkillName + "target =" + m_role.CurrentEnemy);
				return false;
				// if (m_role.CurrentEnemy == null || (entry.skill.layers & (1 << m_role.CurrentEnemy.gameObject.layer)) == 0)
			}
			if (entry.skill.TType == Skill.TargetType.SCENEBOX && m_role.BaseAI.SceneBox != "blank")
			{
				Debug.Log("Can't UseSkill:" + skill.SkillName + "target =" + m_role.BaseAI.SceneBox);
				return false;
			}
			Debug.Log("UseSkill: " + skill.SkillName + "cd =" + entry.cd);
			m_role.SkillDetector.layers = entry.skill.layers;
			m_role.SkillDetector.Radius = entry.skill.radius;
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
		m_UseEntry?.skill.StepEffect(m_role, m_UseEntry.targets);
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
