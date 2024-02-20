using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class RoleControl : MonoBehaviour
{
	public enum State
	{
		IDLE,
		MOVE,
		PURSUING,
		ATTACKING,
		SKILLING,
		DEAD
	}
	[Header("Base")]
	public Weapon DefaultWeapon;
	public Transform WeaponLocator;
	public CharacterData Data => m_CharacterData;
	protected CharacterData m_CharacterData;
	public State CurState { get { return m_State; } set { SetState(value); } }
	protected State m_State;
	public EventSender eventSender => m_eventSender;
	protected EventSender m_eventSender;
	protected Vector3 m_BirthPos;
	public Vector3 BirthPos => m_BirthPos;
	public bool isIdle { get { return m_State == State.IDLE; } }
	public bool isStandBy { get { return (m_State != State.DEAD && m_State != State.SKILLING); } }
	public float CurStateDuring => m_StateDuring;
	float m_StateDuring;

	//[Header("AI")]
	public AIBase BaseAI => m_Ai;
	protected AIBase m_Ai;
	protected CharacterData m_Enemy;
	public CharacterData CurrentEnemy
	{
		get { return m_Enemy; }
		set
		{
			if (value != null)
			{
				m_eventSender.Send(value.gameObject, "roleEvent_OnSetCurrentEnemy");
				m_Enemy = value;
			}
			else if (m_Enemy != null && value == null)
			{
				m_eventSender.Send(gameObject, "roleEvent_OnRemoveCurrentEnemy");
				m_Enemy = value;
			}
		}
	}
	public SkillUser SkillUser => m_SkillUser;
	SkillUser m_SkillUser;

	[Header("Audio")]
	public AudioClip[] HitClip;

	void Awake()
	{
		m_BirthPos = transform.position;
		AnimationDispatcher dispatcher = GetComponentInChildren<AnimationDispatcher>();
		if (dispatcher)
		{
			dispatcher.AttackStep.AddListener(AttackFrame);
			//m_Animator = dispatcher.GetComponent<Animator>();
		}
		//else m_Animator = GetComponentInChildren<Animator>();

		m_eventSender = GetComponent<EventSender>();

		m_CharacterData = GetComponent<CharacterData>();
		m_CharacterData.Init();
		m_CharacterData.Equipment.InitWeapon(DefaultWeapon);

		m_Ai = GetComponent<AIBase>();
		m_Ai.Init();


		m_SkillUser = GetComponent<SkillUser>();

		m_CharacterData.OnDamage += (damage) =>
		{
			m_eventSender?.Send(gameObject, "roleEvent_OnDamage");
			DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
		};

		m_CharacterData.Equipment.OnEquiped += item =>
		{
			if (item.Slot == EquipmentItem.EquipmentSlot.Weapon && WeaponLocator)
			{
				Weapon wp = item as Weapon;
				BaseAI.EnemyDetector.Radius = System.Math.Max(wp.Stats.MaxRange, BaseAI.EnemyDetector.Radius);
				wp.bulletTrans = WeaponLocator;
				if (!item.WorldObjectPrefab) return;
				var obj = Instantiate(item.WorldObjectPrefab, WeaponLocator, false);
				//Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
			}
		};

		m_CharacterData.Equipment.OnUnequip += item =>
		{
			if (item.Slot == EquipmentItem.EquipmentSlot.Weapon)
			{
				foreach (Transform t in WeaponLocator)
					Destroy(t.gameObject);
			}
		};
	}

	void Update()
	{
		m_StateDuring += Time.deltaTime;
		//Dead
		if (m_State == State.DEAD) return;
		if (m_CharacterData.Stats.CurrentHealth == 0 && m_State != State.DEAD)
		{
			m_CharacterData.Death();
			SetState(State.DEAD);
		}
		//Skill
		if (m_SkillUser && m_State != State.SKILLING && m_SkillUser.CurSkill != null) { SetState(State.SKILLING); }
		if (m_SkillUser && m_State == State.SKILLING && m_SkillUser.CurSkill == null) { SetState(State.IDLE); }
		//ATTACK
		if (m_State == State.ATTACKING)
		{
			if (m_Enemy && m_CharacterData.CanAttackReach(m_Enemy)) CheckAttack();
			else SetState(State.PURSUING);
		}
		//PURSUING
		if (m_State == State.PURSUING)
		{
			if (m_CharacterData.Equipment.Weapon == null)
			{
				if (DefaultWeapon) m_CharacterData.Equipment.EquipWeapon();
				else
				{
					Debug.LogError("Miss a Weapon! role = " + gameObject);
				}
			}
			if (m_Enemy && m_CharacterData.CanAttackReach(m_Enemy)) { SetState(State.ATTACKING); }
			else m_eventSender?.Send(gameObject, "roleEvent_OnPursuing");
		}
		//MOVE
		if (m_State == State.MOVE)
		{
			m_eventSender?.Send(gameObject, "roleEvent_OnMoving");
		}
		//IDLE
		if (m_State == State.IDLE)
		{
			m_eventSender?.Send(gameObject, "roleEvent_OnIdling");
		}
	}

	void SetState(State nextState)
	{
		if (m_State == nextState) return;
		m_State = nextState;
		m_StateDuring = 0;
		m_eventSender?.Send(gameObject, "roleEvent_OnState_" + System.Enum.GetName(typeof(State), m_State));
	}
	public void CheckAttack()
	{
		if (m_CharacterData.CanAttackTarget(m_Enemy))
		{
			BaseAI.Stop();
			BaseAI.LookAt(m_Enemy.transform);
			m_CharacterData.AttackTriggered();
			m_eventSender?.Send(gameObject, "roleEvent_OnAttack");
		}
	}

	void AttackFrame()
	{
		//if we can't reach the target anymore when it's time to damage, then that attack miss.
		if (m_Enemy && m_CharacterData.CanAttackReach(m_Enemy))
		{
			m_CharacterData.Attack(m_Enemy);
		}
		else if (m_Enemy) Helpers.Log(this, "AttackMiss: ", $"{Data.CharacterName}->{m_Enemy.CharacterName}");
		else Helpers.Log(this, "AttackMiss: ", $"{Data.CharacterName}->(Enemy Missed)");
	}

	// if (m_Enemy && m_State == State.PURSUING)
	// {
	// 	BaseAI.Agent.SetDestination(m_Enemy.gameObject.transform.position);
	// 	float distToEnemy = Vector3.SqrMagnitude(m_Enemy.gameObject.transform.position - transform.position);
	// 	if (m_CharacterData.CanAttackTarget(m_Enemy))
	// 	{
	// 		m_CharacterData.AttackTriggered();
	// 		m_Animator.SetTrigger(m_AttackParamID);
	// 		BaseAI.Agent.ResetPath();
	// 		BaseAI.Agent.velocity = Vector3.zero;
	// 		SetState(State.ATTACKING);
	// 		return;
	// 	}
	// }
	// if (m_Enemy && m_State == State.ATTACKING)
	// {
	// 	if (!m_CharacterData.CanAttackReach(m_Enemy))
	// 	{
	// 		SetState(State.PURSUING);
	// 		return;
	// 	}
	// 	else
	// 	{
	// 		if (m_CharacterData.CanAttackTarget(m_Enemy))
	// 		{
	// 			m_CharacterData.AttackTriggered();
	// 			m_Animator.SetTrigger(m_AttackParamID);
	// 			return;
	// 		}
	// 	}
	// }
}


