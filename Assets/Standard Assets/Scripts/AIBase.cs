using System.Collections;
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
	protected CharacterData m_Enemy;
	public CharacterData CurrentEnemy { get { return m_Enemy; } }
	public virtual float SpeedScale { get { return 1; } }
	protected CharacterData m_character;

	public virtual void Init(CharacterData data)
	{
		m_character = data;
		m_character.OnDamage += OnDamageAI;
		m_character.OnDeath.AddListener((character) => { SetState(State.DEAD); OnDeathAI(); });

		AnimationDispatcher dispatcher = GetComponentInChildren<AnimationDispatcher>();
		if (dispatcher) dispatcher.AttackStep.AddListener(AttackFrame);

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
				EnemyDetector.layers = LayerMask.GetMask("Player");
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
			default:
				break;
		}
		m_State = State.IDLE;
	}

	void Update()
	{
		if (m_State == State.INACTIVE) return;

		m_StateDuring += Time.deltaTime;
		//Dead
		if (m_State == State.DEAD) return;
		//Skill
		if (m_character.SkillUser && m_State != State.SKILLING && m_character.SkillUser.CurSkill != null) { SetState(State.SKILLING); }
		if (m_character.SkillUser && m_State == State.SKILLING && m_character.SkillUser.CurSkill == null) { SetState(State.IDLE); }
		//ATTACK
		if (m_State == State.ATTACKING)
		{
			if (CurrentEnemy && m_character.CanAttackReach(CurrentEnemy)) CheckAttack();
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
			if (CurrentEnemy && m_character.CanAttackReach(CurrentEnemy)) { SetState(State.ATTACKING); }
			else { OnPursuingAI(); GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnPursuing"); }
		}
		//MOVE
		if (m_State == State.MOVE)
		{
			OnMovingAI();
			GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnMoving");
		}
		//IDLE
		if (m_State == State.IDLE)
		{
			OnIdlingAI();
			GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnIdling");
		}
	}
	public void SetState(State nextState)
	{
		if (m_State == nextState) return;
		m_State = nextState;
		m_StateDuring = 0;
		GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnState_" + System.Enum.GetName(typeof(State), m_State));
	}

	public void CheckAttack()
	{
		if (m_character.CanAttackTarget(CurrentEnemy))
		{
			m_character.BaseAI.Stop();
			m_character.BaseAI.LookAt(CurrentEnemy.transform);
			m_character.AttackTriggered();
			GetComponent<EventSender>()?.Send(gameObject, "roleEvent_OnAttack");
		}
	}

	void AttackFrame()
	{
		//if we can't reach the target anymore when it's time to damage, then that attack miss.
		if (CurrentEnemy && m_character.CanAttackReach(CurrentEnemy))
		{
			m_character.Attack(CurrentEnemy);
		}
		else if (CurrentEnemy) Helpers.Log(this, "AttackMiss: ", $"{m_character.CharacterName}->{CurrentEnemy.CharacterName}");
		else Helpers.Log(this, "AttackMiss: ", $"{m_character.CharacterName}->(Enemy Missed)");
	}
	public virtual void LookAt(Transform trans)
	{
		Vector3 forward = (trans.position - m_character.transform.position);
		forward.y = 0;
		forward.Normalize();
		m_character.transform.forward = forward;
	}

	public virtual void Stop() { }
	protected virtual void OnIdlingAI() { }
	protected virtual void OnMovingAI() { }
	protected virtual void OnPursuingAI() { }
	protected virtual void OnDeathAI() { }
	protected virtual void OnDamageAI(Damage damage) { }

	protected virtual void OnEnemyEnter(GameObject enter)
	{
		if (m_Enemy == null) m_Enemy = enter.GetComponent<CharacterData>();
	}
	protected virtual void OnEnemyExit(GameObject exiter)
	{
		if (m_Enemy && m_Enemy.gameObject == exiter)
		{
			m_Enemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
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
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			Debug.Log("OnSkillTargetEvent: target= " + sender + "event= " + eventMessage);
		}
	}
	protected virtual void OnInteractStay(GameObject interactor, float during)
	{
		if (interactor.tag != "item" && CurrentEnemy == null) LookAt(interactor.transform);
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
