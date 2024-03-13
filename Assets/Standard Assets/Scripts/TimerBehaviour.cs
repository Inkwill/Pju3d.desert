using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class TimerBehaviour : MonoBehaviour
{
	public bool autoStart;
	public float timerDuration = 10.0f;
	public int loopTimes = 1;  //loopTimes <= 0  means infinity
	public float cd;
	public UnityEvent<GameObject, GameObject> behaveEvent;
	public UnityEvent<float, float> processEvent;
	protected GameObject m_target;
	float m_passedTime;
	float m_curCd;
	bool m_started = false;
	public bool isStarted
	{
		get { return m_started; }
	}

	void Start()
	{
		if (autoStart) StartTimer();
	}

	protected void StartTimer()
	{
		m_started = true;
		OnStart();
	}

	void Update()
	{
		if (m_started)
		{
			m_passedTime += Time.deltaTime;
			if (m_passedTime <= timerDuration)
			{
				processEvent?.Invoke(timerDuration, m_passedTime);
				OnProcessing(m_passedTime / timerDuration);
			}
			if (m_passedTime >= timerDuration)
			{
				m_started = false;
				m_passedTime = 0f;
				loopTimes--;
				if (m_target != null) behaveEvent?.Invoke(gameObject, m_target);
				else behaveEvent?.Invoke(gameObject, gameObject);
				OnBehaved();
				if (loopTimes == 0)
				{
					OnEnd();
				}
				else if (cd > 0)
				{
					m_curCd = cd;
				}
				else if (cd == 0)
				{
					OnRefresh();
					if (autoStart) StartTimer();
				}
			}
		}
		else
		{
			OnInterval(m_curCd);
			if (m_curCd > 0)
			{
				m_curCd -= Time.deltaTime;
				if (m_curCd <= 0 && loopTimes > 0) OnRefresh();
			}
		}
	}

	protected virtual void OnBehaved() { }
	protected virtual void OnProcessing(float completed) { }
	protected virtual void OnStart() { }
	protected virtual void OnEnd() { }
	protected virtual void OnInterval(float curCd) { }
	protected virtual void OnRefresh() { }
}
