using UnityEngine;
using UnityEngine.AI;
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
	public float Speed = 6.0f;
	public Weapon DefaultWeapon;
	[SerializeField]
	Transform WeaponLocator;
	public CharacterData Data => m_CharacterData;
	protected CharacterData m_CharacterData;
	public EventSender eventSender => m_eventSender;
	protected EventSender m_eventSender;
	protected Vector3 m_BirthPos;
	public Vector3 BirthPos => m_BirthPos;

	public bool isIdle { get { return (m_State == State.IDLE) && !m_Enemy; } }
	public bool isStandBy { get { return (m_State != State.DEAD && m_State != State.SKILLING); } }

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
	protected Vector3 m_Destination = Vector3.zero;
	public SkillUser SkillUser => m_SkillUser;
	SkillUser m_SkillUser;
	public float CurStateDuring => m_StateDuring;
	float m_StateDuring;

	//[Header("Animator")]
	protected Animator m_Animator;
	protected NavMeshAgent m_Agent;
	NavMeshPath m_CalculatedPath;
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
		m_BirthPos = transform.position;
		m_Agent = GetComponent<NavMeshAgent>();
		m_Agent.speed = Speed;
		m_Agent.angularSpeed = 360.0f;
		m_CalculatedPath = new NavMeshPath();

		AnimationDispatcher dispatcher = GetComponentInChildren<AnimationDispatcher>();
		if (dispatcher)
		{
			dispatcher.AttackStep.AddListener(AttackFrame);
			dispatcher.FootStep.AddListener(FootstepFrame);
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

		m_CharacterData = GetComponent<CharacterData>();
		m_CharacterData.Init();
		m_CharacterData.Equipment.InitWeapon(DefaultWeapon);

		m_Ai = GetComponent<RoleAI>();
		m_Ai.Init(this);

		m_SkillUser = GetComponent<SkillUser>();

		m_CharacterAudio = GetComponentInChildren<CharacterAudio>();

		m_CharacterData.OnDamage += (damage) =>
		{
			m_Animator.SetTrigger(m_HitParamID);
			m_CharacterAudio.Hit(transform.position);
			m_eventSender?.Send(gameObject, "roleEvent_OnDamage");
			DamageUI.Instance.NewDamage(damage.GetFullDamage(), transform.position);
		};

		m_CharacterData.Equipment.OnEquiped += item =>
		{
			if (item.Slot == EquipmentItem.EquipmentSlot.Weapon && WeaponLocator)
			{
				Weapon wp = item as Weapon;
				BaseAI.EnemyDetector.Radius = System.Math.Max(wp.Stats.MaxRange, BaseAI.EnemyDetector.Radius);
				if (!item.WorldObjectPrefab) return;
				var obj = Instantiate(item.WorldObjectPrefab, WeaponLocator, false);
				Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
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
		if (m_CharacterData.Stats.CurrentHealth == 0 && m_State != State.DEAD)
		{
			SetState(State.DEAD);
			return;
		}
		if (m_State == State.DEAD) return;
		//Skill
		if (m_SkillUser && m_State != State.SKILLING && m_SkillUser.CurSkill != null) { SetState(State.SKILLING); return; }
		if (m_SkillUser && m_State == State.SKILLING && m_SkillUser.CurSkill == null) { SetState(State.IDLE); return; }
		//ATTACK
		if (m_State == State.ATTACKING && Data.CanAttack)
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
				if (Data.CanAttack) CheckAttack();
			}
			else { SetState(State.IDLE); return; }
		}
		//MOVE
		if (m_State == State.MOVE)
		{
			if (m_Destination != Vector3.zero && Vector3.SqrMagnitude(m_Destination - transform.position) <= 1)
			{
				m_eventSender?.Send(gameObject, "roleEvent_OnMoveEnd");
				SetState(State.IDLE);
				return;
			}
			m_eventSender?.Send(gameObject, "roleEvent_OnMoving");
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
				m_Animator.SetTrigger(m_AttackParamID);
				m_CharacterData.AttackTriggered();
				SetState(State.ATTACKING);
			}
		}
		else SetState(State.PURSUING);
	}
	void AttackFrame()
	{
		//if we can't reach the target anymore when it's time to damage, then that attack miss.
		if (m_Enemy && m_CharacterData.CanAttackReach(m_Enemy))
		{
			m_CharacterData.Attack(m_Enemy);

			var attackPos = m_Enemy.transform.position + transform.up * 0.5f;
			VFXManager.PlayVFX(VFXType.Hit, attackPos);
			SFXManager.PlaySound(m_CharacterAudio.UseType, new SFXManager.PlayData() { Clip = m_CharacterData.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = attackPos });
		}
		//else Debug.Log("Miss Attack! " + Data.CharacterName);
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
		if (m_State == nextState) return;
		m_State = nextState;
		m_eventSender?.Send(gameObject, "roleEvent_OnState_" + System.Enum.GetName(typeof(State), m_State));
		switch (m_State)
		{
			case State.IDLE:
				m_Agent.isStopped = true;
				break;
			case State.MOVE:
				m_Agent.isStopped = false;
				if (m_Destination != Vector3.zero) m_Agent.SetDestination(m_Destination);
				break;
			case State.PURSUING:
				m_Agent.isStopped = false;
				break;
			case State.ATTACKING:
				m_Agent.isStopped = true;
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

	void FootstepFrame()
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

	public void Move(Vector3 direction)
	{
		if (direction.magnitude > 0)
		{
			m_Agent.CalculatePath(transform.position + direction, m_CalculatedPath);
			if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
			{
				m_Agent.SetPath(m_CalculatedPath);
				m_CalculatedPath.ClearCorners();
				SetState(State.MOVE);
			}
		}
	}

	public void MoveTo(Vector3 pos)
	{
		m_Destination = pos;
		SetState(State.MOVE);
	}
}


