using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractOnTrigger : MonoBehaviour
{
	public LayerMask layers;
	public bool once;
	public UnityEvent<string> OnEnter, OnExit;

	public GameObject lastInner;
	public GameObject lastExiter;

	List<GameObject> interObjects;
	new Collider collider;

	void Start()
	{
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
		interObjects = new List<GameObject>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (0 != (layers.value & 1 << other.gameObject.layer))
		{
			ExecuteOnEnter(other.gameObject);
		}
	}

	protected virtual void ExecuteOnEnter(GameObject enter)
	{
		lastInner = enter;
		if (!interObjects.Contains(lastInner))
		{
			interObjects.Add(lastInner);
			lastInner.GetComponent<EventSender>()?.m_event.AddListener(OnInterEvent);
		}
		OnEnter.Invoke("enter");
		if (once) collider.enabled = false;
	}

	void OnTriggerExit(Collider other)
	{
		//if (0 != (layers.value & 1 << other.gameObject.layer))
		//{
		if (interObjects.Contains(other.gameObject))
		{
			lastExiter = other.gameObject;
			ExecuteOnExit(lastExiter);
		}
		//}
	}

	protected virtual void ExecuteOnExit(GameObject exiter)
	{
		if (exiter == lastInner) lastInner = null;
		interObjects.Remove(exiter);
		exiter.GetComponent<EventSender>()?.m_event.RemoveListener(OnInterEvent);
		OnExit.Invoke("exit");
	}

	public GameObject GetIntruder()
	{
		foreach (GameObject intruder in interObjects)
		{
			if (intruder)
			{
				return intruder;
			}
			else
			{
				interObjects.Remove(intruder);
			}
		}
		return null;
	}

	void OnInterEvent(GameObject sender, string eventMessage)
	{
		//Debug.Log("OnInterEvent sender= " + sender + "eventMessage = " + eventMessage);
		if (eventMessage == "Dead" && interObjects.Contains(sender))
		{
			ExecuteOnExit(sender);
			if (lastInner == sender) lastInner = null;
			Debug.Log("interObj is Dead :" + sender);
		}
	}
}

