using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractOnTrigger : MonoBehaviour
{
	public LayerMask layers;
	public UnityEvent<GameObject> OnEnter, OnExit;
	public UnityEvent<GameObject, string> OnEvent;

	public GameObject lastInner { get; private set; }
	public GameObject lastExiter { get; private set; }

	[SerializeField]
	bool once;

	List<GameObject> interObjects;
	Collider m_collider;

	void Start()
	{
		m_collider = GetComponent<Collider>();
		m_collider.isTrigger = true;
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
			lastInner.GetComponent<EventSender>()?.events.AddListener(OnInterEvent);
		}
		OnEnter?.Invoke(enter);
		if (once) GetComponent<Collider>().enabled = false;
	}

	void OnTriggerExit(Collider other)
	{

		if (interObjects.Contains(other.gameObject))
		{
			lastExiter = other.gameObject;
			ExecuteOnExit(lastExiter);
		}
	}

	protected virtual void ExecuteOnExit(GameObject exiter)
	{
		interObjects.Remove(exiter);
		if (exiter == lastInner)
		{
			lastInner = interObjects.Count > 0 ? interObjects[interObjects.Count - 1] : null;
		}
		exiter.GetComponent<EventSender>()?.events.RemoveListener(OnInterEvent);
		OnExit?.Invoke(exiter);
	}

	public GameObject GetNearest()
	{
		if (interObjects.Count == 0) return null;

		GameObject nearest = interObjects[0];
		float nearestDistance = Vector3.Distance(transform.position, nearest.transform.position);

		for (int i = 1; i < interObjects.Count; i++)
		{
			float distance = Vector3.Distance(transform.position, interObjects[i].transform.position);

			if (distance < nearestDistance)
			{
				nearest = interObjects[i];
				nearestDistance = distance;
			}
		}
		return nearest;
	}

	void OnInterEvent(GameObject sender, string eventMessage)
	{
		OnEvent?.Invoke(sender, eventMessage);
		if (eventMessage == "roleEvent_OnState_DEAD" && interObjects.Contains(sender))
		{
			ExecuteOnExit(sender);
		}
	}
}

