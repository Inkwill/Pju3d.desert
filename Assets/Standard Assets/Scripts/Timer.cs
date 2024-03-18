using System;
using UnityEngine.Events;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public bool autoStart;
	public float timerDuration = 10.0f;
	public int loopTimes = 1;  //loopTimes <= 0  means infinity
	public float cd;
	public float CurCd { get { return m_curCd; } }
	public int TotalBehaved { get { return m_behavedTimes; } }
	public UnityEvent<float> waitingEvent;
	public UnityEvent<float, float> processEvent;
	public Action behaveEvent;
	public Action endEvent;
	public Action refreshEvent;
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
	}
	void Start()
	{
		if (autoStart) StartTimer();
	}

	public void StartTimer()
	{
		m_started = true;
	}
	void Update()
	{
		if (m_started)
		{
			m_passedTime += Time.deltaTime;
			m_slider?.SetValue(timerDuration, m_passedTime);
			if (m_passedTime <= timerDuration)
			{
				processEvent?.Invoke(timerDuration, m_passedTime);
			}
			if (m_passedTime >= timerDuration)
			{
				m_started = false;
				m_passedTime = 0f;
				loopTimes--;
				m_behavedTimes++;
				behaveEvent?.Invoke();
				if (loopTimes == 0)
				{
					endEvent?.Invoke();
				}
				else if (cd > 0)
				{
					m_curCd = cd;
				}
				else if (cd == 0)
				{
					refreshEvent?.Invoke();
					if (autoStart) StartTimer();
				}
			}
		}
		else
		{
			waitingEvent?.Invoke(m_curCd);
			if (m_curCd > 0)
			{
				m_curCd -= Time.deltaTime;
				if (m_curCd <= 0 && loopTimes > 0) refreshEvent?.Invoke();
			}
		}
	}
}
