using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(IInteractable))]
public class InteractHandle : MonoBehaviour
{
	[SerializeField]
	InteractOnTrigger Detector;
	public UnityEvent EnterEvent;
	public UnityEvent ExitEvent;
	IInteractable m_host;
	List<IInteractable> m_interactived;

	public void SetHandle(bool active)
	{
		if (active)
		{
			m_host = GetComponent<IInteractable>();
			m_interactived = new List<IInteractable>();
			Detector.OnEnter.AddListener(OnInterEnter);
			Detector.OnExit.AddListener(OnInterExit);
			Detector.OnStay.AddListener(OnInterStay);
		}
		else
		{
			m_interactived?.Clear();
			Detector.OnEnter.RemoveListener(OnInterEnter);
			Detector.OnExit.RemoveListener(OnInterExit);
			Detector.OnStay.RemoveListener(OnInterStay);
		}
	}

	void OnInterEnter(GameObject enter)
	{
		if (m_interactived.Count >= m_host.Data.maxActorCount) return;
		IInteractable interactor = enter.GetComponent<IInteractable>();
		if (interactor != null) { EnterEvent?.Invoke(); }
	}

	void OnInterExit(GameObject exiter)
	{
		IInteractable interactor = exiter.GetComponent<IInteractable>();
		if (interactor != null)
		{
			ExitEvent?.Invoke();
			if (interactor == m_host.CurrentInteractor) m_host.CurrentInteractor = null;
			if (m_interactived.Contains(interactor)) m_interactived.Remove(interactor);
		}
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (duration < 0.5f || m_interactived.Count >= m_host.Data.maxActorCount) return;
		IInteractable interactor = stayer.GetComponent<IInteractable>();
		if (m_host.CanInteract(interactor) && !m_interactived.Contains(interactor))
		{
			interactor.InteractWith(m_host);
			m_interactived.Add(interactor);
		}
	}
}
