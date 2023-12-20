using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerBehaviour : MonoBehaviour
{
	public float interval = 1.0f;
	public int times = -1;  //times <= 0  means loop
	public bool auto = true;
	public Slider progressSlider;
	float timer = 0f;
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
	private void Start()
	{
		isStarted = auto;
	}

	void Update()
	{
		if (m_started)
		{
			timer += Time.deltaTime;
			if (timer >= interval)
			{
				OnTimer();
				timer = 0f;
				times--;
				if (times == 0) isStarted = false;
			}
			else
			{
				OnProcessing(Time.deltaTime);
				if (progressSlider) progressSlider.value = timer / interval;
			}
		}
	}

	public virtual void OnTimer() { }
	public virtual void OnProcessing(float completed) { }
	public virtual void OnStart() { }
	public virtual void OnEnd() { }
}
