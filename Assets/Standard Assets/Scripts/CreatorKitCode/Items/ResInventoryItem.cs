using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "ResInventoryItem", menuName = "Data/ResInventory Item", order = -999)]
public class ResInventoryItem : Item
{
	public ResItem.ResType resType;
	public int capacity;


}
