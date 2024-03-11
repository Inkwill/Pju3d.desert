using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public abstract bool CanInteract(IInteractable target);
	public abstract void OnInteractorEnter(CharacterData character);
	public abstract void InteractWith(IInteractable target);
	public abstract string InteractAnim(CharacterData character);
}
