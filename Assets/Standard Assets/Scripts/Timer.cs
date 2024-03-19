using System;
using UnityEngine.Events;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public bool autoStart;
	public float timerDuration = 10.0f;
	public int MaxTimes = 1;//maxTimes <= 0  means infinity
	public int LeftTimes { get { return m_leftTimes; } set { m_leftTimes = value; } }
	int m_leftTimes;
	public float cd;
	public float CurCd { get { return m_curCd; } }
	public int TotalBehaved { get { return m_behavedTimes; } }
	public Action<float> waitingAction;
	public Action<float, float> processAction;
	public Action behaveAction;
	public Action endAction;
	public Action refreshAction;
	UISliderHandle m_slider;
	float m_passedTime;
	float m_curCd;
	bool m_started = false;
	int m_behavedTimes;
	public bool isStarted
	{
		get { return m_started; }
	}

	void Awake()
	{
		m_slider = GetComponentInChildren<UISliderHandle>();
		m_leftTimes = MaxTimes;
	}
	void Start()
	{
		if (autoStart) m_started = true;
	}

	public void StartTimer(bool auto = false)
	{
		StartTimer(MaxTimes, auto);
	}
	public void StartTimer(int times, bool auto = false)
	{
		m_leftTimes = MaxTimes = times;
		m_started = true;
		autoStart = auto;
	}
	void Update()
	{
		if (m_started)
		{
			m_passedTime += Time.deltaTime;
			m_slider?.SetValue(timerDuration, m_passedTime);
			if (m_passedTime <= timerDuration)
			{
				processAction?.Invoke(timerDuration, m_passedTime);
			}
			if (m_passedTime >= timerDuration)
			{
				m_started = false;
				m_passedTime = 0f;
				m_leftTimes--;
				m_behavedTimes++;
				behaveAction?.Invoke();
				if (m_leftTimes == 0)
				{
					endAction?.Invoke();
				}
				else if (cd > 0)
				{
					m_curCd = cd;
				}
				else if (cd == 0)
				{
					refreshAction?.Invoke();
					if (autoStart) m_started = true;
				}
			}
		}
		else
		{
			waitingAction?.Invoke(m_curCd);
			if (m_curCd > 0)
			{
				m_curCd -= Time.deltaTime;
				if (m_curCd <= 0 && m_leftTimes > 0) { refreshAction?.Invoke(); if (autoStart) m_started = true; }
			}
		}
	}
}
