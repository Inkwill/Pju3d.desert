using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;

public class GameGoalSystem : MonoBehaviour
{
	public enum GoalType
	{
		AddItem
	}

	public class GameGoal
	{
		public GoalData data { get { return m_data; } }
		public Action<GoalData> AchievedEvent;
		public bool Completed
		{
			get
			{
				bool completed = true;
				foreach (var data in m_completeData)
				{
					completed &= data.Value >= m_data.Conditions[data.Key];
				}
				return completed;
			}
		}
		Dictionary<string, int> m_completeData;
		GoalData m_data;
		StatisticsHandle m_record;
		public GameGoal(StatisticsHandle record, GoalData data)
		{
			m_data = data;
			m_record = record;
			m_record.UpdateAction += UpdateGoalInfo;
			m_completeData = new Dictionary<string, int>();
			foreach (var cond in m_data.Conditions)
			{
				m_completeData.Add(cond.Key, 0);
			}
		}
		void UpdateGoalInfo(GoalType type, Dictionary<string, int> recordData)
		{
			if (type != m_data.type) return;
			switch (m_data.type)
			{
				case GameGoalSystem.GoalType.AddItem:
					foreach (var data in recordData)
					{
						bool update = false;
						if (m_completeData.ContainsKey(data.Key)) { m_completeData[data.Key] += data.Value; update = true; }
						if (m_completeData.ContainsKey("any")) { m_completeData["any"] += data.Value; update = true; }
						if (update) Helpers.Log(this, "UpdateGoal", $"{m_data.goalName},progress={ProgressInfo}");
					}
					break;
				default:
					break;
			}
			if (Completed) Achieve();
		}
		public string ProgressInfo
		{
			get
			{
				List<string> result = new List<string>();
				foreach (var item in m_completeData)
				{
					result.Add($"{item.Key}:{item.Value}/{m_data.Conditions[item.Key]}");
				}
				return string.Join(",", result);
			}
		}

		void Achieve()
		{
			AchievedEvent?.Invoke(m_data);
			m_record.UpdateAction -= UpdateGoalInfo;
			if (m_record.Owner != null)
			{
				foreach (var effect in m_data.rewardEffects)
				{
					effect.Key.Take(m_record.Owner.gameObject, effect.Value);
				}
			}
		}
	}

	public GoalData TestGoal;
	List<GoalData> m_AchievedGoals;
	GameGoal m_curGoal;

	public void Init()
	{
		m_AchievedGoals = new List<GoalData>();
		var record = GameManager.Player.GetComponent<StatisticsHandle>();
		if (record) m_curGoal = AddGoal(record, TestGoal);
	}
	public GameGoal AddGoal(StatisticsHandle recorder, GoalData data)
	{
		var goal = new GameGoal(recorder, data);
		goal.AchievedEvent += OnGameGoalAchieved;
		Helpers.Log(this, "AddGoal", $"{goal.data.goalName},completed={goal.ProgressInfo}");
		return goal;
	}

	void OnGameGoalAchieved(GoalData data)
	{
		if (data == m_curGoal.data)
		{
			m_AchievedGoals.Add(data);
			m_curGoal.AchievedEvent -= OnGameGoalAchieved;
			m_curGoal = null;
			Helpers.Log(this, "AchieveGoal", $"{data.goalName},reward={data.rewardEffects}");
		}
	}
}