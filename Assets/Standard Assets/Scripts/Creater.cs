using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creater : TimerBehaviour
{
	public GameObject createrObj;
	public GameObject fxProgress;
	public GameObject hideObj;


	public override void OnStart()
	{
		if (fxProgress) fxProgress.SetActive(true);
	}

	public override void OnEnd()
	{
		if (fxProgress) fxProgress.SetActive(false);
	}
	public override void OnTimer()
	{
		CreateObj(transform);
	}

	public virtual void CreateObj(Transform trans)
	{
		if (createrObj)
		{
			GameObject obj = Instantiate(createrObj, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		}
		if (hideObj.active) hideObj.SetActive(false);
	}
}
