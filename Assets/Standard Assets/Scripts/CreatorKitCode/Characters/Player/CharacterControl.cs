using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace CreatorKitCodeInternal
{
	public class CharacterControl : MonoBehaviour,
		AnimationControllerDispatcher.IAttackFrameReceiver,
		AnimationControllerDispatcher.IFootstepFrameReceiver
	{
		public static CharacterControl Instance { get; protected set; }

		public VariableJoystick variableJoystick;

		public float Speed = 10.0f;

		public CharacterData Data => m_CharacterData;
		public CharacterData CurrentTarget
		{
			get { return m_CurrentTargetCharacterData; }
			set
			{
				m_CurrentTargetCharacterData = value;
				GameObject target = value ? m_CurrentTargetCharacterData.gameObject : null;
				m_eventSender.Send(target, "onTarget");
			}
		}

		public Transform WeaponLocator;

		public ParticleSystem fx_Working;

		[Header("Audio")]
		public AudioClip[] SpurSoundClips;

		public bool canWork { get { return (m_CurrentState == State.DEFAULT) && !m_CurrentTargetCharacterData; } protected set { } }

		Vector3 m_LastRaycastResult;
		Animator m_Animator;
		NavMeshAgent m_Agent;
		CharacterData m_CharacterData;

		HighlightableObject m_Highlighted;

		RaycastHit[] m_RaycastHitCache = new RaycastHit[16];

		InteractOnTrigger m_Detector;
		int m_SpeedParamID;
		int m_AttackParamID;
		int m_HitParamID;
		int m_FaintParamID;
		int m_RespawnParamID;
		int m_WokingID;

		//bool m_IsKO = false;
		float m_KOTimer = 0.0f;

		int m_InteractableLayer;
		//int m_LevelLayer;
		//Collider m_TargetCollider;
		InteractableObject m_TargetInteractable = null;
		Camera m_MainCamera;

		NavMeshPath m_CalculatedPath;

		CharacterAudio m_CharacterAudio;

		int m_TargetLayer;
		CharacterData m_CurrentTargetCharacterData = null;
		//this is a flag that tell the controller it need to clear the target once the attack finished.
		//usefull for when clicking elwswhere mid attack animation, allow to finish the attack and then exit.
		bool m_ClearPostAttack = false;

		SpawnPoint m_CurrentSpawn = null;

		DigTool m_digTool;
		EventSender m_eventSender;

		public UICharacterHud hud;

		public enum State
		{
			DEFAULT,
			MOVE,
			PURSUING,
			HIT,
			ATTACKING,
			WORKING,
			Dead
		}

		State m_CurrentState;

		void Awake()
		{
			Instance = this;
			m_MainCamera = Camera.main;
			m_CharacterData = GetComponent<CharacterData>();
		}

		// Start is called before the first frame update
		void Start()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;

			m_CalculatedPath = new NavMeshPath();

			m_Agent = GetComponent<NavMeshAgent>();
			m_Animator = GetComponentInChildren<Animator>();
			m_Detector = GetComponentInChildren<InteractOnTrigger>();
			m_Detector.OnEnter.AddListener(OnEnter);
			m_Detector.OnExit.AddListener(OnExit);
			m_digTool = GetComponentInChildren<DigTool>();
			hud = GetComponentInChildren<UICharacterHud>();
			m_eventSender = GetComponent<EventSender>();

			m_Agent.speed = Speed;
			m_Agent.angularSpeed = 360.0f;

			m_LastRaycastResult = transform.position;

			m_SpeedParamID = Animator.StringToHash("Speed");
			m_AttackParamID = Animator.StringToHash("Attack");
			m_HitParamID = Animator.StringToHash("Hit");
			m_FaintParamID = Animator.StringToHash("Faint");
			m_RespawnParamID = Animator.StringToHash("Respawn");
			m_WokingID = Animator.StringToHash("Attack");



			m_CharacterData.Equipment.OnEquiped += item =>
			{
				if (item.Slot == (EquipmentItem.EquipmentSlot)666)
				{
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

			m_CharacterData.Init();

			//m_InteractableLayer = 1 << LayerMask.NameToLayer("Interactable");
			//m_LevelLayer = 1 << LayerMask.NameToLayer("Level");
			//m_TargetLayer = 1 << LayerMask.NameToLayer("Target");

			SetState(State.DEFAULT);

			m_CharacterAudio = GetComponent<CharacterAudio>();

			m_CharacterData.OnDamage += () =>
			{
				m_Animator.SetTrigger(m_HitParamID);
				m_CharacterAudio.Hit(transform.position);
			};
		}

		void FixedUpdate()
		{
			if (m_CurrentState == State.Dead || m_CurrentState == State.WORKING) return;

			if (m_CurrentTargetCharacterData == null && m_CurrentState == State.DEFAULT)
			{
				GameObject enemy = m_Detector.GetNearest();
				if (enemy) CurrentTarget = enemy.GetComponent<CharacterData>();
			}

			Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
			if (direction.magnitude > 0)
			{
				//Debug.Log(direction);
				//m_Agent.Move(direction * 0.1f);
				m_Agent.CalculatePath(transform.position + direction, m_CalculatedPath);
				if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
				{
					m_Agent.SetPath(m_CalculatedPath);
					m_CalculatedPath.ClearCorners();
					SetState(State.MOVE);
				}
			}
			else if (direction.magnitude == 0 && m_CurrentState == State.MOVE)
			{
				SetState(State.DEFAULT);
			}
		}

		// Update is called once per frame
		void Update()
		{
			Vector3 pos = transform.position;

			if (m_CurrentState == State.Dead)
			{
				m_KOTimer += Time.deltaTime;
				m_Agent.enabled = false;
				if (gameObject.layer != LayerMask.NameToLayer("PlayerCorpse"))
				{
					Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("PlayerCorpse"));
					//Destroy(GetComponent<Rigidbody>());
				}
				if (m_KOTimer > 3.0f && m_CurrentSpawn)
				{
					GoToRespawn();
				}

				return;
			}

			//The update need to run, so we can check the health here.
			//Another method would be to add a callback in the CharacterData that get called
			//when health reach 0, and this class register to the callback in Start
			//(see CharacterData.OnDamage for an example)
			if (m_CurrentState != State.Dead && m_CharacterData.Stats.CurrentHealth == 0)
			{
				m_Animator.SetTrigger(m_FaintParamID);

				m_Agent.isStopped = true;
				m_Agent.ResetPath();
				m_KOTimer = 0.0f;

				Data.Death();

				m_CharacterAudio.Death(pos);
				SetState(State.Dead);
				return;
			}

			//Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);

			if (m_CurrentTargetCharacterData != null)
			{
				if (m_CurrentTargetCharacterData.Stats.CurrentHealth == 0)
					CurrentTarget = null;
				else
					CheckAttack();
			}

			float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
			if (!Mathf.Approximately(mouseWheel, 0.0f))
			{
				Vector3 view = m_MainCamera.ScreenToViewportPoint(Input.mousePosition);
				if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
					CameraController.Instance.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
			}
			/*
				if (Input.GetMouseButtonDown(0))
				{ //if we click the mouse button, we clear any previously et targets

					if (m_CurrentState != State.ATTACKING)
					{
						m_CurrentTargetCharacterData = null;
						m_TargetInteractable = null;
					}
					else
					{
						m_ClearPostAttack = true;
					}
				}


			if (!EventSystem.current.IsPointerOverGameObject() && m_CurrentState != State.ATTACKING)
			{
				//Raycast to find object currently under the mouse cursor
				ObjectsRaycasts(screenRay);

				if (Input.GetMouseButton(0))
				{
					if (m_TargetInteractable == null && m_CurrentTargetCharacterData == null)
					{
						InteractableObject obj = m_Highlighted as InteractableObject;
						if (obj)
						{
							InteractWith(obj);
						}
						else
						{
							CharacterData data = m_Highlighted as CharacterData;
							if (data != null)
							{
								m_CurrentTargetCharacterData = data;
							}
							else
							{
								//MoveCheck(screenRay);
							}
						}
					}
				}
			}
			*/
			m_Animator.SetFloat(m_SpeedParamID, m_Agent.velocity.magnitude / m_Agent.speed);

			//Keyboard shortcuts
			if (Input.GetKeyUp(KeyCode.I))
				UISystem.Instance.ToggleInventory();
		}

		void GoToRespawn()
		{
			m_Animator.ResetTrigger(m_HitParamID);

			m_Agent.Warp(m_CurrentSpawn.transform.position);
			m_Agent.isStopped = true;
			m_Agent.ResetPath();

			CurrentTarget = null;
			m_TargetInteractable = null;

			SetState(State.DEFAULT);

			m_Animator.SetTrigger(m_RespawnParamID);

			m_CharacterData.Stats.ChangeHealth(m_CharacterData.Stats.stats.health);
		}

		// void ObjectsRaycasts(Ray screenRay)
		// {
		// 	bool somethingFound = false;

		// 	//first check for interactable Object
		// 	int count = Physics.SphereCastNonAlloc(screenRay, 1.0f, m_RaycastHitCache, 1000.0f, m_InteractableLayer);
		// 	if (count > 0)
		// 	{
		// 		for (int i = 0; i < count; ++i)
		// 		{
		// 			InteractableObject obj = m_RaycastHitCache[0].collider.GetComponentInParent<InteractableObject>();
		// 			if (obj != null && obj.IsInteractable)
		// 			{
		// 				SwitchHighlightedObject(obj);
		// 				somethingFound = true;
		// 				break;
		// 			}
		// 		}
		// 	}
		// 	else
		// 	{
		// 		count = Physics.SphereCastNonAlloc(screenRay, 1.0f, m_RaycastHitCache, 1000.0f, m_TargetLayer);

		// 		if (count > 0)
		// 		{
		// 			CharacterData data = m_RaycastHitCache[0].collider.GetComponentInParent<CharacterData>();
		// 			if (data != null)
		// 			{
		// 				SwitchHighlightedObject(data);
		// 				somethingFound = true;
		// 			}
		// 		}
		// 	}

		// 	if (!somethingFound && m_Highlighted != null)
		// 	{
		// 		SwitchHighlightedObject(null);
		// 	}
		// }

		void SwitchHighlightedObject(HighlightableObject obj)
		{
			if (m_Highlighted != null) m_Highlighted.Dehighlight();

			m_Highlighted = obj;
			if (m_Highlighted != null) m_Highlighted.Highlight();
		}

		// void MoveCheck(Ray screenRay)
		// {
		// 	if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
		// 	{
		// 		m_Agent.SetPath(m_CalculatedPath);
		// 		m_CalculatedPath.ClearCorners();
		// 	}

		// 	if (Physics.RaycastNonAlloc(screenRay, m_RaycastHitCache, 1000.0f, m_LevelLayer) > 0)
		// 	{
		// 		Vector3 point = m_RaycastHitCache[0].point;
		// 		//avoid recomputing path for close enough click
		// 		if (Vector3.SqrMagnitude(point - m_LastRaycastResult) > 1.0f)
		// 		{
		// 			NavMeshHit hit;
		// 			if (NavMesh.SamplePosition(point, out hit, 0.5f, NavMesh.AllAreas))
		// 			{//sample just around where we hit, avoid setting destination outside of navmesh (ie. on building)
		// 				m_LastRaycastResult = point;
		// 				//m_Agent.SetDestination(hit.position);

		// 				m_Agent.CalculatePath(hit.position, m_CalculatedPath);
		// 			}
		// 		}
		// 	}
		// }

		// void CheckInteractableRange()
		// {
		// 	if (m_CurrentState == State.ATTACKING)
		// 		return;

		// 	Vector3 distance = m_TargetCollider.ClosestPointOnBounds(transform.position) - transform.position;


		// 	if (distance.sqrMagnitude < 1.5f * 1.5f)
		// 	{
		// 		StopAgent();
		// 		m_TargetInteractable.InteractWith(m_CharacterData);
		// 		m_TargetInteractable = null;
		// 	}
		// }

		void StopAgent()
		{
			m_Agent.ResetPath();
			m_Agent.velocity = Vector3.zero;
		}

		void CheckAttack()
		{
			if (m_CurrentState == State.ATTACKING || m_CurrentState == State.MOVE || m_CurrentState == State.Dead || m_CurrentState == State.WORKING)
				return;

			if (m_CharacterData.CanAttackReach(m_CurrentTargetCharacterData))
			{
				StopAgent();

				//if the mouse button isn't pressed, we do NOT attack
				if (true)//Input.GetMouseButton(0))
				{
					Vector3 forward = (m_CurrentTargetCharacterData.transform.position - transform.position);
					forward.y = 0;
					forward.Normalize();


					transform.forward = forward;
					if (m_CharacterData.CanAttackTarget(m_CurrentTargetCharacterData))
					{
						SetState(State.ATTACKING);

						m_CharacterData.AttackTriggered();
						m_Animator.SetTrigger(m_AttackParamID);
					}
				}
			}
			else
			{
				m_Agent.SetDestination(m_CurrentTargetCharacterData.transform.position);
				SetState(State.PURSUING);
			}
		}

		public void AttackFrame()
		{
			if (m_CurrentState == State.WORKING)
			{
				m_Animator.SetTrigger(m_WokingID);
				if (fx_Working) fx_Working.Play();
				return;
			}

			if (m_CurrentTargetCharacterData == null)
			{
				m_ClearPostAttack = false;
				return;
			}

			//if we can't reach the target anymore when it's time to damage, then that attack miss.
			if (m_CharacterData.CanAttackReach(m_CurrentTargetCharacterData))
			{
				m_CharacterData.Attack(m_CurrentTargetCharacterData);

				var attackPos = m_CurrentTargetCharacterData.transform.position + transform.up * 0.5f;
				VFXManager.PlayVFX(VFXType.Hit, attackPos);
				SFXManager.PlaySound(m_CharacterAudio.UseType, new SFXManager.PlayData() { Clip = m_CharacterData.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = attackPos });
			}

			if (m_ClearPostAttack)
			{
				m_ClearPostAttack = false;
				CurrentTarget = null;
				m_TargetInteractable = null;
			}

			SetState(State.DEFAULT);
		}

		public void SetNewRespawn(SpawnPoint point)
		{
			if (m_CurrentSpawn != null)
				m_CurrentSpawn.Deactivated();

			m_CurrentSpawn = point;
			m_CurrentSpawn.Activated();
		}

		// public void InteractWith(InteractableObject obj)
		// {
		// 	if (obj.IsInteractable)
		// 	{
		// 		m_TargetCollider = obj.GetComponentInChildren<Collider>();
		// 		m_TargetInteractable = obj;
		// 		m_Agent.SetDestination(obj.transform.position);
		// 	}
		// }

		public void FootstepFrame()
		{
			Vector3 pos = transform.position;

			m_CharacterAudio.Step(pos);

			SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
			{
				Clip = SpurSoundClips[Random.Range(0, SpurSoundClips.Length)],
				Position = pos,
				PitchMin = 0.8f,
				PitchMax = 1.2f,
				Volume = 0.3f
			});

			VFXManager.PlayVFX(VFXType.StepPuff, pos);
		}

		void SetState(State nextState)
		{
			if (m_CurrentState != nextState)
			{
				//Debug.Log("curState=" + m_CurrentState + "   nextState=" + nextState);
				m_CurrentState = nextState;
				GetComponent<EventSender>()?.Send(gameObject, System.Enum.GetName(typeof(State), m_CurrentState));
			}
		}

		void OnEnter(GameObject enter)
		{
			GetComponent<EventSender>()?.Send(enter, "enemy_enter");
		}

		void OnExit(GameObject exiter)
		{
			if (m_CurrentTargetCharacterData && m_CurrentTargetCharacterData.gameObject == exiter)
			{
				CurrentTarget = m_Detector.GetNearest()?.GetComponent<CharacterData>();
			}
			else
			{
				Debug.Log("kill enemy : " + exiter);
			}
			GetComponent<EventSender>()?.Send(exiter, "enemy_exiter");
		}

		public void ChangeState(State state, bool active)
		{
			switch (state)
			{
				case State.WORKING:
					if (m_CurrentState == State.DEFAULT && active)
					{
						SetState(state);
						m_Animator.SetTrigger(m_WokingID);
					}
					else if (!active)
					{
						SetState(State.DEFAULT);
					}
					break;
				default:
					Debug.LogError("Cant manual change character state to :" + state);
					break;
			}
		}
	}
}