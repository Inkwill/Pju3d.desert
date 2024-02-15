using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public abstract bool interactable { get; }
    public abstract void InteractWith(GameObject target);
}
