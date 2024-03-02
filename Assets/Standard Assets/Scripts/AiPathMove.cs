using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoleAI))]
public class AiPathMove : MonoBehaviour
{
	Transform[] m_paths;
	int m_curPathIndex;
	bool m_Offensive;
	RoleAI m_roleAi;

	void Start()
	{
		m_roleAi = GetComponent<RoleAI>();
		m_roleAi.Character.StateUpdateAction += OnStateUpdate;
		m_roleAi.MoveEndAction += () =>
		{
			if (m_paths.Length > m_curPathIndex)
			{
				m_curPathIndex++;
			}
		};
	}

	void OnStateUpdate(AIBase.State curState)
	{
		if (curState == AIBase.State.IDLE)
		{
			if (m_paths.Length > m_curPathIndex)
			{
				m_roleAi.MoveTo(m_paths[m_curPathIndex].position);
			}
		}
		if (curState == AIBase.State.MOVE)
		{
			if (m_roleAi.Character.CurrentEnemy && m_Offensive) m_roleAi.SetState(AIBase.State.PURSUING);
		}

	}
	public void SetPath(Transform pathRoot, bool offensive = true)
	{
		m_paths = pathRoot.GetComponentsInChildren<Transform>();
		m_curPathIndex = 0;
		m_Offensive = offensive;
	}
}
