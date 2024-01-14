using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCodeInternal;

public class UIWindow : MonoBehaviour
{
	public Text winTitle;

	protected CharacterControl m_player;
	protected DigTool m_digtool;
	protected UIManager m_uiRoot;
	protected List<Button> switch_buttons;

	bool m_isOpened;

	void Awake()
	{
		m_uiRoot = UIManager.root;
		m_player = m_uiRoot.player;
		m_digtool = m_uiRoot.digtool;
	}

	void OnEnable()
	{
		OnOpen();
	}

	void OnDisable()
	{
		OnClose();
	}
	public virtual void Close()
	{
		OnClose();
		Animator anim = GetComponent<Animator>();
		if (anim) anim.Play("close");
		else OnClosed();
	}

	protected virtual void OnOpen() { }
	protected virtual void OnOpened() { m_isOpened = true; }
	protected virtual void OnClose() { m_isOpened = false; }
	protected virtual void OnClosed() { gameObject.SetActive(false); }

	public void Back()
	{
		m_uiRoot.BackWindow(this);
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
