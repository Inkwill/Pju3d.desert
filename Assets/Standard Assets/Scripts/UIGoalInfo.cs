using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIGoalInfo : UIEventSender
{
	public Text goalDes;
	public Text goalSummary;
	public Text expCount;
	public Image iconCompleted;
	public Animator animInfo;
	GameGoalSystem.GameGoal m_gameGoal;

	void Awake()
	{
		OnClickEvent.AddListener(OnClick);
	}
	public void SetGoal(GameGoalSystem.GameGoal goal)
	{
		m_gameGoal = goal;
		expCount.text = m_gameGoal != null ? m_gameGoal.data.PlayerExp.ToString() : "";
		goalDes.text = m_gameGoal != null ? m_gameGoal.data.describe : "";
		gameObject.SetActive(m_gameGoal != null);
		animInfo.gameObject.SetActive(m_gameGoal != null);
		if (m_gameGoal != null && !m_gameGoal.Completed) StartCoroutine(GameManager.Instance.WaitAction(2.0f, () => animInfo.SetBool("show", false)));
		UpdateGoal();
	}
	public void UpdateGoal()
	{
		goalSummary.text = m_gameGoal != null ? m_gameGoal.Summary : "";
		iconCompleted.gameObject.SetActive((m_gameGoal != null && m_gameGoal.Completed));
		animInfo.SetBool("show", m_gameGoal != null && m_gameGoal.Completed);
	}

	void OnClick()
	{
		if (m_gameGoal != null && m_gameGoal.Completed)
			m_gameGoal.Acheve();
		else animInfo.SetBool("show", !animInfo.GetBool("show"));
	}
}
