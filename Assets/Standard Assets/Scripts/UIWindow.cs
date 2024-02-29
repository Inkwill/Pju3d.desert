using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UIWindow : MonoBehaviour
{
	public string winName;
	public UIWindow Instance;
	public Image winMask;
	public AudioClip OpenClip;
	public AudioClip CloseClip;

	void OnEnable()
	{
		if (OpenClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = OpenClip });
		if (winMask) winMask.enabled = false;
		Instance = this;
		OnOpen();
	}

	void OnDisable()
	{
		if (CloseClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = CloseClip });
		OnClose();
	}
	public void Close()
	{
		if (!gameObject.activeSelf) return;
		if (winMask) winMask.enabled = false;
		Animator anim = GetComponent<Animator>();
		if (anim) anim.SetTrigger("close");
		else OnClosed();
	}
	protected virtual void OnOpen() { }
	protected virtual void OnOpened() { if (winMask) winMask.enabled = true; }
	protected virtual void OnClose() { }
	void OnClosed() { gameObject.SetActive(false); }

	public void Back()
	{
		GameManager.GameUI.BackWindow(this);
	}

	public void BackToMain()
	{
		GameManager.GameUI.SwitchWindow("winMain");
	}

	public static void SetButton(Button bt, bool active)
	{
		if (bt.interactable == active) return;
		if (!bt.gameObject.activeSelf) bt.gameObject.SetActive(true);

		bt.interactable = active;
		//string state = active ? "show" : "hide";
		Animator animator = bt.GetComponent<Animator>();
		if (animator) animator.SetBool("show", active);//animator.Play(anim);

		// Image[] imgs = bt.gameObject.GetComponentsInChildren<Image>();
		// foreach (Image img in imgs)
		// {
		// 	img.color = active ? Color.red : Color.gray;
		// }
	}

	public virtual void OnButtonClick(string eventName) { }
}
