using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractOnTrigger : MonoBehaviour
{
	public LayerMask layers;
	public float Radius
	{
		get
		{
			if (m_capCollider) return m_capCollider.radius;
			else return m_boxCollier.size.y;
		}
		set
		{
			if (m_capCollider) m_capCollider.radius = value;
			else m_boxCollier.size = new Vector3(value, value, value);
		}
	}
	public UnityEvent<GameObject> OnEnter, OnExit, OnStay;
	public UnityEvent<GameObject, string> OnEvent;

	public List<GameObject> Inners { get { return interObjects; } }
	public GameObject lastInner { get; private set; }
	public GameObject lastExiter { get; private set; }

	[SerializeField]
	bool once;

	List<GameObject> interObjects;
	CapsuleCollider m_capCollider;
	BoxCollider m_boxCollier;
	float m_stayDuring;

	void Start()
	{
		m_capCollider = GetComponent<CapsuleCollider>();
		m_boxCollier = GetComponent<BoxCollider>();
		if (m_capCollider) m_capCollider.isTrigger = true;
		if (m_boxCollier) m_boxCollier.isTrigger = true;
		interObjects = new List<GameObject>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == gameObject) return;

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

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject == gameObject) return;

		if (0 != (layers.value & 1 << other.gameObject.layer) && !interObjects.Contains(other.gameObject))
		{
			OnTriggerEnter(other);
		}
		else if (other.gameObject == lastInner)
		{
			m_stayDuring += Time.deltaTime;
			if (m_stayDuring >= 1.0f)
			{
				OnStay?.Invoke(other.gameObject);
				m_stayDuring = 0;
			}
		}
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

