using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "ResInventoryItem", menuName = "Data/ResInventory Item", order = -900)]
public class ResInventoryItem : Item
{
	public ResItem.ResType Type;
	public int capacity;


}
