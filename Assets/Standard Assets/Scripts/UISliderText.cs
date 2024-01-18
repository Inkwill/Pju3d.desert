using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderText : MonoBehaviour
{
	public enum TextType
	{
		Current,
		Left,
		Percent
	}
	public TextType type = TextType.Percent;
	public string form = "{0:F0}%";
	Slider slider;
	Text m_text;

	void Start()
	{
		m_text = GetComponentInChildren<Text>();
		slider = GetComponent<Slider>();
		slider.onValueChanged.AddListener(SetText);
		SetText(slider.value);
	}

	void SetText(float value)
	{
		if (!m_text) return;
		switch (type)
		{
			case TextType.Left:
				m_text.text = string.Format(form, slider.maxValue - slider.value);
				break;
			case TextType.Current:
				m_text.text = string.Format(form, slider.value);
				break;
			case TextType.Percent:
				m_text.text = string.Format(form, 100 * slider.value / slider.maxValue);
				break;
			default:
				break;
		}
	}
}
