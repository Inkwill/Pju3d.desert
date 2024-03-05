﻿using UnityEngine;
using CreatorKitCode;

public class SpawnerSample : TimerBehaviour
{
	public GameObject ObjectToSpawn;

	public Transform pathRoot;
	public int radius = 5;
	public int angleStep = 15;
	public int spawnNum = 5;

	protected override void OnBehaved()
	{
		for (int i = 0; i < spawnNum; i++)
		{
			Spawn(angleStep * (i + 1), radius);
		}
	}

	protected override void OnRefresh()
	{
		if (autoStart) StartBehaviour();
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

	void Spawn(int angle, int radius)
	{
		Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * radius;
		CharacterData enemy = Instantiate(ObjectToSpawn, spawnPosition, Quaternion.Euler(0, 180, 0)).GetComponent<CharacterData>();
		if (pathRoot) enemy.gameObject.AddComponent<AiPathMove>().SetPath(pathRoot);
	}
}

