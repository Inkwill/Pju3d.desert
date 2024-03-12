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
	string m_prompt;
	Slider slider;
	Text m_text;

	public void Init(float maxValue, float value = 0, string prompt = "", TextType type = TextType.Percent)
	{
		if (slider == null)
		{
			slider = GetComponent<Slider>();
			m_text = GetComponentInChildren<Text>();
			slider.onValueChanged.AddListener(HandleSlider);
		}
		SetValue(maxValue, value, prompt, type);
	}

	public void SetValue(float maxValue, float value, string content = "", TextType type = TextType.Percent)
	{
		slider.maxValue = maxValue;
		slider.value = value;
		m_prompt = content;
		Type = type;
		if (value == 0 || value >= slider.maxValue)
		{
			gameObject.SetActive(false);
			m_text.text = "";
		}
		else if (!gameObject.activeSelf) gameObject.SetActive(true);
	}

	void HandleSlider(float value)
	{
		if (!m_text) return;
		if (value == 0 || value >= slider.maxValue) { m_text.text = ""; return; }
		switch (Type)
		{
			case TextType.Countdown:
				m_text.text = string.Format($"{m_prompt}({(slider.maxValue - slider.value):F0}s)");
				break;
			case TextType.Text:
				m_text.text = m_prompt;
				break;
			case TextType.Percent:
				m_text.text = string.Format("{0}({1:F0}%)", m_prompt, 100 * slider.value / slider.maxValue);
				break;
			default:
				break;
		}
	}
}
