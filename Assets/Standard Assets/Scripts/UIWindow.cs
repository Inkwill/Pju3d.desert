using System;
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
	public float openDuring = 0.2f;
	public float closeDuring = 0.2f;
	Animator m_anim;
	void Awake()
	{
		m_anim = GetComponent<Animator>();
	}
	public void Open()
	{
		gameObject.SetActive(true);
		OnOpen();
		if (OpenClip) SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() { Clip = OpenClip });
		if (winMask) winMask.enabled = false;
		GameManager.GameUI.winOpenAction?.Invoke(this);
		GameManager.StartWaitAction(openDuring, () => { OnOpened(); if (winMask) winMask.enabled = true; });
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
		m_anim?.SetTrigger("close");
		GameManager.StartWaitAction(closeDuring, () => { gameObject.SetActive(false); });
	}
	protected virtual void OnOpen() { }
	protected virtual void OnOpened() { }
	protected virtual void OnClose() { }
	protected virtual void OnClosed() { }

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
