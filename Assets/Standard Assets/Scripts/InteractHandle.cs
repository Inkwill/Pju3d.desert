using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(IInteractable))]
public class InteractHandle : MonoBehaviour
{
	[SerializeField]
	InteractOnTrigger Detector;
	[SerializeField]
	float handleTime = 1.0f;
	[SerializeField]
	bool exclusive;
	public UnityEvent EnterEvent;
	public UnityEvent ExitEvent;
	IInteractable m_interactor;
	IInteractable m_monopolist;

	public void SetHandle(bool active)
	{
		if (active)
		{
			m_interactor = GetComponent<IInteractable>();
			Detector.OnEnter.AddListener(OnInterEnter);
			Detector.OnExit.AddListener(OnInterExit);
			Detector.OnStay.AddListener(OnInterStay);
		}
		else
		{
			Detector.OnEnter.RemoveListener(OnInterEnter);
			Detector.OnExit.RemoveListener(OnInterExit);
			Detector.OnStay.RemoveListener(OnInterStay);
		}
	}

	void OnInterEnter(GameObject enter)
	{
		if (m_monopolist != null) return;
		IInteractable interactor = enter.GetComponent<IInteractable>();
		if (interactor != null) EnterEvent?.Invoke();
	}

	void OnInterExit(GameObject exiter)
	{
		IInteractable interactor = exiter.GetComponent<IInteractable>();
		if (interactor != null)
		{
			ExitEvent?.Invoke();
			if (interactor == m_monopolist) m_monopolist = null;
			if (interactor == m_interactor.CurrentInteractor) m_interactor.CurrentInteractor = null;
		}
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (m_monopolist != null || duration < handleTime) return;
		IInteractable interactor = stayer.GetComponent<IInteractable>();
		if (m_interactor.CanInteract(interactor))
		{
			interactor.InteractWith(m_interactor);
			if (exclusive && m_monopolist == null) m_monopolist = interactor;
		}
	}
}
