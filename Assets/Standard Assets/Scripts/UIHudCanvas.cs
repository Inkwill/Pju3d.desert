using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHudCanvas : MonoBehaviour
{
	public static UIHudCanvas Instance { get; private set; }
	Canvas m_Canvas;
	Camera m_MainCamera;
	public Button button;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		m_Canvas = GetComponent<Canvas>();
		m_MainCamera = Camera.main;
	}
}
