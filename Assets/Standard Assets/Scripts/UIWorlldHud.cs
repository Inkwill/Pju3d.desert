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
	bool m_bubble;
	float m_bubbleDuration = 1.0f;
	private void Update()
	{
		this.transform.forward = Camera.main.transform.forward;
		if (m_bubble) m_bubbleDuration -= Time.deltaTime;
		if (m_bubbleDuration <= 0f) { bubble_anim.SetBool("show", false); m_bubble = false; }
	}

	public void Bubble(string content)
	{
		if (m_bubble) return;
		m_bubble = true;
		m_bubbleDuration = 1.0f;
		if (!bubble_anim.gameObject.activeSelf) bubble_anim.gameObject.SetActive(true);
		else if (!bubble_anim.GetBool("show")) bubble_anim.SetBool("show", true);
		StartCoroutine(ShowText(content));
	}

	IEnumerator ShowText(string content)
	{
		for (int i = 0; i < content.Length; i++)
		{
			bubble_text.text = content.Substring(0, i + 1);
			m_bubbleDuration += 0.05f;
			yield return new WaitForSeconds(0.05f);
		}
	}
}
