using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSender : MonoBehaviour
{
	public UnityEvent<GameObject, string> m_event;

	public void Send(GameObject sender, string eventMessage)
	{
		m_event.Invoke(sender, eventMessage);
	}
}
