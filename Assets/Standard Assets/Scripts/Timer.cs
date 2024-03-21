using System;
using UnityEngine.Events;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public bool autoStart;
	public int MaxTimes { get { return m_maxTimes; } }
	int m_maxTimes = 1; //maxTimes <= 0  means infinity
	public int LeftTimes { get { return m_leftTimes; } set { m_leftTimes = value; } }
	int m_leftTimes;
	public float CurCd { get { return m_curCd; } }
	float m_cd;
	public int TotalBehaved { get { return m_behavedTimes; } }
	int m_behavedTimes;
	public Action<float> waitingAction;
	public Action<float, float> processAction;
	public Action behaveAction;
	public Action endAction;
	public Action refreshAction;
	UISliderHandle m_slider;
	float m_timerDuration;
	float m_passedTime;
	float m_curCd;
	bool m_started = false;

	public bool isStarted
	{
		get { return m_started; }
	}

	void Awake()
	{
		m_slider = GetComponentInChildren<UISliderHandle>();
	}
	void Start()
	{
		if (autoStart) m_started = true;
	}

	public void SetTimer(float during = 0, int maxTimes = 1, float cd = 0)
	{
		m_leftTimes = m_maxTimes = maxTimes;
		m_timerDuration = during;
		m_cd = cd;
	}
	static public Timer SetTimer(GameObject obj, float during = 0, int maxTimes = 1, float cd = 0)
	{
		Timer timer = obj.AddComponent<Timer>();
		timer.SetTimer(during, maxTimes, cd);
		return timer;
	}
	public void StartTimer(bool auto = false)
	{
		m_started = true;
		autoStart = auto;
	}
	void Update()
	{
		if (m_started)
		{
			m_passedTime += Time.deltaTime;
			m_slider?.SetValue(m_timerDuration, m_passedTime);
			if (m_passedTime <= m_timerDuration)
			{
				processAction?.Invoke(m_timerDuration, m_passedTime);
			}
			if (m_passedTime >= m_timerDuration)
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
				else if (m_cd > 0)
				{
					m_curCd = m_cd;
				}
				else if (m_cd == 0)
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
