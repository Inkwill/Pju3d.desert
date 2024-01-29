using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class Building : TimerBehaviour
{
	[SerializeField]
	string creatPrefab;
	[SerializeField]
	UIItemDemandHud hud;
	RoleControl m_builder;
	InventorySystem.ItemDemand m_Demand;
	void Start()
	{
		var dic = new Dictionary<string, int>
		{
			{ "Wood", 1 },
			{ "Money", 1 },
		};
		m_Demand = new InventorySystem.ItemDemand(dic);
		hud.Init(m_Demand);
	}
}
