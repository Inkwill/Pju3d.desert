using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class PlayerAI : RoleAI
{
	public override void Init()
	{
		base.Init();
		m_character.Inventory.ItemEvent += (OnItemEvent);
	}
	void FixedUpdate()
	{
		if (GameManager.BuildMode) return;
		if (m_role.CurState == RoleControl.State.DEAD || m_role.CurState == RoleControl.State.SKILLING) return;

		Vector3 direction = Vector3.forward * GameManager.GameUI.JoyStick.Vertical + Vector3.right * GameManager.GameUI.JoyStick.Horizontal;
		if (direction.magnitude > 0)
		{
			m_character.CurrentEnemy = null;
			// if (m_role.CurState == RoleControl.State.ATTACKING)
			// 	m_Animator.StopPlayback();
			//if (m_Destination != Vector3.zero) m_Destination = Vector3.zero;
			Move(direction);
		}
		else if (direction.magnitude == 0 && m_role.CurState == RoleControl.State.MOVE && m_Destination == Vector3.zero)
			m_role.SetState(RoleControl.State.IDLE);
	}

	void OnItemEvent(Item item, string eventName, int itemCount)
	{
		if (eventName == "Add")
		{
			if (m_character.Equipment.Weapon == null || m_character.Equipment.ViceWeapon == null)
			{
				Weapon wp = item as Weapon;
				if (wp) m_character.Inventory.EquipItem(wp);
			}
		}
	}
}
