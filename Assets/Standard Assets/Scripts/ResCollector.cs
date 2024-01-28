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

    void Start()
    {
        m_Inventory = GetComponent<CharacterData>().Inventory;
        Debug.Log("m_Inventory = " + m_Inventory);
        m_Inventory.Actions += OnItemEvent;
    }

    void OnItemEvent(InventorySystem.InventoryEntry itemEntry, string eventName)
    {
        ResItem r_item = itemEntry.Item as ResItem;
        if (r_item && r_item.Type == ResType)
        {
            if (eventName == "Add")
            {
                GameObject resObj = Instantiate(r_item.WorldObjectPrefab, root, false);
                resObj.transform.position += new Vector3(0, (itemEntry.Count - 1) * 0.2f, 0);
            }
            else if (eventName == "Remove")
            {
                Transform[] trans = root.GetComponentsInChildren<Transform>();
                Destroy(trans[trans.Length - 1].gameObject);
            }
        }
        Debug.Log("OnItemEvent: item = " + itemEntry.Item + "event = " + eventName);
    }
}
