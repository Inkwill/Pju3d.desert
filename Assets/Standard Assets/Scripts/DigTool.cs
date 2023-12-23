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

	void OnPitCompleted(GameObject sender, string eventMessage)
	{
		if (eventMessage == "event_pit_completed")
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
			default:
				break;
		}
		return display ? "空地" : null;
	}

	public Pit DigPit()
	{
		Pit pit = Instantiate(pitObj, transform.position, Quaternion.Euler(0, 180, 0)) as Pit;
		pit.GetComponent<EventSender>()?.m_event.AddListener(OnPitCompleted);
		m_character.PlayWork(true);
		return pit;
	}

	public Creater Plant(string pbName)
	{
		Pit curPit = m_Detector.lastInner?.GetComponent<Pit>();
		Creater plant = Resources.Load<Creater>(pbName);
		plant = Instantiate(plant, curPit.gameObject.transform.position, Quaternion.Euler(0, 180, 0));
		curPit.OnPlanted(plant);
		return plant;
		//plant.GetComponent<EventSender>()?.m_event.AddListener(OnCreateCompleted);
	}
}
