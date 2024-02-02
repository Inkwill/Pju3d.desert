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
		ENEMY,
		NEUTRAL
	}
	public Camp camp;
	public InteractOnTrigger EnemyDetector;
	public InteractOnTrigger InteractDetector;
	public InteractOnTrigger SceneDetector;
	public AudioClip[] SpottedAudioClip;
	RoleControl m_role;
	Transform[] m_paths;
	int m_curPathIndex;

	Renderer m_Renderer;

	bool m_buildmodel = true;
	public bool BuildModel
	{
		get { return m_buildmodel; }
		set
		{
			m_buildmodel = value;
			if (m_Renderer) m_Renderer.enabled = m_buildmodel;
		}
	}

	public bool CanDig
	{
		get { return SceneBoxInfo(false) == "blank"; }
		set { }
	}

	public void Init(RoleControl role)
	{
		m_role = role;
		m_role.eventSender.events.AddListener(OnRoleEvent);
		EnemyDetector.OnEnter.AddListener(OnEnemyEnter);
		EnemyDetector.OnExit.AddListener(OnEnemyExit);
		EnemyDetector.OnEvent.AddListener(OnEnemyEvent);
		m_Renderer = SceneDetector.GetComponent<MeshRenderer>();
		// m_role = role;
		InteractDetector.layers = LayerMask.GetMask("Interactable");
		SceneDetector.layers = LayerMask.GetMask("Scene");
		switch (m_role.BaseAI.camp)
		{
			case RoleAI.Camp.PLAYER:
				m_role.gameObject.layer = LayerMask.NameToLayer("Player");
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				break;
			case RoleAI.Camp.ENEMY:
				m_role.gameObject.layer = LayerMask.NameToLayer("Enemy");
				EnemyDetector.layers = LayerMask.GetMask("Player");
				break;
			case RoleAI.Camp.NEUTRAL:
				m_role.gameObject.layer = LayerMask.NameToLayer("Interactable");
				EnemyDetector.layers = LayerMask.GetMask("Noting");
				break;
			default:
				break;
		}
	}

	void FixedUpdate()
	{
		switch (SceneBoxInfo(false))
		{
			case "blank":
				m_Renderer.material.color = Color.green;
				break;
			case "pit":
				m_Renderer.material.color = Color.red;
				break;
			case "npc":
				m_Renderer.material.color = Color.red;
				break;
			default:
				m_Renderer.material.color = Color.red;
				break;
		}
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
	void OnEnemyEnter(GameObject enter)
	{
		if (m_role.isStandby) m_role.CurrentEnemy = enter.GetComponent<CharacterData>();
		//enter.GetComponent<EventSender>()?.events.AddListener(OnEnemyEvent);
		//m_eventSender.Send(enter, "roleEvent_OnEnemyEnter");
	}

	void OnEnemyExit(GameObject exiter)
	{
		if (m_role.CurrentEnemy && m_role.CurrentEnemy.gameObject == exiter)
		{
			m_role.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
			//m_eventSender.Send(exiter, "roleEvent_OnEnemyExit");
		}
	}
	void OnEnemyEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD")
		{
			m_role.CurState = RoleControl.State.IDLE;
			Debug.Log("OnEnemyEvent: target= " + sender + "event= " + eventMessage);
		}
	}
	void HighlightTarget(GameObject obj, bool active)
	{
		HighlightableObject target = obj.GetComponent<HighlightableObject>();
		if (target)
		{
			if (active) target.Highlight();
			else target.Dehighlight();
		}
	}
	public string SceneBoxInfo(bool display)
	{
		GameObject sceneBox = SceneDetector.lastInner;
		if (!sceneBox) return display ? "空地" : "blank";
		string sceneTag = sceneBox.tag;
		switch (sceneTag)
		{
			case "road":
				return display ? "道路" : sceneTag;
			case "building":
				return display ? "建筑:" + sceneBox : sceneTag;
			case "pit":
				return display ? "坑:" + sceneBox : sceneTag;
			case "mount":
				return display ? "山体:" : sceneTag;
			case "npc":
				return display ? "npc:" + sceneBox : sceneTag;
			case "creater":
				return display ? "建造中..." : sceneTag;
			default:
				break;
		}
		return display ? "空地" : "blank";
	}
}
