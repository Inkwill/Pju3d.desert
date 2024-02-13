using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;

public class InteractSystem : MonoBehaviour
{
	RoleControl m_role;
	public DropBox dropBox;

	void Start()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);
	}

	void OnRoleEvent(GameObject obj, string eventName)
	{
		if (eventName == "roleEvent_OnState_DEAD")
		{
			foreach (var drop in dropBox.GetDropItem())
			{
				DropItem(drop.item, drop.itemNum);
				Debug.Log("drop item : " + drop.item + " num= " + drop.itemNum);
			}
			Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
			StartCoroutine(DestroyCorpse());
		}

	}

	IEnumerator DestroyCorpse()
	{
		VFXManager.PlayVFX(VFXType.SmokePoof, transform.position);
		yield return new WaitForSeconds(1.0f);
		VFXManager.PlayVFX(VFXType.Death, transform.position);
		Destroy(gameObject);
	}

	void DropItem(Item item, int num)
	{
		GameObject lootObj = Resources.Load("Loot") as GameObject;
		Vector3 direction = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.right;
		Vector3 spawnPosition = transform.position + direction * Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			Loot loot = Instantiate(lootObj, spawnPosition, Quaternion.Euler(0, 0, 0)).GetComponent<Loot>();
			loot.Item = item;
		}
	}
}
