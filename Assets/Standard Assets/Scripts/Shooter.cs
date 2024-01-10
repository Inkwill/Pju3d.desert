using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class Shooter : TimerBehaviour
{
	GameObject m_CurrentTarget;
	InteractOnTrigger m_Detector;
	public CharacterData m_CharacterData;
	void Start()
	{
		m_Detector = GetComponentInChildren<InteractOnTrigger>();
		m_Detector.OnEnter.AddListener(OnEnter);
		m_Detector.OnExit.AddListener(OnExit);
		m_CharacterData.Init();
	}

	protected override void OnTimer()
	{
		if (m_CurrentTarget)
		{
			GameObject createObj = Resources.Load("bullet") as GameObject;
			createObj = Instantiate(createObj, transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			Bullet bullet = createObj.GetComponent<Bullet>();
			if (bullet)
			{
				bullet.target = m_CurrentTarget.transform;
				bullet.shooter = m_CharacterData;
				bullet.active = true;
			}
		}
	}

	protected override void OnInterval()
	{
		if (!m_CurrentTarget)
		{
			m_CurrentTarget = m_Detector.GetNearest();
			if (m_CurrentTarget) isStarted = true;
		}
	}

	void OnEnter(GameObject enter)
	{
		if (!m_CurrentTarget)
		{
			m_CurrentTarget = enter;
			isStarted = true;
		}
	}

	void OnExit(GameObject exiter)
	{
		if (m_CurrentTarget.gameObject == exiter)
		{
			m_CurrentTarget = null;
			isStarted = false;
		}
	}
}
