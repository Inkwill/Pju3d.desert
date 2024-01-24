using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;

public class UITargetInfo : MonoBehaviour
{
	public void Init(CharacterControl character)
	{
		character.GetComponent<EventSender>()?.events.AddListener(OnPlayerEvent);
	}

	void OnPlayerEvent(GameObject obj, string eventName)
	{
		// if (eventName == "onTarget")
		// {
		// 	monsterHud.Show(m_player.CurrentTarget);
		// }
	}
}
