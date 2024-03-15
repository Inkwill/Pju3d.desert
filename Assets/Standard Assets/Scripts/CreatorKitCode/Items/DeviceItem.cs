using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "DeviceItem", menuName = "Data/Device Item", order = -999)]
public class DeviceItem : Item
{
	public GameObject prefab;
	public GameObject modle;
	public bool undergroud;
}
