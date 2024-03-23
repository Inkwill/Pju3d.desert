using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData", order = 700)]
public class LevelData : ScriptableObject
{
	public string LevelId;
	public string Desc;
	public Vector3 teleportPos;
	public int maxStageNum;
	public List<KeyValueData.KeyValue<int, SpawnData>> spawners;
	public List<KeyValueData.KeyValue<int, StoryEventData>> storyEvents;
	public List<KeyValueData.KeyValue<int, GoalData>> finishGoals;

	public List<SpawnData> GetSpawners(int step)
	{
		return KeyValueData.GetValues<SpawnData>(spawners, step);
	}

	public GoalData GetfinishGoal(int step)
	{
		return KeyValueData.GetValue<GoalData>(finishGoals, step);
	}

	public StoryEventData GetstoryEvent(int step)
	{
		return KeyValueData.GetValue<StoryEventData>(storyEvents, step);
	}

	public void ActiveSpawner(int step)
	{
		foreach (var spawner in GetSpawners(step))
		{
			spawner.Instantiate().StartSpawn();
		}
	}
	public void AddStoryEvent(int step, Character character)
	{
		GetstoryEvent(step).Instantiate(character.gameObject);
	}
	public void Teleport(Character character)
	{
		var agent = character.GetComponent<NavMeshAgent>();
		if (agent)
		{
			agent.enabled = false;
			agent.transform.position = teleportPos;
			agent.enabled = true;
		}
		else character.transform.position = teleportPos;
	}
}
