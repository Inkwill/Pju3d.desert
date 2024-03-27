using System;
using Cinemachine;
using UnityEngine;


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
		STORY,
		SLG,
		TD
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
	public CinemachineVirtualCamera VCamera { get; protected set; }
	public Transform buildModeFollow;
	protected float m_CurrentDistance;
	protected CinemachineFramingTransposer m_FramingTransposer;

	public static GameObject CurrentSlected
	{
		get { return m_CurrentSelected; }
		set
		{
			m_CurrentSelected?.GetComponent<HighlightableObject>()?.Dehighlight();
			m_CurrentSelected = value;
			m_CurrentSelected?.GetComponent<HighlightableObject>()?.Highlight();
			Helpers.Log(Instance, "PickObject", "target= " + m_CurrentSelected.name);
		}
	}
	static GameObject m_CurrentSelected;
	Mode m_lastMode;
	public Mode CurMode { get { return m_curMode; } }
	Mode m_curMode;
	float m_setDistance;
	int m_switchSpeed = 1;
	bool m_setTrigger;
	Action m_handleAction;
	Vector3 m_dragMousePos = Vector3.zero;
	Vector3 m_revisCampos = Vector3.zero;

	void Awake()
	{
		Instance = this;
		VCamera = GetComponent<CinemachineVirtualCamera>();
		m_FramingTransposer = VCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
	}

	void Start()
	{
		Zoom(0f);
		buildModeFollow.position = GameManager.CurHero.transform.position;
	}

	private void FixedUpdate()
	{
		if (Mathf.Abs(m_setDistance - m_CurrentDistance) > 0.01f && m_setTrigger)
		{
			// 在 10 秒内从 m_CurrentDistance 插值到 m_setDistance
			//float t = Mathf.PingPong(Time.time / 10f, 1f); // 控制插值的比例
			float interpolatedValue = m_CurrentDistance < m_setDistance ? m_switchSpeed * 0.01f : -0.01f * m_switchSpeed;
			//float interpolatedValue = m_CurrentDistance < m_setDistance ? Mathf.Lerp(m_CurrentDistance, m_setDistance, t) : (-1 * Mathf.Lerp(m_setDistance, m_CurrentDistance, t));
			Zoom(interpolatedValue);
		}
		else
		{
			OnSetOver();
		}

		if (m_curMode == Mode.BUILD || m_curMode == Mode.SLG)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_dragMousePos = Input.mousePosition;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				// 如果射线与物体碰撞
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(new string[] { "CurHero", "Enemy", "Neutral", "Interactable" })))
				{
					CurrentSlected = hit.collider.gameObject;
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				m_dragMousePos = Vector3.zero;
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 direction = Input.mousePosition - m_dragMousePos;
				if (direction.magnitude > 0)
				{
					Vector3 transPos = buildModeFollow.transform.position - new Vector3(direction.x, 0, direction.y) * 1.0f * Time.deltaTime;
					if (!GameManager.GameLevel.CurLevel.Data.InBaseBoundary(transPos)) transPos = buildModeFollow.transform.position - new Vector3(direction.x, 0, direction.y) * 0.1f * Time.deltaTime;
					buildModeFollow.transform.position = transPos;
					m_dragMousePos = Input.mousePosition;
					//Helpers.Log(this, "MouseDrag", $"from{m_dragMousePos}To{Input.mousePosition}->{direction.magnitude}");
				}
			}
			else if (!GameManager.GameLevel.CurLevel.Data.InBaseBoundary(buildModeFollow.transform.position))
			{
				buildModeFollow.transform.position = Vector3.Lerp(buildModeFollow.transform.position, GameManager.GameLevel.CurLevel.Data.ConstraintPos(buildModeFollow.transform.position), 3.0f);
				if (GameManager.GameLevel.CurLevel.Data.InBaseBoundary(buildModeFollow.transform.position)) m_revisCampos = Vector3.zero;
			}

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
	public void SetMode(Mode mode, Action action = null)
	{
		if (m_setTrigger) return;
		m_lastMode = m_curMode;
		m_curMode = mode;
		buildModeFollow.position = GameManager.CurHero.transform.position;
		switch (mode)
		{
			case Mode.RPG:
				m_setDistance = 0.4f;
				m_switchSpeed = 2;
				GameManager.GameUI.JoyStick.enabled = true;
				GameManager.CurHero.BaseAI.SetState(AIBase.State.IDLE);
				break;
			case Mode.TD:
				m_setDistance = 0.6f;
				m_switchSpeed = 2;
				GameManager.GameUI.JoyStick.enabled = true;
				GameManager.CurHero.BaseAI.SetState(AIBase.State.IDLE);
				break;
			case Mode.BUILD:
				m_setDistance = 1.0f;
				GameManager.GameUI.JoyStick.enabled = false;
				GameManager.CurHero.BaseAI.SetState(AIBase.State.INACTIVE);
				break;
			case Mode.STORY:
				m_setDistance = 0f;
				m_switchSpeed = 1;
				GameManager.GameUI.JoyStick.enabled = false;
				GameManager.CurHero.BaseAI.SetState(AIBase.State.INACTIVE);
				break;
			case Mode.SLG:
				m_setDistance = 0.8f;
				m_switchSpeed = 5;
				GameManager.GameUI.JoyStick.enabled = false;
				GameManager.CurHero.BaseAI.SetState(AIBase.State.INACTIVE);
				break;
			default:
				break;
		}
		m_setTrigger = true;
		if (action != null) m_handleAction = action;
	}

	void OnSetOver()
	{
		var cine = GameManager.Instance.VCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
		switch (m_curMode)
		{
			case Mode.RPG:
				GameManager.Instance.VCamera.Follow = GameManager.Instance.VCamera.LookAt = GameManager.CurHero.transform;
				if (cine)
				{
					cine.m_XDamping = 0f;
					cine.m_YDamping = 0f;
					cine.m_ZDamping = 0.5f;
				}

				break;
			case Mode.BUILD:
			case Mode.SLG:
				GameManager.Instance.VCamera.Follow = GameManager.Instance.VCamera.LookAt = buildModeFollow;
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
		m_handleAction?.Invoke();
		m_handleAction = null;
	}
}
