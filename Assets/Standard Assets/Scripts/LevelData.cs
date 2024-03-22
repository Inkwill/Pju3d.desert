using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData", order = 700)]
public class LevelData : ScriptableObject
{
	public Vector3 teleportPos;
	public List<KeyValueData.KeyValue<SpawnData, Vector3>> spawners;

	public void StartStage(Character character)
	{
		if (teleportPos != Vector3.zero) Teleport(character);
		foreach (var spawner in spawners)
		{
			spawner.Key.Instantiate(spawner.Value).StartSpawn();
		}
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
