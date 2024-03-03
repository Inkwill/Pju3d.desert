using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractorTree : MonoBehaviour, IInteractable
{
	public UnityEvent<GameObject, GameObject> InteractEvent;
	int m_maxCount = 10;
	public virtual bool CanInteract(CharacterData character)
	{
		return character && character.BaseAI.isIdle;
	}

	public void InteractWith(CharacterData character)
	{
		InteractEvent?.Invoke(gameObject, character.gameObject);
		Helpers.Log(this, "InteractWith", character.CharacterName);
	}

	public void InteractFail(CharacterData character)
	{
		Helpers.Log(this, "InteractWith", "Fail with " + character.CharacterName);
	}
}
