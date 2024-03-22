using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Data/SpawnData", order = 600)]
public class SpawnData : ScriptableObject
{
	public GameObject ObjectToSpawn;
	public AiData aiData;
	public int spawnCount = 1;
	public int spawnTimes = 1;
	public float spawnWaitTime = 10.0f;
	public float spawnCd = 10.0f;
	public int radius = 5;
	public int angleStep = 15;
	public Vector3[] paths;
	public Spawner Instantiate(Vector3 pos)
	{
		Spawner spawner = GameObject.Instantiate(new GameObject()).AddComponent<Spawner>();
		spawner.transform.position = pos;
		spawner.Init(this);
		return spawner;
	}
}
