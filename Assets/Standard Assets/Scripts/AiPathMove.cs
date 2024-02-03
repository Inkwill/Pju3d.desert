using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoleAI))]
public class AiPathMove : MonoBehaviour
{
	Transform[] m_paths;
	int m_curPathIndex;
	bool m_Offensive;
	RoleControl m_role;

	void Start()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);
	}
	void OnRoleEvent(GameObject obj, string eventName)
	{
		if (eventName == "roleEvent_OnMoveEnd" && m_paths.Length > m_curPathIndex)
		{
			m_curPathIndex++;
		}
		if (eventName == "roleEvent_OnIdling" && m_paths.Length > m_curPathIndex)
		{
			m_role.MoveTo(m_paths[m_curPathIndex].position);
		}
		if (eventName == "roleEvent_OnMoving" && m_Offensive)
		{
			if (m_role.CurrentEnemy) m_role.CurState = RoleControl.State.PURSUING;
		}
	}
	public void SetPath(Transform pathRoot, bool offensive = true)
	{
		m_paths = pathRoot.GetComponentsInChildren<Transform>();
		m_curPathIndex = 0;
		m_Offensive = offensive;
	}
}
