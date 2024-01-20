using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
	public UnityEvent<GameObject, string> events;
	public Action<GameObject, string> actions;

	public void Send(GameObject sender, string eventMessage)
	{
		events?.Invoke(sender, eventMessage);
		actions?.Invoke(sender, eventMessage);
	}
}
