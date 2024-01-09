using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{

	public static Package PackageInstance;

	public int item_water
	{
		get { return m_warter; }
		set { m_warter = value; }
	}

	int m_warter = 100;
	private void Start()
	{
		PackageInstance = this;
	}
}
