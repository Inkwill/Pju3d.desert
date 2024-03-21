using System;
using UnityEngine;
using CreatorKitCode;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	public SpawnData data;
	public Transform pathRoot;
	public Action<Character> spawnEvent;
	Timer m_timer;
	List<GameObject> m_members;

	void Awake()
	{
		m_timer = Timer.SetTimer(gameObject, data.spawnWaitTime, data.spawnTimes, data.spawnCd);
		m_timer.autoStart = true;
		m_timer.behaveAction += ActSpawn;
	}
	public void StartSpawn()
	{
		m_timer.StartTimer();
	}

	void ActSpawn()
	{
		for (int i = 0; i < data.spawnCount; i++)
		{
			Spawn(data.angleStep * (i + 1), data.radius);
		}
	}

	void Spawn(int angle, int radius)
	{
		Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * radius;
		Character enemy = Instantiate(data.ObjectToSpawn, spawnPosition, Quaternion.Euler(0, 180, 0)).GetComponent<Character>();
		if (pathRoot) enemy.gameObject.AddComponent<AiPathMove>().SetPath(pathRoot);
		if (data.aiData) enemy.BaseAI.Data = data.aiData;
		spawnEvent?.Invoke(enemy);
	}

	// void Update()
	// {
	// 	if (spawnTimes <= 0) return;

	// 	if (colldown < spawnCd)
	// 	{
	// 		colldown += Time.deltaTime;
	// 		return;
	// 	}

	// 	colldown = 0;
	/*
	Vector3 spawnPosition = transform.position;

	Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
	spawnPosition = transform.position + direction * radius;
	Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);

	angle = 55;
	direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
	spawnPosition = transform.position + direction * radius;
	Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);

	angle = 95;
	direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
	spawnPosition = transform.position + direction * radius;
	Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);

	angle = 30;
	direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
	spawnPosition = transform.position + direction * radius;
	Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);
	*/
	// }


}

