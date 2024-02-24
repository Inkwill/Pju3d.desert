using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;


[RequireComponent(typeof(CharacterData))]
public class AIBase : MonoBehaviour
{
	public enum Camp
	{
		PLAYER,
		ALLY,
		ENEMY,
		NEUTRAL
	}
	public Camp camp;
	public virtual float SpeedScale { get { return 1; } }
	[Header("Detector")]
	public InteractOnTrigger SceneDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger SkillDetector;
	public string SceneBox { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, false); } }
	public string SceneBoxName { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, true); } }
	protected CharacterData m_character;


	public virtual void Init()
	{
		m_character = GetComponent<CharacterData>();
		GetComponent<EventSender>()?.events.AddListener(OnCharacterEvent);

		if (EnemyDetector)
		{
			EnemyDetector.OnEnter.AddListener(OnEnemyEnter);
			EnemyDetector.OnExit.AddListener(OnEnemyExit);
			EnemyDetector.OnEvent.AddListener(OnEnemyEvent);
		}
		if (SkillDetector)
		{
			SkillDetector.OnEnter.AddListener(OnSkillTargetEnter);
			SkillDetector.OnExit.AddListener(OnSkillTargetExit);
			SkillDetector.OnEvent.AddListener(OnSkillTargetEvent);
		}
		if (InteractDetector)
		{
			InteractDetector.OnStay.AddListener(OnInteractStay);
			// m_role.InteractDetector.OnExit.AddListener(OnInteracterExit);
			// m_role.InteractDetector.OnEvent.AddListener(OnInteractorEvent);
		}
		switch (camp)
		{
			case Camp.PLAYER:
				m_character.gameObject.layer = LayerMask.NameToLayer("Player");
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Interactable", "Player", "Neutral");
				break;
			case Camp.ENEMY:
				m_character.gameObject.layer = LayerMask.NameToLayer("Enemy");
				EnemyDetector.layers = LayerMask.GetMask("Player");
				InteractDetector.layers = LayerMask.GetMask("Interactable");
				break;
			case Camp.ALLY:
				m_character.gameObject.layer = LayerMask.NameToLayer("Player");
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			case Camp.NEUTRAL:
				m_character.gameObject.layer = LayerMask.NameToLayer("Neutral");
				EnemyDetector.layers = LayerMask.GetMask("Noting");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			default:
				break;
		}
	}

	public virtual void LookAt(Transform trans)
	{
		Vector3 forward = (trans.position - m_character.transform.position);
		forward.y = 0;
		forward.Normalize();
		m_character.transform.forward = forward;
	}

	public virtual void Stop() { }

	void OnCharacterEvent(GameObject obj, string eventName)
	{
		if (eventName == "roleEvent_OnIdling") OnIdlingAI();
		if (eventName == "roleEvent_OnMoving") OnMovingAI();
		if (eventName == "roleEvent_OnPursuing") OnPursuingAI();
		if (eventName == "characterEvent_OnDamage") OnDamageAI();
		if (eventName == "characterEvent_OnDeath") OnDeadAI();
	}

	protected virtual void OnIdlingAI() { }
	protected virtual void OnMovingAI() { }
	protected virtual void OnPursuingAI() { }
	protected virtual void OnDeadAI() { }
	protected virtual void OnDamageAI() { }

	protected virtual void OnEnemyEnter(GameObject enter)
	{
		if (m_character.CurrentEnemy == null) m_character.CurrentEnemy = enter.GetComponent<CharacterData>();
	}
	protected virtual void OnEnemyExit(GameObject exiter)
	{
		if (m_character.CurrentEnemy && m_character.CurrentEnemy.gameObject == exiter)
		{
			m_character.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
			//m_eventSender.Send(exiter, "roleEvent_OnEnemyExit");
		}
	}
	protected virtual void OnEnemyEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			//m_role.CurState = RoleControl.State.IDLE;
			//Debug.Log("OnEnemyEvent: target= " + sender + "event= " + eventMessage);
		}
	}

	void OnSkillTargetEnter(GameObject enter)
	{
		if (m_character.SkillUser && m_character.SkillUser.CurSkill != null) m_character.SkillUser.AddTarget(enter);
		//Debug.Log("OnSkillTargetEnter: enter= " + enter);
	}

	void OnSkillTargetExit(GameObject exiter)
	{
		if (m_character.SkillUser && m_character.SkillUser.CurSkill != null) m_character.SkillUser.RemoveTarget(exiter);
		//Debug.Log("OnSkillTargetExit: exiter= " + exiter);
	}
	void OnSkillTargetEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "characterEvent_OnDeath")
		{
			Debug.Log("OnSkillTargetEvent: target= " + sender + "event= " + eventMessage);
		}
	}
	protected virtual void OnInteractStay(GameObject interactor, float during)
	{
		if (interactor.tag != "item" && m_character.CurrentEnemy == null) LookAt(interactor.transform);
		//HighlightTarget(interactor.gameObject, true);
		//Debug.Log("[RoleAI-" + m_role + "] OnInteracting with : " + interactor.gameObject);
	}
	// void HighlightTarget(GameObject obj, bool active)
	// {
	// 	HighlightableObject target = obj.GetComponent<HighlightableObject>();
	// 	if (target)
	// 	{
	// 		if (active) target.Highlight();
	// 		else target.Dehighlight();
	// 	}
	// }
}
