using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Data/SpawnData", order = 600)]
public class SpawnData : ScriptableObject
{
	public GameObject ObjectToSpawn;
	public Vector3 spawnerPos;
	public AiData aiData;
	public int spawnCount = 1;
	public int spawnTimes = 1;
	public float spawnWaitTime = 10.0f;
	public float spawnCd = 10.0f;
	public int radius = 5;
	public int angleStep = 15;
	public List<KeyValueData.KArray<Vector3>> paths;
	public Spawner Instantiate()
	{
		Spawner spawner = GameObject.Instantiate(new GameObject()).AddComponent<Spawner>();
		spawner.transform.position = spawnerPos;
		spawner.Init(this);
		return spawner;
	}

	public Vector3[] GetPath(int index = -1)
	{
		if (paths.Count < 1 || index >= paths.Count) return null;
		if (index >= 0) return paths[index].Value;
		else
		{
			int randIndex = UnityEngine.Random.Range(0, paths.Count);
			return paths[randIndex].Value;
		}
	}
}
