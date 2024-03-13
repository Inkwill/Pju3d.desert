using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldHud : MonoBehaviour
{
	[SerializeField]
	Animator bubble_anim;
	[SerializeField]
	Text bubble_text;
	[SerializeField]
	GameObject uiPrefab;
	GameObject m_uiHud;
	bool m_bubble;
	string m_storeBubble;
	float m_bubbleDuration;
	public void ShowUIHud()
	{
		if (m_uiHud == null)
		{
			m_uiHud = Instantiate(uiPrefab, GameManager.GameUI.transform);
			m_uiHud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
			m_uiHud.gameObject.SetActive(true);
		}
		else m_uiHud.gameObject.SetActive(true);
	}

	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
		if (m_uiHud) m_uiHud.transform.position = Camera.main.WorldToScreenPoint(transform.position);
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
