using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UIWindow : MonoBehaviour
{
	public Text winTitle;
	public Image winMask;
	public AudioClip OpenClip;
	public AudioClip CloseClip;
	protected List<Button> switch_buttons;

	void OnEnable()
	{
		if (OpenClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = OpenClip });
		if (winMask) winMask.enabled = false;
		OnOpen();
	}

	void OnDisable()
	{
		if (CloseClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = CloseClip });
		OnClose();
	}
	public void Close()
	{
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

	protected void SwitchButton(Button bt)
	{
		foreach (Button button in switch_buttons)
		{
			SetButton(button, bt == button);
		}
	}

	public virtual void OnButtonClick(string eventName) { }
}
