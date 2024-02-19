using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : RoleAI
{
	void FixedUpdate()
	{
		if (m_role.CurState == RoleControl.State.DEAD || m_role.CurState == RoleControl.State.SKILLING) return;

		Vector3 direction = Vector3.forward * GameManager.GameUI.JoyStick.Vertical + Vector3.right * GameManager.GameUI.JoyStick.Horizontal;
		if (direction.magnitude > 0)
		{
			m_role.CurrentEnemy = null;
			// if (m_role.CurState == RoleControl.State.ATTACKING)
			// 	m_Animator.StopPlayback();
			//if (m_Destination != Vector3.zero) m_Destination = Vector3.zero;
			Move(direction);
		}
		else if (direction.magnitude == 0 && m_role.CurState == RoleControl.State.MOVE && m_Destination == Vector3.zero)
			m_role.CurState = RoleControl.State.IDLE;
	}


}
