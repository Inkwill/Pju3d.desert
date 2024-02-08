using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

[CreateAssetMenu(fileName = "MoveSkill", menuName = "Data/MoveSkill", order = 2)]
public class MoveSkill : Skill
{
	public float Speed;
	float m_baseSpeed = 0;

	public override void Implement(RoleControl user, GameObject target = null)
	{
		user.Speed = m_baseSpeed;
		base.Implement(user, target);
	}

	public override void Operating(RoleControl user)
	{
		if (m_baseSpeed == 0) m_baseSpeed = user.Speed;
		user.transform.Translate(Vector3.forward * Speed * Time.deltaTime);
		var Effectpos = user.transform.position;
		VFXManager.PlayVFX(fxStep, Effectpos);
	}
}
