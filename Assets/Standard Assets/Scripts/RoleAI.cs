using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

[RequireComponent(typeof(RoleControl))]
public class RoleAI : MonoBehaviour
{
	public enum Camp
	{
		PLAYER,
		ENEMY
	}
	public Camp camp;
	public AudioClip[] SpottedAudioClip;
	public InteractOnTrigger Detector;
	RoleControl m_role;
	Transform[] m_paths;
	int m_curPathIndex;

	void Awake()
	{
		m_role = GetComponent<RoleControl>();
		m_role.eventSender.events.AddListener(OnRoleEvent);
		Detector.OnEnter.AddListener(OnTargetEnter);
		Detector.OnExit.AddListener(OnTargetExit);
		Detector.OnEvent.AddListener(OnTargetEvent);
		Detector.layers = (camp == Camp.ENEMY) ? LayerMask.GetMask("Player") : LayerMask.GetMask("Enemy");
		m_role.gameObject.layer = (camp == Camp.ENEMY) ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
	}

	void OnRoleEvent(GameObject obj, string eventName)
	{
		if (camp == Camp.ENEMY && eventName == "roleEvent_OnMoveEnd" && m_paths.Length > m_curPathIndex)
		{
			m_curPathIndex++;
		}
	}

	public void HandleState(RoleControl.State state, float during)
	{
		//if (camp == Camp.PLAYER) Debug.Log("HandleState :" + state + " during = " + during);
		if (state == RoleControl.State.PURSUING)
		{
			if (SpottedAudioClip.Length != 0)
			{
				SFXManager.PlaySound(SFXManager.Use.Enemies, new SFXManager.PlayData()
				{
					Clip = SpottedAudioClip[Random.Range(0, SpottedAudioClip.Length)],
					Position = transform.position
				});
			}
		}
		if (state == RoleControl.State.IDLE && camp == Camp.ENEMY && m_paths.Length > m_curPathIndex)
		{
			m_role.MoveTo(m_paths[m_curPathIndex].position);
		}
	}
	public void SetPath(Transform pathRoot)
	{
		m_paths = pathRoot.GetComponentsInChildren<Transform>();
		m_curPathIndex = 0;
	}
	void OnTargetEnter(GameObject enter)
	{
		if (m_role.isStandby) m_role.CurrentEnemy = enter.GetComponent<CharacterData>();
		//enter.GetComponent<EventSender>()?.events.AddListener(OnTargetEvent);
		//m_eventSender.Send(enter, "roleEvent_OnTargetEnter");
	}

	void OnTargetExit(GameObject exiter)
	{
		if (m_role.CurrentEnemy && m_role.CurrentEnemy.gameObject == exiter)
		{
			m_role.CurrentEnemy = Detector.GetNearest()?.GetComponent<CharacterData>();
			//m_eventSender.Send(exiter, "roleEvent_OnTargetExit");
		}
	}
	void OnTargetEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			m_role.CurState = RoleControl.State.IDLE;
			Debug.Log("OnTargetEvent: target= " + sender + "event= " + eventMessage);
		}
	}
}
