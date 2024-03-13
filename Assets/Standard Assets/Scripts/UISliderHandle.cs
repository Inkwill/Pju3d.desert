using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderHandle : MonoBehaviour
{
	public enum TextType
	{
		Countdown,
		Percent,
		Text
	}
	public TextType Type = TextType.Percent;
	public string Prompt;
	Slider m_slider;
	Text m_text;

	void Start()
	{
		m_slider = GetComponent<Slider>();
		m_text = GetComponentInChildren<Text>();
		m_slider.onValueChanged.AddListener(HandleSlider);
		Hide();
	}

	public void Hide()
	{
		SetValue(1, 0);
	}
	public void SetValue(float maxValue, float value)
	{
		m_slider.maxValue = maxValue;
		m_slider.value = value;
		if (value == 0 || value >= m_slider.maxValue) gameObject.SetActive(false);
		else if (!gameObject.activeSelf) gameObject.SetActive(true);
	}

	void HandleSlider(float value)
	{
		if (!m_text) return;
		if (value == 0 || value >= m_slider.maxValue) { m_text.text = ""; return; }
		switch (Type)
		{
			case TextType.Countdown:
				m_text.text = string.Format($"{Prompt}({(m_slider.maxValue - m_slider.value):F0}s)");
				break;
			case TextType.Text:
				m_text.text = Prompt;
				break;
			case TextType.Percent:
				m_text.text = string.Format("{0}({1:F0}%)", Prompt, 100 * m_slider.value / m_slider.maxValue);
				break;
			default:
				break;
		}
	}
}
