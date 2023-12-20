using UnityEngine;
using CreatorKitCode;

namespace CreatorKitCodeInternal
{
	public class SpawnerSample : TimerBehaviour
	{
		public SimpleEnemyController ObjectToSpawn;

		public Transform pathRoot;
		public int radius = 5;
		public int angleStep = 15;
		public int spawnNum = 5;

		public override void OnTimer()
		{
			for (int i = 0; i < spawnNum; i++)
			{
				SpawnPotion(angleStep * (i + 1), radius);
			}
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

		void SpawnPotion(int angle, int radius)
		{

			Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
			Vector3 spawnPosition = transform.position + direction * radius;
			SimpleEnemyController enemy = Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity) as SimpleEnemyController;
			enemy.pathRoot = pathRoot;
		}
	}
}
