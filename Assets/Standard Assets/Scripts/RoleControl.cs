using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;

public class RoleControl : MonoBehaviour,
	RoleAnimationDispatcher.IAttackFrameReceiver,
	RoleAnimationDispatcher.IFootstepFrameReceiver,
	RoleAnimationDispatcher.ISkillstepFrameReceiver
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
	public float Speed = 6.0f;
	public Weapon DefaultWeapon;
	[SerializeField]
	Transform WeaponLocator;
	public CharacterData Data => m_CharacterData;
	protected CharacterData m_CharacterData;
	public EventSender eventSender => m_eventSender;
	protected EventSender m_eventSender;
	public bool isIdle { get { return (m_State == State.IDLE) && !m_Enemy; } protected set { } }

	//[Header("AI")]
	public State CurState { get { return m_State; } set { SetState(value); } }
	protected State m_State;
	public RoleAI BaseAI => m_Ai;
	protected RoleAI m_Ai;
	protected CharacterData m_Enemy;
	public CharacterData CurrentEnemy
	{
		get { return m_Enemy; }
		set
		{
			m_Enemy = value;
			GameObject target = value ? m_Enemy.gameObject : null;
			m_eventSender.Send(target, "roleEvent_OnSetCurrentEnemy");
		}
	}
	protected Vector3 m_Destination;
	public Skill CurSkill => m_skill;
	protected Skill m_skill;
	public float CurStateDuring => m_StateDuring;
	float m_StateDuring;
	float m_AiBeat = 0.1f;
	float m_AiDuring;

	[Header("Animator")]
	protected Animator m_Animator;
	protected NavMeshAgent m_Agent;
	int m_DeathParamID;
	int m_SpeedParamID;
	int m_AttackParamID;
	int m_HitParamID;
	int m_RespawnParamID;

	[Header("Audio")]
	public AudioClip[] SpurSoundClips;
	public AudioClip[] SpottedAudioClip;
	public CharacterAudio AudioPlayer => m_CharacterAudio;
	protected CharacterAudio m_CharacterAudio;

	void Awake()
	{
		m_Agent = GetComponent<NavMeshAgent>();
		RoleAnimationDispatcher dispatcher = GetComponentInChildren<RoleAnimationDispatcher>();
		if (dispatcher)
		{
			dispatcher.Init(this);
			m_Animator = dispatcher.GetComponent<Animator>();
		}
		else m_Animator = GetComponentInChildren<Animator>();

		m_DeathParamID = Animator.StringToHash("Death");
		m_SpeedParamID = Animator.StringToHash("Speed");
		m_AttackParamID = Animator.StringToHash("Attack");
		m_HitParamID = Animator.StringToHash("Hit");
		//m_FaintParamID = Animator.StringToHash("Faint");
		m_RespawnParamID = Animator.StringToHash("Respawn");

		m_eventSender = GetComponent<EventSender>();

		m_Agent.speed = Speed;
		m_Agent.angularSpeed = 360.0f;

		m_CharacterData = GetComponent<CharacterData>();
		m_CharacterData.Init();
		m_CharacterData.Equipment.InitWeapon(DefaultWeapon);

		m_Ai = GetComponent<RoleAI>();
		m_Ai.Init(this);

		m_CharacterAudio = GetComponentInChildren<CharacterAudio>();

		m_CharacterData.OnDamage += () =>
		{
			m_Animator.SetTrigger(m_HitParamID);
			m_CharacterAudio.Hit(transform.position);
			m_eventSender?.Send(gameObject, "roleEvent_OnDamage");
		};

		m_CharacterData.Equipment.OnEquiped += item =>
		{
			if (item.Slot == (EquipmentItem.EquipmentSlot)666 && WeaponLocator)
			{
				if (!item.WorldObjectPrefab) return;
				var obj = Instantiate(item.WorldObjectPrefab, WeaponLocator, false);
				Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
			}
		};

		m_CharacterData.Equipment.OnUnequip += item =>
		{
			if (item.Slot == (EquipmentItem.EquipmentSlot)666)
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
		if (m_CharacterData.Stats.CurrentHealth == 0 && m_State != State.DEAD)
		{
			SetState(State.DEAD);
			return;
		}
		if (m_State == State.DEAD) return;
		//Skill
		if (m_State == State.SKILLING)
		{
			if (m_StateDuring < m_skill.Duration)
			{
				m_Animator.SetTrigger(m_skill.SkillAnim);
				m_skill.Operating(this);
				m_eventSender?.Send(gameObject, "roleEvent_OnOperatSkill");
			}
			else
			{
				m_skill.Implement(this);
				m_eventSender?.Send(gameObject, "roleEvent_OnImplementSkill");
				SetState(State.IDLE);
				return;
			}
		}
		//ATTACK
		if (m_State == State.ATTACKING)
		{
			if (m_Enemy) CheckAttack();
			else { SetState(State.IDLE); return; }
		}
		//PURSUING
		if (m_State == State.PURSUING)
		{
			if (SpottedAudioClip.Length != 0)
			{
				SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
				{
					Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
					Position = transform.position
				});
			}
			if (m_Enemy)
			{
				LookAt(m_Enemy.transform);
				m_Agent.SetDestination(m_Enemy.gameObject.transform.position);
				m_eventSender?.Send(gameObject, "roleEvent_OnPursuing");
				CheckAttack();
			}
			else { SetState(State.IDLE); return; }
		}
		//MOVE
		if (m_State == State.MOVE)
		{
			if (Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
			{
				m_eventSender?.Send(gameObject, "roleEvent_OnMoveEnd");
				SetState(State.IDLE);
				return;
			}
			else { m_eventSender?.Send(gameObject, "roleEvent_OnMoving"); }
		}
		//IDLE
		if (m_State == State.IDLE)
		{
			if (m_Enemy && m_Ai.Offensive) { SetState(State.PURSUING); return; }
			m_eventSender?.Send(gameObject, "roleEvent_OnIdling");
		}
		//PlayMove
		m_Animator.SetFloat(m_SpeedParamID, m_Agent.velocity.magnitude / Speed);
	}
	void CheckAttack()
	{
		if (m_CharacterData.Equipment.Weapon == null)
		{
			if (DefaultWeapon) m_CharacterData.Equipment.EquipWeapon();
			else
			{
				Debug.LogError("Miss a Weapon! role = " + gameObject);
				return;
			}
		}

		if (m_CharacterData.CanAttackReach(m_Enemy))
		{
			m_Agent.ResetPath();
			m_Agent.velocity = Vector3.zero;
			LookAt(m_Enemy.transform);
			if (m_CharacterData.CanAttackTarget(m_Enemy))
			{
				SetState(State.ATTACKING);
			}
		}
		else SetState(State.PURSUING);
	}
	public void AttackFrame()
	{
		//if we can't reach the target anymore when it's time to damage, then that attack miss.
		if (m_Enemy && m_CharacterData.CanAttackReach(m_Enemy))
		{
			m_CharacterData.Attack(m_Enemy);

			var attackPos = m_Enemy.transform.position + transform.up * 0.5f;
			VFXManager.PlayVFX(VFXType.Hit, attackPos);
			SFXManager.PlaySound(m_CharacterAudio.UseType, new SFXManager.PlayData() { Clip = m_CharacterData.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = attackPos });
		}
		else Debug.Log("Miss Attack! " + Data.CharacterName);
		//SetState(State.PURSUING);
	}
	// if (m_Enemy && m_State == State.PURSUING)
	// {
	// 	m_Agent.SetDestination(m_Enemy.gameObject.transform.position);
	// 	float distToEnemy = Vector3.SqrMagnitude(m_Enemy.gameObject.transform.position - transform.position);
	// 	if (m_CharacterData.CanAttackTarget(m_Enemy))
	// 	{
	// 		m_CharacterData.AttackTriggered();
	// 		m_Animator.SetTrigger(m_AttackParamID);
	// 		m_Agent.ResetPath();
	// 		m_Agent.velocity = Vector3.zero;
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


	protected void SetState(State nextState)
	{
		m_State = nextState;
		if (m_State != nextState) m_StateDuring = 0;
		m_eventSender?.Send(gameObject, "roleEvent_OnState_" + System.Enum.GetName(typeof(State), m_State));
		switch (m_State)
		{
			case State.IDLE:
				m_Agent.isStopped = true;
				break;
			case State.MOVE:
				m_Agent.isStopped = false;
				m_Agent.SetDestination(m_Destination);
				break;
			case State.PURSUING:
				m_Agent.isStopped = false;
				break;
			case State.ATTACKING:
				m_Agent.isStopped = true;
				m_CharacterData.AttackTriggered();
				m_Animator.SetTrigger(m_AttackParamID);
				break;
			case State.SKILLING:
				m_Agent.isStopped = true;
				break;
			case State.DEAD:
				m_Agent.isStopped = true;
				m_Agent.enabled = false;
				m_Animator.SetTrigger(m_DeathParamID);
				m_CharacterAudio.Death(transform.position);
				m_CharacterData.Death();
				Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
				// if (m_LootSpawner != null)
				// 	m_LootSpawner.SpawnLoot();
				//Destroy(m_Agent);
				//Destroy(GetComponent<Collider>());
				//Destroy(this);
				//m_Animator.SetTrigger(m_FaintParamID);
				//m_Agent.ResetPath();
				//m_KOTimer = 0.0f;
				break;
		}
	}
	public void LookAt(Transform trans)
	{
		Vector3 forward = (trans.position - transform.position);
		forward.y = 0;
		forward.Normalize();
		transform.forward = forward;
	}

	public void FootstepFrame()
	{
		Vector3 pos = transform.position;
		m_CharacterAudio.Step(pos);
		VFXManager.PlayVFX(VFXType.StepPuff, pos);

		if (SpurSoundClips.Length > 0)
			SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
			{
				Clip = SpurSoundClips[Random.Range(0, SpurSoundClips.Length)],
				Position = pos,
				PitchMin = 0.8f,
				PitchMax = 1.2f,
				Volume = 0.3f
			});
	}

	public void SkillstepFrame()
	{
		m_skill.StepEffect(this);
	}

	public void UseSkill(Skill skill)
	{
		if (isIdle && skill.CanUsedBy(this))
		{
			m_skill = skill;
			SetState(State.SKILLING);
		}
	}

	public void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		SetState(State.MOVE);
	}
}


