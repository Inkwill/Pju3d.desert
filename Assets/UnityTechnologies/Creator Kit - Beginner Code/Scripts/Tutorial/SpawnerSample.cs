using UnityEngine;
using CreatorKitCode;

public class SpawnerSample : MonoBehaviour
{
	public GameObject ObjectToSpawn;
	public int radius = 5;
	public int angleStep = 15;
	public int lootNum = 5;

	void Start()
	{
		for (int i = 0; i < lootNum; i++)
		{
			SpawnPotion(angleStep * (i + 1), radius);

		}
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
	}

	void SpawnPotion(int angle, int radius)
	{

		Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * radius;
		Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);

	}
}

