using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHandle : MonoBehaviour
{
	DeviceItem m_device;
	Character m_character;
	GameObject m_deviceModle;
	UIInventoryWindow winInventory;
	public Action<DeviceItem> OnPlaced;

	public void SetDevice(DeviceItem device)
	{
		m_device = device;
		winInventory = GameManager.GameUI.GetWindow("winInventory") as UIInventoryWindow;
		m_character = GetComponent<Character>();
		if (m_character.BaseAI.SceneDetector && device.prefab)
		{
			m_deviceModle = Instantiate(device.modle, m_character.BaseAI.SceneDetector.transform, false);
			m_deviceModle.transform.localPosition = Vector3.zero;

		}
	}

	public void PlaceDevice()
	{
		GameManager.GameUI.WinMain.btConfirm.gameObject.SetActive(false);
		Destroy(m_deviceModle);
		Instantiate(m_device.prefab, m_character.BaseAI.SceneDetector.transform.position, m_character.BaseAI.SceneDetector.transform.rotation);
		OnPlaced?.Invoke(m_device);
	}

	public void CanclePlace()
	{
		GameManager.GameUI.WinMain.btConfirm.gameObject.SetActive(false);
		Destroy(m_deviceModle);
		OnPlaced?.Invoke(null);
	}
}
