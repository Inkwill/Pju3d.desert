using System.Collections;
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
		int m_step;
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
			if (m_data.GetSpawners(m_step) != null) m_data.ActiveSpawner(m_step);
			if (m_data.GetstoryEvent(m_step) != null) m_data.AddStoryEvent(m_step, m_character);
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
