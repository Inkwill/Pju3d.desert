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

	public GameObject lastInteracter;

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
		lastInteracter = enter;
		if (!interObjects.Contains(lastInteracter))
		{
			interObjects.Add(lastInteracter);
			lastInteracter.GetComponent<EventSender>()?.m_event.AddListener(OnInterEvent);
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
			ExecuteOnExit(other.gameObject);
		}
		//}
	}

	protected virtual void ExecuteOnExit(GameObject exiter)
	{
		lastInteracter = exiter;
		interObjects.Remove(lastInteracter);
		lastInteracter.GetComponent<EventSender>()?.m_event.RemoveListener(OnInterEvent);
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
			if (lastInteracter == sender) lastInteracter = null;
			Debug.Log("sender is Dead :" + sender);
		}
	}

	public GameObject GetSceneBox()
	{
		foreach (GameObject sceneBox in interObjects)
		{
			if (sceneBox)
			{
				return sceneBox;
			}
			else
			{
				interObjects.Remove(sceneBox);
			}
		}
		return null;
	}
}

