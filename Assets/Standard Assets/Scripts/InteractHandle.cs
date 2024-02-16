using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InteractHandle : MonoBehaviour
{
	public UnityEvent<RoleControl, string> InteractEvent;
	[SerializeField]
	InteractOnTrigger Detector;
	[SerializeField]
	float handleTime = 1.0f;
	[SerializeField]
	Slider slider;
	public float During { get { return m_during; } set { m_during = value; if (slider) slider.value = value; } }
	float m_during;
	public RoleControl CurrentTarget { get { return m_target; } set { m_target = value; } }
	RoleControl m_target;

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
		if (m_target != null) return;
		RoleControl role = enter.GetComponent<RoleControl>();
		if (role)
		{
			InteractEvent?.Invoke(role, "Enter");
			During = 0;
			slider?.gameObject.SetActive(true);
		}
		//Helpers.Log(this, "OninterEnter", "enter= " + enter);
	}

	void OnInterExit(GameObject exiter)
	{
		if (m_target && m_target.gameObject == exiter) { m_target = null; }
		RoleControl role = exiter.GetComponent<RoleControl>();
		if (role) { InteractEvent?.Invoke(role, "Exit"); During = 0; slider?.gameObject.SetActive(false); }
		//Helpers.ShowUIElement(slider.gameObject, false);
		//Helpers.Log(this, "OninterExit", "exiter= " + exiter);
	}

	void OnInterStay(GameObject stayer, float duration)
	{
		if (m_target != null) return;
		RoleControl role = stayer.GetComponent<RoleControl>();
		if (role && role.isIdle)
		{
			m_during += Time.deltaTime;
			During = m_during;
			if (m_during >= handleTime)
			{
				During = 0;
				InteractEvent?.Invoke(role, "Completed");
				slider?.gameObject.SetActive(false);
			}
			InteractEvent?.Invoke(role, "Stay");
			//Helpers.Log(this, "OninterStay", "stayer= " + stayer);
		}
	}
}
