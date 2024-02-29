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
		AddItem,
		ItemDemand,
		KillEnemy
	}

	public class GameGoal
	{
		public GoalData data { get { return m_data; } }
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
		GameGoalSystem m_owner;
		public GameGoal(GameGoalSystem owner, GoalData data)
		{
			m_data = data;
			m_owner = owner;
			owner.Recorder.UpdateAction += UpdateGoalInfo;
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
				case GameGoalSystem.GoalType.ItemDemand:
				case GameGoalSystem.GoalType.KillEnemy:
					foreach (var data in recordData)
					{
						bool update = false;
						if (m_completeData.ContainsKey(data.Key)) { m_completeData[data.Key] += data.Value; update = true; }
						if (m_completeData.ContainsKey("any")) { m_completeData["any"] += data.Value; update = true; }
						if (update)
						{
							m_owner.GameGoalAction?.Invoke(this, "UpdateGoal");
							Helpers.Log(this, "UpdateGoal", $"{m_data.goalId},progress={string.Join(",", ProgressInfo)}");
						}

					}
					break;
				default:
					break;
			}
			if (Completed) { m_owner.Recorder.UpdateAction -= UpdateGoalInfo; }
		}
		public List<string> ProgressInfo
		{
			get
			{
				List<string> result = new List<string>();
				foreach (var item in m_completeData)
				{
					result.Add($"{item.Value}/{m_data.Conditions[item.Key]}");
				}
				return result;
			}
		}

		public string Summary
		{
			get
			{
				string result = m_data.summary + " ";
				switch (m_data.type)
				{
					case GoalType.AddItem:
					case GoalType.ItemDemand:
					case GoalType.KillEnemy:
						result += ProgressInfo[0];
						break;
					default:
						break;
				}
				return result;
			}
		}

		public void Acheve()
		{
			m_owner.AchieveGoal(this);
		}
	}

	public Action<GameGoal, string> GameGoalAction;
	public StatisticsHandle Recorder => m_recorder;
	public GameGoal CurrentGoal => m_curGoal;
	List<string> m_AchievedGoals;
	GameGoal m_curGoal;
	StatisticsHandle m_recorder;

	public void Init()
	{
		m_AchievedGoals = new List<string>();
		m_recorder = GameManager.CurHero.GetComponent<StatisticsHandle>();
	}
	public GameGoal AddGoal(GoalData data)
	{
		if (m_curGoal != null)
		{
			if (m_curGoal.Completed) m_curGoal.Acheve();
			else
			{
				Helpers.Log(this, "AddGoal", $"Fail:{data.goalId},cur:{m_curGoal.data.goalId}");
				return null;
			}
		}
		var goal = new GameGoal(this, data);
		if (m_curGoal == null) m_curGoal = goal;
		GameGoalAction?.Invoke(goal, "AddGoal");
		Helpers.Log(this, "AddGoal", $"{goal.data.goalId},completed={goal.ProgressInfo}");
		return goal;
	}

	void AchieveGoal(GameGoal goal)
	{
		m_AchievedGoals.Add(goal.data.goalId);
		GameGoalAction?.Invoke(goal, "AchieveGoal");
		Helpers.Log(this, "AchieveGoal", $"{goal.data.goalId},exp={goal.data.LordExp}");
		if (goal == m_curGoal) m_curGoal = null;
		if (m_recorder.Owner != null && goal.data.rewardEffects != null)
		{
			foreach (var effect in goal.data.rewardEffects)
			{
				effect.Key.TakeEffect(m_recorder.Owner.gameObject, m_recorder.Owner.gameObject, effect.Value);
			}
		}
		GameManager.Lord.AddExp(goal.data.LordExp);
	}

	public bool IsAchievedGoal(string goalId)
	{
		return m_AchievedGoals.Contains(goalId);
	}
}