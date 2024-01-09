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

	public virtual void OnOpen() { }
	public virtual void OnClose() { }
	public virtual void OnClosed() { gameObject.SetActive(false); }

	public void Back()
	{
		m_uiRoot.BackWindow(this);
	}
}
