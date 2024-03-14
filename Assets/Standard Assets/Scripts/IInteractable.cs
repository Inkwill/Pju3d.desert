using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
	public abstract IInteractable CurrentInteractor { get; set; }
	public abstract bool CanInteract(IInteractable target);
	public abstract void InteractWith(IInteractable target);
	public abstract InteractData Data { get; }

}
