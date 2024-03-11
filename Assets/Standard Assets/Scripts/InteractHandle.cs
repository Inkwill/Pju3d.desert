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
	[SerializeField]
	UISliderHandle slider;
	float m_during;
	IInteractable m_interactor;
	IInteractable m_monopolist;

	public void SetHandle(bool active)
	{
		if (active)
		{
			m_interactor = GetComponent<IInteractable>();
			slider?.Init(handleTime, 0, "Check...", UISliderHandle.TextType.Text);
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
		CharacterData character = enter.GetComponent<CharacterData>();
		if (character && m_interactor is ItemDemander)
		{
			var demander = m_interactor as ItemDemander;
			demander.OnInteractorEnter(character);
		}
	}

	void OnInterExit(GameObject exiter)
	{
		if (m_monopolist != null && exiter.GetComponent<IInteractable>() == m_monopolist) m_monopolist = null;
		if (exiter == Detector.lastInner && slider) slider.SetValue(handleTime, 0);
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (m_monopolist != null || duration < 0.5f) return;
		var target = stayer.GetComponent<IInteractable>();
		if (m_interactor.CanInteract(target))
		{
			m_during += Time.deltaTime;
			slider?.SetValue(handleTime, m_during, "Check...", UISliderHandle.TextType.Text);
			if (m_during >= handleTime)
			{
				if (exclusive && m_monopolist == null)
				{
					m_monopolist = target;
				}
				target.InteractWith(m_interactor);
				m_during = 0;
			}
		}
	}
}
