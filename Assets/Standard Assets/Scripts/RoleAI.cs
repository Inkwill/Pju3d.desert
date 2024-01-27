using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;

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
	Vector3[] m_paths;
	int m_curPathIndex;

	void Awake()
	{
		m_role = GetComponent<RoleControl>();
		Detector.OnEnter.AddListener(OnTargetEnter);
		Detector.OnExit.AddListener(OnTargetExit);
		Detector.OnEvent.AddListener(OnTargetEvent);
		Detector.layers = (camp == Camp.ENEMY) ? LayerMask.GetMask("Player") : LayerMask.GetMask("Enemy");
		m_role.gameObject.layer = (camp == Camp.ENEMY) ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
	}
	public void HandleState(RoleControl.State state, float during)
	{
		//Debug.Log("HandleState :" + state + " during = " + during);
		if (state == SimpleEnemyController.State.PURSUING)
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
	}
	public void SetPath(Vector3[] path)
	{
		m_paths = path;
		m_curPathIndex = 0;
	}
	void OnTargetEnter(GameObject enter)
	{
		if (!m_role.CurrentEnemy) m_role.CurrentEnemy = enter.GetComponent<CharacterData>();
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
		if (eventMessage == "roleEvent_OnState_DEAD") Debug.Log("OnTargetEvent: target= " + sender + "event= " + eventMessage);
	}
}
