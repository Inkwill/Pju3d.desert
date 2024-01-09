using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.AI;

public class DigTool : MonoBehaviour
{
	public Pit pitObj;

	public CharacterControl m_character;

	InteractOnTrigger m_Detector;


	// Update is called once per frame
	private void Start()
	{
		m_Detector = GetComponent<InteractOnTrigger>();
	}
	void FixedUpdate()
	{
		// if (Input.GetKeyDown(KeyCode.Return))
		// {

		// }
		// if (Input.GetKeyDown("space"))
		// {
		// 	Debug.Log("SceneDetector : " + m_Detector.GetSceneBox());
		// }
	}

	void OnWorkCompleted(GameObject sender, string eventMessage)
	{
		if (eventMessage == "event_work_completed")
		{
			m_character.PlayWork(false);
		}
	}

	public string SceneBoxInfo(bool display)
	{
		GameObject sceneBox = m_Detector.lastInner;
		if (!sceneBox) return display ? "空地" : null;
		string sceneTag = sceneBox.tag;
		switch (sceneTag)
		{
			case "road":
				return display ? "道路" : sceneTag;
				break;
			case "building":
				return display ? "建筑:" + sceneBox : sceneTag;
				break;
			case "pit":
				return display ? "坑:" + sceneBox : sceneTag;
				break;
			case "mount":
				return display ? "山体:" : sceneTag;
				break;
			case "npc":
				return display ? "npc:" + sceneBox : sceneTag;
				break;
			case "creater":
				return display ? "建造中..." : sceneTag;
				break;
			default:
				break;
		}
		return display ? "空地" : null;
	}

	public Creater DoCreate(string pbName)
	{
		Creater creater = Resources.Load<Creater>("creater");
		creater = Instantiate(creater, transform.position, Quaternion.Euler(0, 180, 0));
		//creater.GetComponent<EventSender>()?.m_event.AddListener(OnWorkCompleted);
		//m_character.PlayWork(true);
		creater.DoCreate(m_character, pbName);
		return creater;
	}

	public void DoPlant(string pbName)
	{
		Pit pit = GetLastInner<Pit>();
		if (pit) pit.DoCreate(m_character, pbName);
		//creater = Instantiate(creater, transform.position, Quaternion.Euler(0, 180, 0));
		//creater.GetComponent<EventSender>()?.m_event.AddListener(OnWorkCompleted);
		//m_character.PlayWork(true);
		//creater.DoCreate(m_character, pbName);
	}

	public T GetLastInner<T>()
	{
		if (m_Detector.lastInner)
		{
			return m_Detector.lastInner.GetComponent<T>();
		}
		else { return default(T); }
	}
}
