using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCodeInternal;

public class UIWindow : MonoBehaviour
{

	public Text winTitle;
	public CharacterControl player;

	private void Start()
	{
		player = CharacterControl.Instance;
	}

	public void Close()
	{
		Animator anim = GetComponent<Animator>();
		if (anim) anim.Play("close");
		else OnClosed();
	}
	public void OnClosed()
	{
		gameObject.SetActive(false);
	}
}
