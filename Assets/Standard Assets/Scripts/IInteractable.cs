using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public abstract bool CanInteract(CharacterData character);
	public abstract void OnInteractorEnter(CharacterData character);
	public abstract void InteractWith(CharacterData character);
	public abstract string InteractAnim(CharacterData character);
}
