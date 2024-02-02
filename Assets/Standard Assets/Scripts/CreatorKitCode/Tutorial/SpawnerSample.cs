using UnityEngine;
using CreatorKitCode;

public class SpawnerSample : TimerBehaviour
{
	public GameObject ObjectToSpawn;

	public Transform pathRoot;
	public int radius = 5;
	public int angleStep = 15;
	public int spawnNum = 5;
	public bool auto = true;


	private void Start()
	{
		isStarted = auto;
	}
	protected override void OnTimer()
	{
		for (int i = 0; i < spawnNum; i++)
		{
			Spawn(angleStep * (i + 1), radius);
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

	void Spawn(int angle, int radius)
	{
		Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * radius;
		RoleControl enemy = Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity).GetComponent<RoleControl>();
		enemy.BaseAI.SetPath(pathRoot);
	}
}

