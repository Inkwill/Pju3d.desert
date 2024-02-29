using UnityEngine;
using CreatorKitCode;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterData))]
public class ResCollector : MonoBehaviour
{
	public ResItem.ResType ResType;
	public Transform root;
	int m_Count;
	CharacterData m_character;
	void Start()
	{
		m_character = GetComponent<CharacterData>();
		//Debug.Log("m_Inventory = " + m_Inventory);
		m_character.Inventory.ItemEvent += OnItemEvent;
		m_Count = 0;
	}

	void OnItemEvent(Item item, string eventName, int itemCount)
	{
		ResItem r_item = item as ResItem;
		if (r_item && r_item.Type == ResType)
		{
			if (eventName == "Add")
			{
				for (int i = 0; i < itemCount; i++)
				{
					GameObject resObj = Instantiate(r_item.WorldObjectPrefab, root, false);
					resObj.transform.position += new Vector3(0, m_Count * 0.2f, 0);
					m_Count++;
				}
			}
			else if (eventName == "Minus")
			{
				Transform[] trans = root.GetComponentsInChildren<Transform>();
				for (int i = 0; i < itemCount; i++)
				{
					Destroy(trans[m_Count].gameObject);
					m_Count--;
				}
			}
			// else if (eventName == "Fulfill")
			// {
			// 	for (int i = 0; i < itemCount; i++)
			// 	{
			// 		GameObject resObj = Instantiate(r_item.WorldObjectPrefab, root, false);
			// 		resObj.transform.DOMove(resObj.transform.position + new Vector3(0, 4, 0), 2);

			// 	}
			// }
		}
	}
	public void Display(Item item, int count, Action action)
	{

		Debug.Log("ResCollector display item= " + item.ItemName + " count= " + count.ToString());
		StartCoroutine(DisplayAction(action));
	}

	IEnumerator DisplayAction(Action action)
	{
		yield return new WaitForSeconds(2.0f);
		action?.Invoke();
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
