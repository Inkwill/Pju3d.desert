using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelSystem
{
	public class Level
	{
		LevelData m_data;
		Character m_character;
		public int Step { get { return m_step; } }
		public bool isFinally { get { return m_step == m_data.maxStageNum; } }
		int m_step;
		public Action<Level> stageAction;
		public Level(LevelData data, Character character)
		{
			m_data = data;
			m_character = character;
		}
		public void Start(Character character)
		{
			if (m_data.teleportPos != Vector3.zero) m_data.Teleport(character);
			GameManager.GameGoal.GameGoalAction += OnGoalAction;
			StartStage(1);
		}
		void OnGoalAction(GameGoalSystem.GameGoal goal, string actionName)
		{
			if (actionName == "AchieveGoal" && m_data.GetfinishGoal(m_step) == goal.data) FinishStage();
		}

		void StartStage(int step)
		{
			m_step = step;
			stageAction?.Invoke(this);
			foreach (var spData in m_data.GetSpawners(step))
			{
				var spawner = spData.Instantiate();
				spawner.StartSpawn();
				stageAction += spawner.OnStageAction;
			}
			foreach (var storyEvent in m_data.GetstoryEvents(step))
			{
				storyEvent.Instantiate(m_character.gameObject);
			}
			Helpers.Log(this, "LevelStage", $"{m_data.LevelId}:{m_step}/{m_data.maxStageNum}");
		}

		void FinishStage()
		{
			if (m_step < m_data.maxStageNum) StartStage(++m_step);
			else Finish();
		}

		void Finish()
		{
			GameManager.GameGoal.GameGoalAction -= OnGoalAction;
		}
	}

	Level m_curLevel;

	public void StartLevel(LevelData levelData, Character character)
	{
		var level = new Level(levelData, character);
		level.Start(character);
		m_curLevel = level;
	}
}
