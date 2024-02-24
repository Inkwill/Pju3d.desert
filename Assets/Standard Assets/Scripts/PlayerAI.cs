using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class PlayerAI : RoleAI
{
	public override void Init(CharacterData data)
	{
		base.Init(data);
		m_character.Inventory.ItemEvent += (OnItemEvent);
	}
	void FixedUpdate()
	{
		if (GameManager.BuildMode) return;
		if (CurState == State.DEAD || CurState == State.SKILLING) return;

		Vector3 direction = Vector3.forward * GameManager.GameUI.JoyStick.Vertical + Vector3.right * GameManager.GameUI.JoyStick.Horizontal;
		if (direction.magnitude > 0)
		{
			//CurrentEnemy = null;
			// if (m_role.CurState == RoleControl.State.ATTACKING)
			// 	m_Animator.StopPlayback();
			//if (m_Destination != Vector3.zero) m_Destination = Vector3.zero;
			Move(direction);
		}
		else if (direction.magnitude == 0 && CurState == State.MOVE && m_Destination == Vector3.zero)
			SetState(State.IDLE);
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
