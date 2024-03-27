using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelSystem
{
	public class Level
	{
		public LevelData Data { get { return m_data; } }
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
			if (m_data.openingRemark != "")
			{
				UIStoryWindow winStory = GameManager.GameUI.GetWindow("winStory") as UIStoryWindow;
				winStory.story.text = m_data.openingRemark;
				GameManager.GameUI.OpenWindow(winStory);
				GameManager.GameUI.winCloseAction += (win) => { if (win == winStory) StartStage(1); };

			}
			else StartStage(1);
			if (m_data.teleportPos != Vector3.zero) m_data.Teleport(character);
			GameManager.Instance.CameraCtrl.SetMode(m_data.cameraMode);
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
			if (m_data.stageDuring > 0)
			{
				GameManager.StartWaitAction(m_data.stageDuring, FinishStage);
			}
			else GameManager.GameGoal.GameGoalAction += OnGoalAction;
			Helpers.Log(this, "LevelStage", $"{m_data.LevelId}:{m_step}/{m_data.maxStageNum}");
		}

		void OnGoalAction(GameGoalSystem.GameGoal goal, string actionName)
		{
			if (actionName == "AchieveGoal" && m_data.GetfinishGoal(m_step) == goal.data)
			{
				FinishStage();
				GameManager.GameGoal.GameGoalAction -= OnGoalAction;
			}
		}
		void FinishStage()
		{
			if (m_step < m_data.maxStageNum) StartStage(++m_step);
			else Finish();
		}

		void Finish()
		{

		}
	}
	public Level CurLevel { get { return m_curLevel; } }
	Level m_curLevel;

	public void StartLevel(LevelData levelData, Character character)
	{
		var level = new Level(levelData, character);
		level.Start(character);
		m_curLevel = level;
	}
}
