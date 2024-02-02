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


	// Update is called once per frame
	// private void Start()
	// {
	// 	m_Detector = GetComponent<InteractOnTrigger>();
	// 	m_role = GetComponentInParent<RoleControl>();
	// 	//m_Detector.OnEnter.AddListener(OnEnter);
	// 	//m_Detector.OnExit.AddListener(OnExit);
	// 	m_Renderer = GetComponent<Renderer>();
	// 	m_role.eventSender.events.AddListener(OnRoleEvent);
	// }


	// void OnRoleEvent(GameObject role, string eventName)
	// {
	// 	//Debug.Log("DigTool OnEnter: " + enter);
	// }



	// void OnWorkCompleted(GameObject sender, string eventMessage)
	// {
	// 	if (eventMessage == "event_work_completed")
	// 	{
	// 		//m_character.ChangeState(CharacterControl.State.WORKING, false);
	// 	}
	// }



	// public T Create<T>(string pbName)
	// {
	// 	GameObject pbObj = Resources.Load(pbName) as GameObject;
	// 	T target = default(T);
	// 	if (pbObj) target = Instantiate(pbObj, transform.position, Quaternion.Euler(0, 180, 0)).GetComponent<T>();

	// 	//creater.GetComponent<EventSender>()?.m_event.AddListener(OnWorkCompleted);
	// 	//m_character.PlayWork(true);
	// 	//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
	// 	//creater.DoCreate(m_character, pbName);
	// 	return target;
	// }

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
