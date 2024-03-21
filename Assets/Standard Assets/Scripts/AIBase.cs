using System;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;


[RequireComponent(typeof(Character))]
public class AIBase : MonoBehaviour
{
	public enum State
	{
		IDLE,
		MOVE,
		PURSUING,
		ATTACKING,
		SKILLING,
		INTERACTING,
		DEAD,
		INACTIVE
	}
	public State CurState { get { return m_State; } }
	protected State m_State = State.INACTIVE;
	public float CurStateDuring => m_StateDuring;
	float m_StateDuring;
	public bool isIdle { get { return m_State == State.IDLE || m_State == State.INTERACTING; } }
	public bool isStandBy { get { return (m_State != State.DEAD && m_State != State.SKILLING); } }
	public bool isActive { get { return m_State != State.INACTIVE && m_State != State.DEAD; } }
	public AiData Data { get { return m_data; } set { m_data = value; } }
	protected AiData m_data;


	[Header("Detector")]
	public InteractOnTrigger SceneDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger SkillDetector;
	public string SceneBox { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, false); } }
	public string SceneBoxName { get { return GameManager.SceneBoxInfo(SceneDetector.lastInner, true); } }
	public virtual float SpeedScale { get { return 1; } }
	public Character CurrentEnemy => m_character.CurrentEnemy;
	public Character Character => m_character;
	protected Character m_character;


	void Start()
	{
		if (m_data == null) m_data = AiData.GetAiDataByKey("default");
		m_character = GetComponent<Character>();
		m_character.DamageEvent.AddListener(OnDamageAI);
		m_character.InteractAction += (interactor) => { SetState(State.INTERACTING); };
		m_character.DeathEvent.AddListener((character) => { SetState(State.DEAD); OnDeathAI(); });

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
			case Character.Camp.PLAYER:
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Interactable", "Player", "Neutral");
				break;
			case Character.Camp.ENEMY:
				EnemyDetector.layers = LayerMask.GetMask("Player", "Building");
				InteractDetector.layers = LayerMask.GetMask("Interactable");
				break;
			case Character.Camp.ALLY:
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			case Character.Camp.NEUTRAL:
				EnemyDetector.layers = LayerMask.GetMask("Noting");
				InteractDetector.layers = LayerMask.GetMask("Player");
				break;
			case Character.Camp.BUILDING:
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
		if (m_State == State.IDLE || m_State == State.INTERACTING)
		{
			if (CurrentEnemy != null) { SetState(State.PURSUING); return; }
			else if (EnemyDetector.Inners.Count > 0)
			{
				m_character.SetEnemy(EnemyDetector.GetNearest().GetComponent<Character>());
				SetState(State.PURSUING);
				return;
			}
		}
		//Skill
		if (m_character.SkillUser && m_State != State.SKILLING && m_character.SkillUser.CurSkill != null) { SetState(State.SKILLING); }
		if (m_character.SkillUser && m_State == State.SKILLING && m_character.SkillUser.CurSkill == null) { SetState(State.IDLE); }
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
			if (m_character.CurrentEnemy && m_character.CanAttackReach()) SetState(State.ATTACKING);
			else if (m_character.CurrentEnemy == null && EnemyDetector.Inners.Count > 0) m_character.SetEnemy(EnemyDetector.GetNearest()?.GetComponent<Character>());
			if (CurrentEnemy) LookAt(CurrentEnemy.transform);
			else { SetState(State.IDLE); return; }
		}
		//INTERACTING
		if (m_State == State.INTERACTING && m_character.CurrentInteractor == null) SetState(State.IDLE);
		if (m_State != State.INTERACTING && m_character.CurrentInteractor != null) m_character.CurrentInteractor = null;
		m_character.StateUpdateAction?.Invoke(m_State);
		OnStateUpdate(m_State);
	}
	public void SetState(State nextState)
	{
		if (m_State == nextState) return;
		m_State = nextState;
		m_StateDuring = 0;
		if (m_State == State.INACTIVE) { m_character.BaseAI.Stop(); m_character.SetEnemy(null); }
		m_character.StateStartAction?.Invoke(m_State);
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
		if (m_character.CurrentEnemy == null) m_character.SetEnemy(enter.GetComponent<Character>());
	}
	protected virtual void OnEnemyExit(GameObject exiter)
	{
		if (m_character.CurrentEnemy && m_character.CurrentEnemy.gameObject == exiter)
		{
			m_character.SetEnemy(null);
		}
	}
	protected virtual void OnEnemyEvent(GameObject sender, string eventMessage)
	{

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
