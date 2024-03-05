using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractProducer : MonoBehaviour, IInteractable
{
	public UnityEvent<GameObject, GameObject> InteractEvent;
	public UnityEvent<GameObject> ExhaustedEvent;
	int m_maxCount = 10;
	public bool CanInteract(CharacterData character)
	{
		return character.BaseAI.isIdle && m_maxCount > 0;
	}

	public void OnInteractorEnter(CharacterData character)
	{

	}

	public void InteractWith(CharacterData character)
	{
		InteractEvent?.Invoke(gameObject, character.gameObject);
		Helpers.Log(this, "InteractWith", character.CharacterName);
		if (--m_maxCount < 1) OnExhausted(character);
	}

	void OnExhausted(CharacterData character)
	{
		GetComponent<InteractHandle>()?.SetHandle(false);
		ExhaustedEvent?.Invoke(gameObject);
	}
}
