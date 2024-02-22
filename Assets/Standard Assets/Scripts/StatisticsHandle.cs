using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using TMPro;
using System;

public class StatisticsHandle : MonoBehaviour
{
	public Action<GameGoalSystem.GoalType, Dictionary<string, int>> UpdateAction;
	public RoleControl Owner { get { return m_owner; } }
	Dictionary<string, int> m_AddItemCount;
	RoleControl m_owner;

	void Start()
	{
		m_owner = GetComponent<RoleControl>();
		m_AddItemCount = new Dictionary<string, int>();
		if (m_owner) m_owner.Data.Inventory.ItemEvent += OnItemEvent;
	}

	void OnItemEvent(Item item, string actionName, int itemCount)
	{
		if (actionName == "Add")
		{
			Record(m_AddItemCount, item.ItemName, itemCount);
			UpdateAction?.Invoke(GameGoalSystem.GoalType.AddItem, new Dictionary<string, int> { { item.ItemName, itemCount } });
			//Helpers.Log(this, "Add", $"{item.ItemName}x{itemCount}");
		}
	}

	void Record(Dictionary<string, int> dic, string key, int value)
	{
		if (dic.ContainsKey(key)) m_AddItemCount[key] += value;
		else dic.Add(key, value);
	}
}
