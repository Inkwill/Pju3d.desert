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
	[SerializeField]
	Vector2 baseBoundary_LB;
	[SerializeField]
	Vector2 baseBoundary_RT;
	public CameraController.Mode cameraMode;
	public int maxStageNum;
	public string openingRemark;
	public float stageDuring;
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

	public List<StoryEventData> GetstoryEvents(int step)
	{
		return KeyValueData.GetValues<StoryEventData>(storyEvents, step);
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

	public bool InBaseBoundary(Vector3 pos)
	{
		return pos.x > baseBoundary_LB.x && pos.x < baseBoundary_RT.x
				&& pos.z > baseBoundary_LB.y && pos.z < baseBoundary_RT.y;
	}

	public Vector3 ConstraintPos(Vector3 sourcePos)
	{
		Vector3 pos = sourcePos;
		if (pos.x < baseBoundary_LB.x) pos.x = baseBoundary_LB.x;
		if (pos.x > baseBoundary_RT.x) pos.x = baseBoundary_RT.x;
		if (pos.z < baseBoundary_LB.y) pos.z = baseBoundary_LB.y;
		if (pos.z > baseBoundary_RT.y) pos.z = baseBoundary_RT.y;
		return pos;
	}
}
