using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;

public class TowerAI : AIBase
{

	protected override void OnStart()
	{
		base.OnStart();
		m_character.AttackEvent.AddListener((character) => { VFXManager.PlayVFX(VFXType.CannonFire, m_character.WeaponRoot.bulletPos); });
	}
	public override void LookAt(Vector3 forward)
	{
		forward.y = 0;
		forward.Normalize();
		m_character.WeaponRoot.transform.forward = forward;
	}
}
