using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class Bullet : MonoBehaviour
{
	public float speed = 8f;

	public CharacterData shooter;
	public Transform target;
	// {

	// 	set { m_target = value; }
	// 	get { return m_target; }
	// }

	Transform m_target;

	public bool active = false;
	// Update is called once per frame

	void Update()
	{
		if (target && active && shooter)
		{
			var targetPos = target.position + Vector3.up * 2.0f;
			if (Vector3.SqrMagnitude(targetPos - transform.position) < 1)
			{
				var enemy = target.gameObject.GetComponent<CharacterData>();
				if (enemy)
					shooter.Attack(target.gameObject.GetComponent<CharacterData>());
				active = false;
				Destroy(gameObject, 0.1f);
				return;
			}
			// 计算朝向目标的方向
			Vector3 direction = (targetPos - transform.position).normalized;

			// 使用LookRotation方法设置旋转
			transform.rotation = Quaternion.LookRotation(direction);

			// 移动子弹
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
	}
}
