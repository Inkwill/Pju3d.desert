using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class Shooter : Creater
{
	Transform m_CurrentTarget;
	InteractOnTrigger m_Detector;
	public CharacterData m_CharacterData;
	private void Start()
	{
		m_Detector = GetComponentInChildren<InteractOnTrigger>();
		m_CharacterData.Init();
	}

	public override void CreateObj(Transform trans)
	{
		if (m_CurrentTarget)
		{
			GameObject obj = Instantiate(createrObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			Bullet bullet = createrObj.GetComponent<Bullet>();
			if (bullet)
			{
				bullet.target = m_CurrentTarget;
				bullet.shooter = m_CharacterData;
				bullet.active = true;
			}
		}
	}

	public void OnDetected(string type)
	{
		if (type == "exit" && m_CurrentTarget)
		{
			if (m_CurrentTarget.gameObject == m_Detector.lastExiter)
			{
				m_CurrentTarget = null;
			}
		}

		var enemy = m_Detector.GetIntruder();
		if (enemy)
		{
			m_CurrentTarget = enemy?.transform;
			isStarted = true;
		}

		if (!enemy && !m_CurrentTarget) isStarted = false;
		//Debug.Log("CurrentTarget = " + m_CurrentTargetCharacterData);
	}
}
