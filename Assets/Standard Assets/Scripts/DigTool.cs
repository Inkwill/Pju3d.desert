using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using CreatorKitCodeInternal;
using UnityEngine.AI;

public class DigTool : MonoBehaviour
{
	RoleControl m_role;
	InteractOnTrigger m_Detector;
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

	// Update is called once per frame
	private void Start()
	{
		m_Detector = GetComponent<InteractOnTrigger>();
		m_role = GetComponentInParent<RoleControl>();
		//m_Detector.OnEnter.AddListener(OnEnter);
		//m_Detector.OnExit.AddListener(OnExit);
		m_Renderer = GetComponent<Renderer>();
		m_role.eventSender.events.AddListener(OnRoleEvent);
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
	void OnRoleEvent(GameObject role, string eventName)
	{
		//Debug.Log("DigTool OnEnter: " + enter);
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

	// void OnWorkCompleted(GameObject sender, string eventMessage)
	// {
	// 	if (eventMessage == "event_work_completed")
	// 	{
	// 		//m_character.ChangeState(CharacterControl.State.WORKING, false);
	// 	}
	// }

	public string SceneBoxInfo(bool display)
	{
		GameObject sceneBox = m_Detector.lastInner;
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

	public T Create<T>(string pbName)
	{
		GameObject pbObj = Resources.Load(pbName) as GameObject;
		T target = default(T);
		if (pbObj) target = Instantiate(pbObj, transform.position, Quaternion.Euler(0, 180, 0)).GetComponent<T>();

		//creater.GetComponent<EventSender>()?.m_event.AddListener(OnWorkCompleted);
		//m_character.PlayWork(true);
		//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
		//creater.DoCreate(m_character, pbName);
		return target;
	}

	// public void DoPlant(string pbName)
	// {
	// 	Pit pit = GetLastInner<Pit>();
	// 	if (pit) pit.DoCreate(m_character, pbName);
	//creater = Instantiate(creater, transform.position, Quaternion.Euler(0, 180, 0));
	//creater.GetComponent<EventSender>()?.m_event.AddListener(OnWorkCompleted);
	//m_character.PlayWork(true);
	//creater.DoCreate(m_character, pbName);
	//}

	// public T GetLastInner<T>()
	// {
	// 	if (m_Detector.lastInner)
	// 	{
	// 		return m_Detector.lastInner.GetComponent<T>();
	// 	}
	// 	else { return default(T); }
	// }
}
