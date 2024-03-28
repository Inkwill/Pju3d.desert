using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIGoalInfo : UIEventSender
{
	public Text goalDes;
	public Text goalSummary;
	public Text expCount;
	public Image iconCompleted;
	public Animator animInfo;
	public Image iconExp;
	GameGoalSystem.GameGoal m_gameGoal;

	void Awake()
	{
		OnClickEvent.AddListener(OnClick);
	}
	public void SetGoal(GameGoalSystem.GameGoal goal)
	{
		if (goal == null) return;
		m_gameGoal = goal;
		expCount.text = m_gameGoal.data.LordExp.ToString();
		goalDes.text = m_gameGoal.data.describe;
		if (!gameObject.activeSelf) gameObject.SetActive(true);
		gameObject.GetComponent<Animator>()?.SetBool("show", true);
		UpdateGoal();
		ShowInfo(true);
	}
	public void UpdateGoal()
	{
		goalSummary.text = m_gameGoal != null ? m_gameGoal.Summary : "";
		iconCompleted.gameObject.SetActive((m_gameGoal.Completed));
		if (m_gameGoal.Completed) ShowInfo(true);
	}

	void OnClick()
	{
		if (m_gameGoal != null && m_gameGoal.Completed)
		{
			gameObject.GetComponent<Animator>()?.SetBool("show", false);
			var expIcon = Instantiate(iconExp.gameObject, iconExp.transform);
			expIcon.transform.DOMove(GameManager.GameUI.winRpg.iconExp.transform.position, 1.0f);
			Destroy(expIcon, 1.0f);
			GameManager.StartWaitAction(1.0f, () => m_gameGoal.Acheve());
		}
		else ShowInfo(!animInfo.GetBool("show"));
	}

	void ShowInfo(bool show)
	{
		if (!animInfo.gameObject.activeSelf) animInfo.gameObject.SetActive(true);
		animInfo.SetBool("show", show);
		if (show && !m_gameGoal.Completed) GameManager.StartWaitAction(3.0f, () => animInfo.SetBool("show", false));
	}
}
