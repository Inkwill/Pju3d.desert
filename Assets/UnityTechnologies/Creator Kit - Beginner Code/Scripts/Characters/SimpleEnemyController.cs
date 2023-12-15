using CreatorKitCode;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace CreatorKitCodeInternal
{
	public class SimpleEnemyController : MonoBehaviour,
		AnimationControllerDispatcher.IAttackFrameReceiver,
		AnimationControllerDispatcher.IFootstepFrameReceiver
	{
		public enum State
		{
			IDLE,
			MOVE,
			PURSUING,
			ATTACKING,
			Dead
		}

		public float Speed = 6.0f;
		public float detectionRadius = 10.0f;
		public Transform pathRoot;
		public AudioClip[] SpottedAudioClip;
		public List<Transform> m_pathList;

		public State m_State;

		Vector3 m_StartingAnchor;
		Animator m_Animator;
		NavMeshAgent m_Agent;
		CharacterData m_CharacterData;
		CharacterAudio m_CharacterAudio;

		int m_SpeedAnimHash;
		int m_AttackAnimHash;
		int m_DeathAnimHash;
		int m_HitAnimHash;
		bool m_Pursuing;
		float m_PursuitTimer = 0.0f;

		//LootSpawner m_LootSpawner;

		int m_curPathIndex;

		float m_idleDuring;

		InteractOnTrigger m_Detector;

		GameObject m_Enemy;

		// Start is called before the first frame update
		void Start()
		{
			m_Animator = GetComponentInChildren<Animator>();
			m_Agent = GetComponent<NavMeshAgent>();
			m_Detector = GetComponentInChildren<InteractOnTrigger>();

			m_SpeedAnimHash = Animator.StringToHash("Speed");
			m_AttackAnimHash = Animator.StringToHash("Attack");
			m_DeathAnimHash = Animator.StringToHash("Death");
			m_HitAnimHash = Animator.StringToHash("Hit");

			m_CharacterData = GetComponent<CharacterData>();
			m_CharacterData.Init();

			m_CharacterAudio = GetComponentInChildren<CharacterAudio>();

			m_CharacterData.OnDamage += () =>
			{
				m_Animator.SetTrigger(m_HitAnimHash);
				m_CharacterAudio.Hit(transform.position);
			};

			m_Agent.speed = Speed;

			//m_LootSpawner = GetComponent<LootSpawner>();

			m_StartingAnchor = transform.position;

			foreach (Transform child in pathRoot)
			{
				m_pathList.Add(child);
			}
			m_curPathIndex = 0;
		}

		// Update is called once per frame
		void Update()
		{
			//See the Update function of CharacterControl.cs for a comment on how we could replace
			//this (polling health) to a callback method.
			if (m_State != State.Dead && m_CharacterData.Stats.CurrentHealth == 0)
			{
				m_Animator.SetTrigger(m_DeathAnimHash);

				m_CharacterAudio.Death(transform.position);
				m_CharacterData.Death();

				// if (m_LootSpawner != null)
				// 	m_LootSpawner.SpawnLoot();

				//Destroy(m_Agent);
				//Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
				//Destroy(GetComponent<Collider>());
				//Destroy(this);
				SetState(State.Dead);
				return;
			}

			switch (m_State)
			{
				case State.IDLE:
					{
						m_idleDuring += Time.deltaTime;
						if (m_Enemy)
						{
							if (SpottedAudioClip.Length != 0)
							{
								SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
								{
									Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
									Position = transform.position
								});
							}

							SetState(State.PURSUING);
						}
						else if (m_idleDuring > 3)
						{
							SetState(State.MOVE);
						}
					}
					break;
				case State.MOVE:
					{
						if (Vector3.SqrMagnitude(m_pathList[m_curPathIndex].position - transform.position) <= 1)
						{
							if (m_curPathIndex + 1 < m_pathList.Count)
							{
								m_curPathIndex++;
							}

							SetState(State.IDLE);
						}
					}
					break;
				case State.PURSUING:
					{
						if (!m_Enemy)
						{
							//m_Agent.SetDestination(m_StartingAnchor);
							SetState(State.MOVE);
						}
						else
						{
							m_Agent.SetDestination(m_Enemy.transform.position);
							float distToEnemy = Vector3.SqrMagnitude(m_Enemy.transform.position - transform.position);
							CharacterData enemyData = m_Enemy.GetComponent<CharacterData>();
							if (m_CharacterData.CanAttackTarget(enemyData))
							{
								m_CharacterData.AttackTriggered();
								m_Animator.SetTrigger(m_AttackAnimHash);
								m_Agent.ResetPath();
								m_Agent.velocity = Vector3.zero;
								SetState(State.ATTACKING);
							}
						}
					}
					break;
				case State.ATTACKING:
					{
						if (!m_Enemy)
						{
							//m_Agent.SetDestination(m_StartingAnchor);
							SetState(State.MOVE);
						}
						else
						{
							CharacterData enemyData = m_Enemy.GetComponent<CharacterData>();
							if (!m_CharacterData.CanAttackReach(enemyData))
							{
								SetState(State.PURSUING);
							}
							else
							{
								if (m_CharacterData.CanAttackTarget(enemyData))
								{
									m_CharacterData.AttackTriggered();
									m_Animator.SetTrigger(m_AttackAnimHash);
								}
							}
						}
					}
					break;
				case State.Dead:
					if (gameObject.layer != LayerMask.NameToLayer("EnemyCorpse"))
					{
						Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("EnemyCorpse"));
						//Destroy(GetComponent<Rigidbody>());
					}
					break;
			}

			m_Animator.SetFloat(m_SpeedAnimHash, m_Agent.velocity.magnitude / Speed);
		}

		public void AttackFrame()
		{
			CharacterData playerData = CharacterControl.Instance.Data;

			//if we can't reach the player anymore when it's time to damage, then that attack miss.
			if (!m_CharacterData.CanAttackReach(playerData))
				return;

			m_CharacterData.Attack(playerData);
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, detectionRadius);
		}

		public void FootstepFrame()
		{
			Vector3 pos = transform.position;

			m_CharacterAudio.Step(pos);
			VFXManager.PlayVFX(VFXType.StepPuff, pos);
		}

		public void OnDetected(string type)
		{
			//Debug.Log("OnDetected, obj = " + gameObject + " type= " + type + " target= " + m_Detector.lastInteracter);
			m_Enemy = m_Detector.GetIntruder();
			//Debug.Log("m_Enemy = " + m_Enemy);
		}

		void SetState(State nextState)
		{
			if (m_State != nextState)
			{
				m_State = nextState;
				GetComponent<EventSender>()?.Send(gameObject, System.Enum.GetName(typeof(State), m_State));
				switch (m_State)
				{
					case State.IDLE:
						m_idleDuring = 0;
						break;
					case State.MOVE:
						m_Agent.SetDestination(m_pathList[m_curPathIndex].position);
						m_Agent.isStopped = false;
						break;
					case State.PURSUING:
						m_Agent.isStopped = false;
						break;
					case State.ATTACKING:
						m_Agent.isStopped = true;
						break;
					case State.Dead:
						m_Agent.isStopped = true;
						m_Agent.enabled = false;
						break;
				}
			}
		}
	}
}