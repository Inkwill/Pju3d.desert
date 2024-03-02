using System;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;


[RequireComponent(typeof(CharacterData))]
public class AIBase : MonoBehaviour
{
	public enum State
	{
		IDLE,
		MOVE,
		PURSUING,
		ATTACKING,
		SKILLING,
		DEAD,
		INACTIVE
	}
	public State CurState { get { return m_State; } }
	protected State m_State = State.INACTIVE;
	public float CurStateDuring => m_StateDuring;
	float m_StateDuring;
	public Action<State> StateUpdateEvent;
	public bool isIdle { get { return m_State == State.IDLE; } }
	public bool isStandBy { get { return (m_State != State.DEAD && m_State != State.SKILLING); } }
	public bool isActive { get { return m_State != State.INACTIVE && m_State != State.DEAD; } }

	[Header("Detector")]
	public InteractOnTrigger SceneDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger SkillDetector;
	public string SceneBox { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, false); } }
	public string SceneBoxName { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, true); } }
	public virtual float SpeedScale { get { return 1; } }
	public CharacterData CurrentEnemy => m_character.CurrentEnemy;
	public CharacterData Character => m_character;
	protected CharacterData m_character;
	public SkillUser SkillUser => m_SkillUser;
	SkillUser m_SkillUser;

	void Start()
	{
		m_character = GetComponent<CharacterData>();
		m_character.OnDamage.AddListener(OnDamageAI);
		m_character.OnDeath.AddListener((character) => { SetState(State.DEAD); OnDeathAI(); });
		m_SkillUser = GetComponent<SkillUser>();

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
		switch (m_character.camp)
		{
			case CharacterData.Camp.PLAYER:
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Interactable", "Player", "Neutral");
				break;
			case CharacterData.Camp.ENEMY:
				EnemyDetector.layers = LayerMask.GetMask("Player", "Building");
				InteractDetector.layers = LayerMask.GetMask("Interactable");
				break;
			case CharacterData.Camp.ALLY:
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			case CharacterData.Camp.NEUTRAL:
				EnemyDetector.layers = LayerMask.GetMask("Noting");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			case CharacterData.Camp.BUILDING:
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Noting");
				break;
			default:
				break;
		}
		m_State = State.IDLE;
		OnStart();
	}

	void Update()
	{
		m_StateDuring += Time.deltaTime;
		//Dead or Inactive
		if (m_State == State.DEAD || m_State == State.INACTIVE) return;
		//Skill
		if (SkillUser && m_State != State.SKILLING && SkillUser.CurSkill != null) { SetState(State.SKILLING); }
		if (SkillUser && m_State == State.SKILLING && SkillUser.CurSkill == null) { SetState(State.IDLE); }
		//ATTACK
		if (m_State == State.ATTACKING)
		{
			if (m_character.CanAttackReach()) m_character.CheckAttack();
			else SetState(State.PURSUING);
		}
		//PURSUING
		if (m_State == State.PURSUING)
		{
			if (m_character.Equipment.Weapon == null)
			{
				if (m_character.DefaultWeapon) m_character.Equipment.EquipWeapon(m_character.DefaultWeapon);
				else
				{
					Debug.LogError("Miss a Weapon! role = " + gameObject);
				}
			}
			if (m_character.CanAttackReach()) SetState(State.ATTACKING);
		}
		StateUpdateEvent?.Invoke(m_State);
		OnStateUpdate(m_State);
	}
	public void SetState(State nextState)
	{
		if (m_State == nextState) return;
		m_State = nextState;
		m_StateDuring = 0;
		if (m_State == State.INACTIVE) { m_character.BaseAI.Stop(); m_character.SetEnemy(null); }
		GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnState_" + System.Enum.GetName(typeof(State), m_State));
	}

	public virtual void LookAt(Transform trans)
	{
		LookAt(trans.position - m_character.transform.position);
	}

	public virtual void LookAt(Vector3 forward)
	{
		forward.y = 0;
		forward.Normalize();
		m_character.transform.forward = forward;
	}

	protected virtual void OnStart() { }
	public virtual void Stop() { }
	protected virtual void OnStateUpdate(State state) { }
	protected virtual void OnDeathAI() { }
	protected virtual void OnDamageAI(Damage damage) { }

	protected virtual void OnEnemyEnter(GameObject enter)
	{
		if (m_character.CurrentEnemy == null) m_character.SetEnemy(enter.GetComponent<CharacterData>());
	}
	protected virtual void OnEnemyExit(GameObject exiter)
	{
		if (m_character.CurrentEnemy && m_character.CurrentEnemy.gameObject == exiter)
		{
			m_character.SetEnemy(EnemyDetector.GetNearest()?.GetComponent<CharacterData>());
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
		if (SkillUser && SkillUser.CurSkill != null) SkillUser.AddTarget(enter);
		//Debug.Log("OnSkillTargetEnter: enter= " + enter);
	}

	void OnSkillTargetExit(GameObject exiter)
	{
		if (SkillUser && SkillUser.CurSkill != null) SkillUser.RemoveTarget(exiter);
		//Debug.Log("OnSkillTargetExit: exiter= " + exiter);
	}
	void OnSkillTargetEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
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
