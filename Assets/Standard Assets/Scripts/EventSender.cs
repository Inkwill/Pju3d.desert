using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
	public enum EventType
	{
		DemandComplete
	}
	public UnityEvent<GameObject, string> events;
	public UnityEvent<EventType, string, int> countEvent;

	public void Send(GameObject sender, string eventMessage)
	{
		events?.Invoke(sender, eventMessage);
	}

	public void Count(EventType eventType, string key, int count = 1)
	{
		countEvent?.Invoke(eventType, key, count);
	}
}
