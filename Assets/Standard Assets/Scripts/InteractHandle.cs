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
	CharacterData m_monopolist;

	public void Start()
	{
		m_interactor = GetComponent<IInteractable>();
		SetHandle(true);
		slider?.Init(handleTime, 0, "Check...", UISliderHandle.TextType.Text);
	}
	public void SetHandle(bool active)
	{
		if (active)
		{
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
		if (m_monopolist) return;
		CharacterData character = enter.GetComponent<CharacterData>();
		if (character && character.BaseAI) m_interactor.OnInteractorEnter(character);
	}

	void OnInterExit(GameObject exiter)
	{
		if (m_monopolist && exiter == m_monopolist.gameObject) m_monopolist = null;
		if (exiter == Detector.lastInner && slider) slider.SetValue(handleTime, 0);
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (m_monopolist) return;
		CharacterData character = stayer.GetComponent<CharacterData>();
		if (character && character.BaseAI && m_interactor.CanInteract(character))
		{
			m_during += Time.deltaTime;
			slider?.SetValue(handleTime, m_during, "Check...", UISliderHandle.TextType.Text);
			if (m_during >= handleTime)
			{
				if (exclusive && m_monopolist == null)
				{
					m_monopolist = character;
				}
				character.BaseAI.StartInteract(m_interactor);
				m_during = 0;
			}
		}
	}
}
