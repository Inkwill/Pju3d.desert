using System;
using UnityEngine;
using CreatorKitCode;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	SpawnData m_data;
	public Action<Character> spawnEvent;
	Timer m_timer;
	List<GameObject> m_members;

	public void Init(SpawnData data)
	{
		m_data = data;
		m_timer = Timer.SetTimer(gameObject, m_data.spawnWaitTime, m_data.spawnTimes, m_data.spawnCd);
		m_timer.behaveAction += ActSpawn;
	}
	public void StartSpawn()
	{
		m_timer?.StartTimer(true);
	}

	void ActSpawn()
	{
		for (int i = 0; i < m_data.spawnCount; i++)
		{
			Spawn(m_data.angleStep * (i + 1), m_data.radius);
		}
	}

	void Spawn(int angle, int radius)
	{
		Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * radius;
		Character enemy = Instantiate(m_data.ObjectToSpawn, spawnPosition, Quaternion.Euler(0, 180, 0)).GetComponent<Character>();
		var path = m_data.GetPath();
		if (path != null) enemy.gameObject.AddComponent<AiPathMove>().SetPath(path);
		if (m_data.aiData) enemy.BaseAI.Data = m_data.aiData;
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

