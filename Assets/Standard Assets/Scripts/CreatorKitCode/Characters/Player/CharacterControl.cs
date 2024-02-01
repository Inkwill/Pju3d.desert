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
	public class CharacterControl : RoleControl,
		AnimationControllerDispatcher.IAttackFrameReceiver,
		AnimationControllerDispatcher.IFootstepFrameReceiver,
		AnimationControllerDispatcher.ISkillstepFrameReceiver
	{

		public InteractOnTrigger Detector_item;


		//HighlightableObject m_Highlighted;

		//RaycastHit[] m_RaycastHitCache = new RaycastHit[16];

		//bool m_IsKO = false;
		//float m_KOTimer = 0.0f;

		//int m_InteractableLayer;
		//int m_LevelLayer;
		//Collider m_TargetCollider;
		//InteractableObject m_TargetInteractable = null;

		NavMeshPath m_CalculatedPath;

		//this is a flag that tell the controller it need to clear the target once the attack finished.
		//usefull for when clicking elwswhere mid attack animation, allow to finish the attack and then exit.
		//bool m_ClearPostAttack = false;

		//SpawnPoint m_CurrentSpawn = null;

		void Start()
		{
			m_CalculatedPath = new NavMeshPath();
			SetState(State.IDLE);
		}

		void FixedUpdate()
		{
			if (m_State == State.DEAD || m_State == State.SKILLING) return;

			Vector3 direction = Vector3.forward * GameManager.GameUI.JoyStick.Vertical + Vector3.right * GameManager.GameUI.JoyStick.Horizontal;
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
			else if (direction.magnitude == 0 && m_State == State.MOVE)
			{
				SetState(State.IDLE);
			}
		}
		// public void ChangeState(State state, bool active)
		// {
		// 	switch (state)
		// 	{
		// 		case State.WORKING:
		// 			if (m_State == State.IDLE && active)
		// 			{
		// 				SetState(state);
		// 				m_Animator.SetTrigger(m_WokingID);
		// 			}
		// 			else if (!active)
		// 			{
		// 				SetState(State.IDLE);
		// 			}
		// 			break;
		// 		default:
		// 			Debug.LogError("Cant manual change character state to :" + state);
		// 			break;
		// 	}
		// }
		//void Update()
		//{

		//The update need to run, so we can check the health here.
		//Another method would be to add a callback in the CharacterData that get called
		//when health reach 0, and this class register to the callback in Start
		//(see CharacterData.OnDamage for an example)


		//Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);



		//m_Animator.SetFloat(m_SpeedParamID, m_Agent.velocity.magnitude / m_Agent.speed);
		/*
			if (Input.GetMouseButtonDown(0))
			{ //if we click the mouse button, we clear any previously et targets

				if (m_State != State.ATTACKING)
				{
					m_Enemy = null;
					m_TargetInteractable = null;
				}
				else
				{
					m_ClearPostAttack = true;
				}
			}


		if (!EventSystem.current.IsPointerOverGameObject() && m_State != State.ATTACKING)
		{
			//Raycast to find object currently under the mouse cursor
			ObjectsRaycasts(screenRay);

			if (Input.GetMouseButton(0))
			{
				if (m_TargetInteractable == null && m_Enemy == null)
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
							m_Enemy = data;
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
		//}

		// void GoToRespawn()
		// {
		// 	m_Animator.ResetTrigger(m_HitParamID);

		// 	m_Agent.Warp(m_CurrentSpawn.transform.position);
		// 	m_Agent.isStopped = true;
		// 	m_Agent.ResetPath();

		// 	CurrentEnemy = null;
		// 	m_TargetInteractable = null;

		// 	SetState(State.IDLE);

		// 	m_Animator.SetTrigger(m_RespawnParamID);

		// 	m_CharacterData.Stats.ChangeHealth(m_CharacterData.Stats.stats.health);
		// }

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

		// void SwitchHighlightedObject(HighlightableObject obj)
		// {
		// 	if (m_Highlighted != null) m_Highlighted.Dehighlight();

		// 	m_Highlighted = obj;
		// 	if (m_Highlighted != null) m_Highlighted.Highlight();
		// }

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
		// 	if (m_State == State.ATTACKING)
		// 		return;

		// 	Vector3 distance = m_TargetCollider.ClosestPointOnBounds(transform.position) - transform.position;


		// 	if (distance.sqrMagnitude < 1.5f * 1.5f)
		// 	{
		// 		StopAgent();
		// 		m_TargetInteractable.InteractWith(m_CharacterData);
		// 		m_TargetInteractable = null;
		// 	}
		// }



		// public void SetNewRespawn(SpawnPoint point)
		// {
		// 	if (m_CurrentSpawn != null)
		// 		m_CurrentSpawn.Deactivated();

		// 	m_CurrentSpawn = point;
		// 	m_CurrentSpawn.Activated();
		// }

		// public void InteractWith(InteractableObject obj)
		// {
		// 	if (obj.IsInteractable)
		// 	{
		// 		m_TargetCollider = obj.GetComponentInChildren<Collider>();
		// 		m_TargetInteractable = obj;
		// 		m_Agent.SetDestination(obj.transform.position);
		// 	}
		// }
	}
}