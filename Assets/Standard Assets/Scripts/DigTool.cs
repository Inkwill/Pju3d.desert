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
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Pit pit = Instantiate(pitObj, transform.position, Quaternion.Euler(0, 180, 0)) as Pit;
			pit.GetComponent<EventSender>()?.m_event.AddListener(OnPitCompleted);
			m_character.PlayWork(true);
		}
		if (Input.GetKeyDown("space"))
		{
			Debug.Log("SceneDetector : " + m_Detector.GetSceneBox());
		}
	}

	void OnPitCompleted(GameObject sender, string eventMessage)
	{
		if (eventMessage == "event_pit_completed")
		{
			m_character.PlayWork(false);
		}
	}

	public string SceneBoxInfo()
	{
		GameObject sceneBox = m_Detector.GetSceneBox();
		if (!sceneBox) return "空地";
		string sceneTag = sceneBox.tag;
		switch (sceneTag)
		{
			case "road":
				return "道路";
				break;
			case "building":
				return "建筑:" + sceneBox;
				break;
			default:
				break;
		}
		return "空地";
	}
}
