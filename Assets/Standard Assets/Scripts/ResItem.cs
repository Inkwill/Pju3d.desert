using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "ResItem", menuName = "Data/Res Item", order = -999)]
public class ResItem : Item
{
	public enum ResType
	{
		Wood,
		Warter,
		Money,
	}
	public ResType Type { get; private set; }
}

