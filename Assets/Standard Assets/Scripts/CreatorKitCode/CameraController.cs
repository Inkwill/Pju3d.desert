using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace CreatorKitCodeInternal
{
	/// <summary>
	/// Control the camera, mainly used as a reference to the main camera through the singleton instance, and to handle
	/// mouse wheel zooming
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		public static CameraController Instance { get; set; }

		public Camera GameplayCamera;

		/// <summary>
		/// Angle in degree (down compared to horizon) the camera will look at when at the closest of the character
		/// </summary>
		public float MinAngle = 5.0f;
		/// <summary>
		/// Angle in degree (down compared to horizon) the camera will look at when at the farthest of the character
		/// </summary>
		public float MaxAngle = 45.0f;
		/// <summary>
		/// Distance at which the camera is from the character when at the closest zoom level
		/// </summary>
		public float MinDistance = 5.0f;
		/// <summary>
		/// Distance at which the camera is from the character when at the max zoom level
		/// </summary>
		public float MaxDistance = 45.0f;

		public CinemachineVirtualCamera Camera { get; protected set; }

		protected float m_CurrentDistance = 0;
		protected CinemachineFramingTransposer m_FramingTransposer;

		float m_setDistance;
		float m_currentSet;
		bool m_setTrigger = false;

		void Awake()
		{
			Instance = this;
			Camera = GetComponent<CinemachineVirtualCamera>();
			m_FramingTransposer = Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
		}

		private void FixedUpdate()
		{
			if (Mathf.Abs(m_setDistance - m_CurrentDistance) > 0.01f && m_setTrigger)
			{
				// 在 10 秒内从 m_CurrentDistance 插值到 m_setDistance
				float t = Mathf.PingPong(Time.time / 10f, 1f); // 控制插值的比例
				float interpolatedValue = m_CurrentDistance < m_setDistance ? 0.05f : -0.05f;
				//float interpolatedValue = m_CurrentDistance < m_setDistance ? Mathf.Lerp(m_CurrentDistance, m_setDistance, t) : (-1 * Mathf.Lerp(m_setDistance, m_CurrentDistance, t));
				Zoom(interpolatedValue);
			}
			else
			{
				m_setTrigger = false;
			}
		}
		void Start()
		{
			Zoom(0);
		}

		/// <summary>
		/// Zoom of the given distance. Note that distance need to be a param between 0...1,a d the distance is a ratio
		/// </summary>
		/// <param name="distance">The distance to zoom, need to be in range [0..1] (will be clamped) </param>
		public void Zoom(float distance)
		{
			m_CurrentDistance = Mathf.Clamp01(m_CurrentDistance + distance);

			Vector3 rotation = transform.rotation.eulerAngles;
			rotation.x = Mathf.LerpAngle(MinAngle, MaxAngle, m_CurrentDistance);
			transform.rotation = Quaternion.Euler(rotation);

			m_FramingTransposer.m_CameraDistance = Mathf.Lerp(MinDistance, MaxDistance, m_CurrentDistance);

			AmbiencePlayer.UpdateVolume(m_CurrentDistance);
		}
		public void CloseTo()
		{
			m_setDistance = 0;
			m_setTrigger = true;
		}

		public void Reset()
		{
			m_setDistance = m_currentSet;
			m_setTrigger = true;
		}
		public void SwitchModel()
		{
			switch (m_currentSet)
			{
				case 0:
					m_currentSet = 0.5f;
					break;
				case 0.5f:
					m_currentSet = 1.0f;
					break;
				case 1.0f:
					m_currentSet = 0.5f;
					break;
				default:
					break;
			}
			Reset();
		}
	}
}