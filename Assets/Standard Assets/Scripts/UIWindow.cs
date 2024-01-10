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
	public virtual void Close()
	{
		OnClose();
		Animator anim = GetComponent<Animator>();
		if (anim) anim.Play("close");
		else OnClosed();
	}

	protected virtual void OnOpen() { }
	protected virtual void OnClose() { }
	protected virtual void OnClosed() { gameObject.SetActive(false); }

	public void Back()
	{
		m_uiRoot.BackWindow(this);
	}

	protected void SetButton(Button bt, bool active)
	{
		if (bt.interactable == active) return;

		bt.interactable = active;
		string anim = active ? "move_up" : "move_down";
		Animator animator = bt.GetComponent<Animator>();
		if (animator) animator.Play(anim);

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
