using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creater : MonoBehaviour
{
	public GameObject target;

	public void CreateObj(Transform trans)
	{
		GameObject obj = Instantiate(target, trans.position, Quaternion.Euler(0, 180, 0)) as GameObject;
		//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		Debug.Log("create at :" + obj.transform.position);

	}
}
