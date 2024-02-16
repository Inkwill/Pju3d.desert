using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerBehaviour : MonoBehaviour
{
	public float during = 10.0f;
	float interval = 1.0f;
	//public virtual int TotalTimes { get { return times; } }
	public int times = -1;  //times <= 0  means loop
	public Slider progressSlider;
	public bool destroyOnEnd;
	float m_curTimer = 0f;
	float m_step = 0f;
	bool m_started = false;
	public bool isStarted
	{
		get { return m_started; }
		set
		{
			if (times == 0 && value) return;
			m_started = value;
			if (progressSlider)
			{
				progressSlider.gameObject.SetActive(value);
				progressSlider.maxValue = during;
				progressSlider.value = m_curTimer;
			}
			if (m_started)
			{
				OnStart();
			}
			else
			{
				OnEnd();
			}
		}
	}

	void Update()
	{
		m_step += Time.deltaTime;
		if (m_started)
		{
			m_curTimer += Time.deltaTime;

			if (m_curTimer >= during)
			{
				OnTimer();
				m_curTimer = 0f;
				times--;
				if (times == 0)
				{
					isStarted = false;
					if (destroyOnEnd) Destroy(gameObject);
				}

			}
			else if (m_step >= interval)
			{
				OnProcessing(m_curTimer / during);
				m_step = 0f;
			}
			if (progressSlider)
			{
				if (!progressSlider.gameObject.activeSelf) progressSlider.gameObject.SetActive(true);
				progressSlider.value = m_curTimer;
			}
		}
		else if (m_step >= interval)
		{
			OnInterval();
			m_step = 0f;
		}
	}

	protected virtual void OnTimer() { }
	protected virtual void OnProcessing(float completed) { }
	protected virtual void OnStart() { }
	protected virtual void OnEnd() { }

	protected virtual void OnInterval() { }
}
