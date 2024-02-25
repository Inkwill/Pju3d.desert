using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using TMPro;
using System;

public class StatisticsHandle : MonoBehaviour
{
	public Action<GameGoalSystem.GoalType, Dictionary<string, int>> UpdateAction;
	public CharacterData Owner { get { return m_owner; } }
	Dictionary<string, int> m_AddItemCount;
	Dictionary<string, int> m_ItemDemandCount;
	Dictionary<string, int> m_KillEnemyCount;
	CharacterData m_owner;

	public void Init(CharacterData data)
	{
		m_owner = data;
		m_AddItemCount = new Dictionary<string, int>();
		m_ItemDemandCount = new Dictionary<string, int>();
		m_KillEnemyCount = new Dictionary<string, int>();
		if (m_owner)
		{
			m_owner.Inventory.ItemEvent += OnItemEvent;
			m_owner.OnKillEnemy += OnKillEnemyCount;
			m_owner.GetComponent<EventSender>()?.countEvent.AddListener(OnCountEvent);
		}
	}
	void Record(Dictionary<string, int> dic, string key, int value)
	{
		if (dic.ContainsKey(key)) dic[key] += value;
		else dic.Add(key, value);
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

	void OnCountEvent(EventSender.EventType eventType, string key, int count)
	{
		if (eventType == EventSender.EventType.DemandComplete)
		{
			Record(m_ItemDemandCount, key, count);
			UpdateAction?.Invoke(GameGoalSystem.GoalType.ItemDemand, new Dictionary<string, int> { { key, count } });
		}
	}

	void OnKillEnemyCount(CharacterData enemy)
	{
		Record(m_KillEnemyCount, enemy.CharacterName, 1);
		UpdateAction?.Invoke(GameGoalSystem.GoalType.KillEnemy, new Dictionary<string, int> { { enemy.CharacterName, 1 } });
	}

}
