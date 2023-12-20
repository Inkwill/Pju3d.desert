using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCodeInternal;
using UnityEngine.AI;

public class DigTool : MonoBehaviour
{
	public Pit pitObj;

	public CharacterControl m_character;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Pit pit = Instantiate(pitObj, transform.position, Quaternion.Euler(0, 180, 0)) as Pit;
			pit.GetComponent<EventSender>()?.m_event.AddListener(OnPitCompleted);
			m_character.PlayWork(true);
		}
	}

	void OnPitCompleted(GameObject sender, string eventMessage)
	{
		if (eventMessage == "event_pit_completed")
		{
			m_character.PlayWork(false);
		}
	}
}
