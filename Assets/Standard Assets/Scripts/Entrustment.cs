using System;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entrustment : IComparable
{
    StoryTeller host;
    string details;
    InventorySystem.ItemDemand m_itemDemand;
    public int FriendlyRequest;


    public Entrustment(StoryTeller role)
    {
        host = role;
        FriendlyRequest = Random.Range(0, GameManager.StoryListener.GetFriendlyValue(role.RoleName) + 3);
    }

    public void AddDemand(List<KeyValueData.KeyValue<Item, int>> data)
    {
        m_itemDemand = new InventorySystem.ItemDemand(KeyValueData.ToDic(data));
    }

    int IComparable.CompareTo(object target)
    {
        Entrustment entrust = (Entrustment)target;
        return FriendlyRequest - entrust.FriendlyRequest;
    }


}
