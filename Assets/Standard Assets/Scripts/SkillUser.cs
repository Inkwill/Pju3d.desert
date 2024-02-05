using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUser : MonoBehaviour
{
	public class SkillEntry
	{
		public Skill skill;
		public float cd;
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
				m_UseEntry.skill.Operating(m_role);
				m_eventSender?.Send(gameObject, "skillEvent_OnOperat");
				m_During += Time.deltaTime;
			}
			else
			{
				m_UseEntry.skill.Implement(m_role);
				m_eventSender?.Send(gameObject, "skillEvent_OnImplement");
				m_UseEntry = null;
				m_During = 0;
			}
		}
	}

	public SkillEntry GetEntry(string index)
	{
		if (m_SkillEntries.Count < 1) return null;
		return m_SkillEntries.Where(sk => sk.skill.SkillName == index).FirstOrDefault();
	}

	public bool UseSkill(Skill skill)
	{
		if (m_UseEntry != null) return false;
		SkillEntry entry = GetEntry(skill.SkillName);
		if (entry != null)
		{
			if (entry.cd > 0)
			{
				Debug.Log("Can't UseSkill:" + skill.SkillName + "cd =" + entry.cd);
				return false;
			}
			else
			{
				Debug.Log("UseSkill: " + skill.SkillName + "cd =" + entry.cd);
				entry.cd = entry.skill.CD;
				m_UseEntry = entry;
				return true;
			}
		}
		else
		{
			entry = new SkillEntry();
			entry.skill = skill;
			entry.cd = entry.skill.CD;
			m_UseEntry = entry;
			m_SkillEntries.Add(entry);
			Debug.Log("UseSkill:" + skill.SkillName + "cd =" + entry.cd);
			return true;
		}
	}

	void SkillStep()
	{
		m_UseEntry?.skill.StepEffect(m_role);
	}
}
