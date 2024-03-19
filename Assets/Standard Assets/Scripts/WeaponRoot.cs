using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRoot : MonoBehaviour
{
	public Transform bulletTrans;
	public Vector3 bulletPos
	{
		get
		{
			if (bulletTrans != null) return bulletTrans.position;
			else return transform.position;
		}
	}
}
