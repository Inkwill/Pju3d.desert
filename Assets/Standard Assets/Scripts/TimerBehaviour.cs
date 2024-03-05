using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimerBehaviour : MonoBehaviour
{
	public float timerDuration = 10.0f;
	public int loopTimes = 1;  //loopTimes <= 0  means infinity
	public float cd;
	public UnityEvent<GameObject, GameObject> behaveEvents;
	public bool autoStart;
	public UISliderHandle progressSlider;
	public string behavePrompt;
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
		if (autoStart) StartBehaviour();
	}

	public void StartBehaviour()
	{
		m_started = true;
		progressSlider?.Init(timerDuration, m_passedTime, behavePrompt);
		OnStart();
	}

	void Update()
	{
		if (m_started)
		{
			m_passedTime += Time.deltaTime;
			progressSlider?.SetValue(timerDuration, m_passedTime);
			if (m_passedTime >= timerDuration)
			{
				if (m_target != null) behaveEvents?.Invoke(gameObject, m_target);
				else behaveEvents?.Invoke(gameObject, gameObject);
				OnBehaved();
				m_passedTime = 0f;
				loopTimes--;
				if (loopTimes == 0)
				{
					m_started = false;
					OnEnd();
				}
				else if (cd > 0)
				{
					m_curCd = cd;
					m_started = false;
				}
				else if (cd == 0)
				{
					m_started = false;
					OnRefresh();
				}

			}
			OnProcessing(m_passedTime / timerDuration);
		}
		else
		{
			OnInterval();

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
	protected virtual void OnInterval() { }
	protected virtual void OnRefresh() { }
}
