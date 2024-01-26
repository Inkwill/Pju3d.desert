using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
	public UnityEvent<GameObject, string> events;

	public void Send(GameObject sender, string eventMessage)
	{
		events?.Invoke(sender, eventMessage);
	}
}
