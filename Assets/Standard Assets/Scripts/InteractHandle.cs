using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractHandle : MonoBehaviour
{
	public UnityEvent<GameObject, string> InteractEvent;
	[SerializeField]
	InteractOnTrigger Detector;
	[SerializeField]
	float handleTime = 1.0f;
	[SerializeField]
	Slider slider;
	public float During { get { return m_during; } set { m_during = value; if (slider) slider.value = value; } }
	float m_during;
	public GameObject CurrentTarget { get { return m_target; } set { m_target = value; } }
	GameObject m_target;

	void Start()
	{
		if (slider) slider.maxValue = handleTime;
		slider?.gameObject.SetActive(false);
		Detector?.OnEnter.AddListener(OnInterEnter);
		Detector?.OnExit.AddListener(OnInterExit);
		Detector?.OnStay.AddListener(OnInterStay);
	}

	void OnInterEnter(GameObject enter)
	{
		if (m_target != null || m_target == enter) return;
		InteractEvent?.Invoke(enter, "Enter");
		During = 0;
		slider?.gameObject.SetActive(true);
		//Helpers.Log(this, "OninterEnter", "enter= " + enter);
	}

	void OnInterExit(GameObject exiter)
	{
		if (m_target != null && m_target == exiter) { m_target = null; }
		InteractEvent?.Invoke(exiter, "Exit");
		During = 0;
		slider?.gameObject.SetActive(false);
		//Helpers.ShowUIElement(slider.gameObject, false);
		//Helpers.Log(this, "OninterExit", "exiter= " + exiter);
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (m_target != null) return;
		RoleAI roleAi = stayer.GetComponent<RoleAI>();
		if (roleAi && roleAi.isIdle)
		{
			m_during += Time.deltaTime;
			During = m_during;
			if (m_during >= handleTime)
			{
				During = 0;
				InteractEvent?.Invoke(stayer, "Ready");
				slider?.gameObject.SetActive(false);
				m_target = stayer;
			}
			InteractEvent?.Invoke(stayer, "Stay");
			//Helpers.Log(this, "OninterStay", "stayer= " + stayer);
		}
	}
}
