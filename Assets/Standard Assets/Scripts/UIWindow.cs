using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;


public class UIWindow : MonoBehaviour
{
	public string winName;
	public Image winMask;
	public AudioClip OpenClip;
	public AudioClip CloseClip;
	Animator m_anim;

	public void Open()
	{
		gameObject.SetActive(true);
		m_anim = GetComponent<Animator>();
		if (OpenClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = OpenClip });
		if (winMask) winMask.enabled = false;

		if (m_anim)
		{
			m_anim.SetTrigger("open");
			GameManager.StartWaitAction(0.2f, () => { OnOpened(); if (winMask) winMask.enabled = true; });
		}
		OnOpen();
		GameManager.GameUI.winOpenAction?.Invoke(this);
	}
	void OnDisable()
	{
		OnClosed();
	}
	public void Close()
	{
		if (!gameObject.activeSelf) return;
		if (winMask) winMask.enabled = false;
		if (CloseClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = CloseClip });
		OnClose();
		GameManager.GameUI.winCloseAction?.Invoke(this);
		if (m_anim) { m_anim.SetTrigger("close"); GameManager.StartWaitAction(0.2f, () => { gameObject.SetActive(false); }); }
		else gameObject.SetActive(false);
	}
	protected virtual void OnOpen() { }
	protected virtual void OnOpened() { }
	protected virtual void OnClose() { }
	protected virtual void OnClosed() { }


	public void Back()
	{
		GameManager.GameUI.BackWindow(this);
	}

	public void BackToMain()
	{
		GameManager.GameUI.CloseAll();
		GameManager.GameUI.OpenWindow("winMain");
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

	public void Switch()
	{
		if (gameObject.activeSelf) Close();
		else Open();
	}
}
