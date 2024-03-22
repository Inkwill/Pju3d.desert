using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelSystem : MonoBehaviour
{
	public class LevelStage
	{
		LevelData m_data;
		Character m_character;
		public LevelStage(LevelData data, Character character)
		{
			m_data = data;
			m_character = character;
		}
		public void Start()
		{
			m_data.StartStage(m_character);
		}
	}
	int m_step;
	[SerializeField]
	LevelData[] datas;

	void Start()
	{
		//GameManager.GameGoal.GameGoalAction += OnGoalAction;
		var state = new LevelStage(datas[0], GameManager.CurHero);
		state.Start();
	}
	// void OnGoalAction(GameGoalSystem.GameGoal goal, string actionName)
	// {
	// 	if (actionName == "AchieveGoal")
	// 	{
	// 		for (int i = 0; i < data.goals.Length; i++)
	// 		{
	// 			if (data.goals[i] == goal.data) StartLevel(i++);
	// 		}
	// 	}
	// }
	// public void EnterLevel()
	// {
	// 	data.TeleportStart(GameManager.CurHero.GetComponent<NavMeshAgent>());
	// }

	// public void StartLevel()
	// {
	// 	StartLevel(0);
	// }
	// public void StartLevel(int step)
	// {
	// 	if (step > data.goals.Length) EndLevel();
	// 	else m_step = step;
	// }

	// void EndLevel()
	// {

	// }

}
