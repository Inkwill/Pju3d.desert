using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHudBase : MonoBehaviour
{
	[SerializeField]
	GameObject uiHudRoot;
	[SerializeField]
	Animator bubble_anim;
	[SerializeField]
	Text bubble_text;
	bool m_bubble;
	string m_storeBubble;
	float m_bubbleDuration;
	Transform m_worldRoot;
	Vector3 m_worldScale;
	bool m_uiShow;
	void Start()
	{
		m_worldRoot = transform.parent;
		m_worldScale = transform.localScale;
	}
	public void ShowWorldHud()
	{
		transform.SetParent(m_worldRoot);
		transform.localScale = m_worldScale;
		transform.forward = Camera.main.transform.forward;
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(true);
		m_uiShow = false;
	}
	public void ShowUIHud()
	{
		transform.SetParent(GameManager.GameUI.transform);
		transform.position = Camera.main.WorldToScreenPoint(m_worldRoot.position);
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		gameObject.SetActive(true);
		m_uiShow = true;
	}
	private void Update()
	{
		if (m_uiShow) transform.position = Camera.main.WorldToScreenPoint(m_worldRoot.position);
		else transform.forward = Camera.main.transform.forward;

		if (bubble_anim)
		{
			if (m_bubbleDuration > 0) m_bubbleDuration -= Time.deltaTime;
			if (m_bubbleDuration <= 0f) { bubble_anim.SetBool("show", false); m_bubble = false; m_storeBubble = ""; }
		}
	}

	public void Bubble(string content)
	{
		if (m_bubble)
		{
			if (m_storeBubble != content)
			{
				GameManager.StartWaitAction(2.0f, () => { Bubble(content); });
				m_storeBubble = content;
			}
		}
		else if (content != m_storeBubble)
		{
			m_bubble = true;
			m_bubbleDuration = 2.0f;
			if (!bubble_anim.gameObject.activeSelf) bubble_anim.gameObject.SetActive(true);
			else bubble_anim.SetBool("show", true);
			StartCoroutine(ShowText(content));
		}
	}

	IEnumerator ShowText(string content)
	{
		for (int i = 0; i < content.Length; i++)
		{
			bubble_text.text = content.Substring(0, i + 1);
			m_bubbleDuration += 0.02f;
			yield return new WaitForSeconds(0.02f);
		}
		m_bubble = false;
		m_storeBubble = content;
	}
}
