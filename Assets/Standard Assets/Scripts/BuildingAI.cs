using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAI : AIBase
{
	public override void LookAt(Transform trans)
	{
		// Vector3 forward = (trans.position - transform.position);
		// forward.y = 0;
		// forward.Normalize();
		// transform.forward = forward;
	}
}
