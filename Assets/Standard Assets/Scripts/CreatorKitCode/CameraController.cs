using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
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
		public enum Mode
		{
			RPG,
			BUILD,
			STORY
		}

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
		Mode m_lastMode;
		Mode m_curMode;
		float m_setDistance;
		bool m_setTrigger;

		void Awake()
		{
			Instance = this;
			Camera = GetComponent<CinemachineVirtualCamera>();
			m_FramingTransposer = Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
		}

		void Start()
		{
			Zoom(0f);
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
				OnSetOver();
			}
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

		public void ResetMode()
		{
			SetMode(m_lastMode);

		}
		public void SetMode(Mode mode)
		{
			if (m_setTrigger) return;
			m_lastMode = m_curMode;
			m_curMode = mode;
			switch (mode)
			{
				case Mode.RPG:
					m_setDistance = 0.3f;
					break;
				case Mode.BUILD:
					m_setDistance = 1.0f;
					break;
				case Mode.STORY:
					m_setDistance = 0f;
					break;
				default:
					break;
			}
			m_setTrigger = true;
		}

		void OnSetOver()
		{
			var cine = GameManager.Instance.VCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
			switch (m_curMode)
			{
				case Mode.RPG:
					GameManager.Instance.VCamera.Follow = GameManager.Instance.VCamera.LookAt = GameManager.Player.transform;
					if (cine)
					{
						cine.m_XDamping = 0f;
						cine.m_YDamping = 0f;
						cine.m_ZDamping = 0.5f;
					}

					break;
				case Mode.BUILD:
					GameManager.Instance.VCamera.Follow = GameManager.Instance.VCamera.LookAt = GameManager.Instance.buildModeFollow;
					if (cine)
					{
						cine.m_XDamping = cine.m_YDamping = cine.m_ZDamping = 0.5f;
					}
					break;
				case Mode.STORY:
					break;
				default:
					break;
			}
			m_setTrigger = false;
		}
	}
}