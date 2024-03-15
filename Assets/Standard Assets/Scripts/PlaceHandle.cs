using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHandle : MonoBehaviour
{
	DeviceItem m_device;
	Character m_character;
	GameObject m_deviceModle;

	public void SetDevice(DeviceItem device)
	{
		m_device = device;
		m_character = GetComponent<Character>();
		if (m_character.BaseAI.SceneDetector && device.prefab)
		{
			m_deviceModle = Instantiate(device.modle, m_character.BaseAI.SceneDetector.transform, false);
			m_deviceModle.transform.localPosition = Vector3.zero;
			m_character.GetComponentInChildren<UIRoleHud>()?.ShowPlaceButton();
		}
	}

	public void PlaceDevice()
	{
		Destroy(m_deviceModle);
		Instantiate(m_device.prefab, m_character.BaseAI.SceneDetector.transform.position, m_character.BaseAI.SceneDetector.transform.rotation);
	}
}
