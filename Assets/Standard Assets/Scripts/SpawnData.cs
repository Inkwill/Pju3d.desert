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
}
