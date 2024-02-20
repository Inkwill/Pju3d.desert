using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class PlayerAI : RoleAI
{
	public override void Init()
	{
		base.Init();
		m_role.Data.Inventory.ItemEvent += (OnItemEvent);
	}
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

	void OnItemEvent(Item item, string eventName, int itemCount)
	{
		if (eventName == "Add")
		{
			if (m_role.Data.Equipment.Weapon == null || m_role.Data.Equipment.ViceWeapon == null)
			{
				Weapon wp = item as Weapon;
				if (wp) m_role.Data.Inventory.EquipItem(wp);
			}
		}
	}
}
