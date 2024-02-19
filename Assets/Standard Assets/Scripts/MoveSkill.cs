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

	public override void Implement(RoleControl user, List<GameObject> targets = null)
	{
		RoleAI ai = user.BaseAI as RoleAI;
		ai.MoveSpeed = m_baseSpeed;
		base.Implement(user, targets);
	}

	public override void Operating(RoleControl user, List<GameObject> targets = null)
	{
		RoleAI ai = user.BaseAI as RoleAI;
		if (m_baseSpeed == 0) m_baseSpeed = ai.MoveSpeed;
		user.transform.Translate(Vector3.forward * Speed * Time.deltaTime);
		var Effectpos = user.transform.position;
		VFXManager.PlayVFX(fxStep, Effectpos);
	}
}
