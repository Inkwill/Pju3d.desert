using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class Bullet : MonoBehaviour
{
	public float speed = 8f;
	public Damage damage;
	public Character target;

	void Update()
	{
		if (target != null && damage != null)
		{
			var targetPos = target.transform.position + Vector3.up * 1.0f;
			if (Vector3.SqrMagnitude(targetPos - transform.position) < 1)
			{
				damage.TakeDamage();
				Destroy(gameObject);
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
