using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;

[RequireComponent(typeof(CharacterData))]
public class ResCollector : MonoBehaviour
{
	public ResItem.ResType ResType;
	public Transform root;
	InventorySystem m_Inventory;
	int m_Count;
	void Start()
	{
		m_Inventory = GetComponent<CharacterData>().Inventory;
		//Debug.Log("m_Inventory = " + m_Inventory);
		m_Inventory.Actions += OnItemEvent;
		m_Count = 0;
	}

	void OnItemEvent(string itemName, string eventName)
	{
		ResItem r_item = KeyValueData.GetValue<Item>(GameManager.Data.Item, itemName) as ResItem;
		if (r_item && r_item.Type == ResType)
		{
			if (eventName == "Add")
			{
				GameObject resObj = Instantiate(r_item.WorldObjectPrefab, root, false);
				resObj.transform.position += new Vector3(0, m_Count * 0.2f, 0);
				m_Count++;
			}
			else if (eventName == "Minus")
			{
				Transform[] trans = root.GetComponentsInChildren<Transform>();
				Destroy(trans[m_Count].gameObject);
				m_Count--;
			}
			Debug.Log("OnItemEvent: item = " + itemName + "event = " + eventName);
		}
	}



	// void RefreshResHeight(ResItem ritem)
	// {
	// 	Transform[] trans = root.GetComponentsInChildren<Transform>();
	// 	int resCount = m_Inventory.ItemCount(ritem.ItemName);
	// 	for (int i = 0; i < resCount; i++)
	// 	{
	// 		if (trans.Length > i + 1) trans[i + 1].position += new Vector3(0, i * 0.2f, 0);
	// 		else
	// 		{
	// 			GameObject resObj = Instantiate(ritem.WorldObjectPrefab, root, false);
	// 			resObj.transform.position += new Vector3(0, i * 0.2f, 0);
	// 		}
	// 	}
	// 	for (int i = resCount; i < trans.Length - 1; i++)
	// 	{
	// 		Destroy(trans[i].gameObject);
	// 	}
	// }
}
