using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using MyBox;

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
	public bool Offensive { get { return m_Offensive; } set { } }

	[ConditionalField(nameof(camp), false, Camp.ENEMY)]
	[SerializeField] bool m_Offensive;
	[ConditionalField(nameof(camp), true, Camp.PLAYER)]
	[SerializeField] float m_WanderRadius;
	[ConditionalField(nameof(camp), true, Camp.PLAYER)]
	[SerializeField] float m_WanderBeat = 3.0f;
	float m_IdleDuring;
	RoleControl m_role;
	GameObject m_interactTarget;

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
		InteractDetector.OnEnter.AddListener(OnInteracterEnter);
		InteractDetector.OnExit.AddListener(OnInteracterExit);
		InteractDetector.OnEvent.AddListener(OnInteracterEvent);
		m_Renderer = SceneDetector.GetComponent<MeshRenderer>();
		// m_role = role;
		SceneDetector.layers = LayerMask.GetMask("Scene");
		switch (m_role.BaseAI.camp)
		{
			case RoleAI.Camp.PLAYER:
				m_role.gameObject.layer = LayerMask.NameToLayer("Player");
				EnemyDetector.layers = LayerMask.GetMask("Enemy");
				InteractDetector.layers = LayerMask.GetMask("Interactable");
				m_Offensive = true;
				m_WanderRadius = 0;
				break;
			case RoleAI.Camp.ENEMY:
				m_role.gameObject.layer = LayerMask.NameToLayer("Enemy");
				EnemyDetector.layers = LayerMask.GetMask("Player");
				InteractDetector.layers = LayerMask.GetMask("Interactable");
				break;
			case RoleAI.Camp.NEUTRAL:
				m_role.gameObject.layer = LayerMask.NameToLayer("Interactable");
				EnemyDetector.layers = LayerMask.GetMask("Noting");
				InteractDetector.layers = LayerMask.GetMask("Interactable", "Player");
				m_Offensive = false;
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
		if (eventName == "roleEvent_OnIdling")
		{
			m_IdleDuring += Time.deltaTime;
			if (m_IdleDuring > m_WanderBeat && m_WanderRadius > 0)
			{
				Wandering();
				m_IdleDuring = 0;
			}
		}
		if (eventName == "roleEvent_OnDamage")
		{
			if (!m_Offensive) m_Offensive = true;
			if (!m_role.CurrentEnemy) m_role.CurrentEnemy = EnemyDetector.GetNearest()?.GetComponent<CharacterData>();
		}
		UIRoleHud hud = m_role.GetComponentInChildren<UIRoleHud>();
		if (hud != null) hud.Bubble(eventName);
	}

	void OnEnemyEnter(GameObject enter)
	{
		if (m_role.CurrentEnemy == null) m_role.CurrentEnemy = enter.GetComponent<CharacterData>();
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
			//m_role.CurState = RoleControl.State.IDLE;
			Debug.Log("OnEnemyEvent: target= " + sender + "event= " + eventMessage);
		}
	}

	void OnInteracterEnter(GameObject enter)
	{

	}

	void OnInteracterExit(GameObject exiter)
	{
		if (m_interactTarget && m_interactTarget == exiter) m_interactTarget = InteractDetector.GetNearest();
	}

	void OnInteracterEvent(GameObject sender, string eventMessage)
	{
		if (eventMessage == "roleEvent_OnState_DEAD" && m_interactTarget == sender)
		{
			m_interactTarget = InteractDetector.GetNearest();
		}
		if (eventMessage == "roleEvent_OnState_IDLE" && m_interactTarget == null && m_role.isIdle)
		{
			m_interactTarget = sender;
			GetComponentInChildren<UIRoleHud>()?.Bubble(sender.GetComponent<RoleControl>()?.Data.CharacterName);
			m_role.LookAt(sender.transform);
		}
		//Debug.Log("OnInteracterEvent: InteracterTarget= " + sender + "event= " + eventMessage);
	}

	void Wandering()
	{
		float randomX = Random.Range(0f, m_WanderRadius);
		float randomZ = Random.Range(0f, m_WanderRadius);

		Vector3 randomPos = new Vector3(m_role.BirthPos.x + randomX, m_role.BirthPos.y, m_role.BirthPos.z + randomZ);
		m_role.MoveTo(randomPos);
		//Debug.Log($"{m_role}-Wandering => {randomPos}");
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

// #if UNITY_EDITOR
// [CustomEditor(typeof(RoleAI))]
// public class RoleAIInspector : Editor  
// {
// 	SerializedObject obj;  
// 	RoleAI roleAi;  
// 	SerializedProperty camp;  
// 	SerializedProperty m_Offensive; 

// 	void OnEnable()
// 	{  
//     	obj = new SerializedObject(target);  
//     	m_Offensive = obj.FindProperty("m_Offensive");
// 	}

// 	public override void OnInspectorGUI()
// 	{
// 		base.OnInspectorGUI();
// 		roleAi = (RoleAI)target;
// 		if (roleAi.camp == RoleAI.Camp.ENEMY)
// 		{
// 			EditorGUILayout.PropertyField(m_Offensive);
// 		}
// 	}
// } 
// #endif
