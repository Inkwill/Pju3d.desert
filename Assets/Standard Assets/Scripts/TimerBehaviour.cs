using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerBehaviour : MonoBehaviour
{
	public float interval = 1.0f;
	public float during = 10.0f;
	public int times = -1;  //times <= 0  means loop
	public bool auto = true;
	public Slider progressSlider;
	float m_curTimer = 0f;
	float m_step = 0f;
	bool m_started = false;
	public bool isStarted
	{
		get { return m_started; }
		set
		{
			m_started = value;
			if (progressSlider) progressSlider.gameObject.SetActive(value);
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
	// private void Start()
	// {
	// 	isStarted = auto;
	// }

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
				if (times == 0) isStarted = false;
			}
			else if (m_step >= interval)
			{
				OnProcessing(m_curTimer / during);
				m_step = 0f;
			}
			if (progressSlider) progressSlider.value = m_curTimer / during;
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
