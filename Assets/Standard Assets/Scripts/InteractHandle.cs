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
	[SerializeField]
	int maxActorCount = 1;
	public UnityEvent EnterEvent;
	public UnityEvent ExitEvent;
	IInteractable m_passive;
	List<IInteractable> m_interactived;

	public void SetHandle(bool active)
	{
		if (active)
		{
			m_passive = GetComponent<IInteractable>();
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
		if (m_interactived.Count >= maxActorCount) return;
		IInteractable interactor = enter.GetComponent<IInteractable>();
		if (interactor != null) { EnterEvent?.Invoke(); }
	}

	void OnInterExit(GameObject exiter)
	{
		IInteractable interactor = exiter.GetComponent<IInteractable>();
		if (interactor != null)
		{
			ExitEvent?.Invoke();
			if (interactor == m_passive.CurrentInteractor) m_passive.CurrentInteractor = null;
			if (m_interactived.Contains(interactor)) m_interactived.Remove(interactor);
		}
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (duration < 0.5f || m_interactived.Count >= maxActorCount) return;
		IInteractable interactor = stayer.GetComponent<IInteractable>();
		if (m_passive.CanInteract(interactor) && !m_interactived.Contains(interactor))
		{
			interactor.InteractWith(m_passive);
			m_interactived.Add(interactor);
		}
	}
}
