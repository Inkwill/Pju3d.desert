using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;

/// <summary>
/// Base class for interactable object, inherit from this class and override InteractWith to handle what happen when
/// the player interact with the object.
/// </summary>
public class InteractHandle : HighlightableObject
{
    public LayerMask layers;
    public InteractOnTrigger SceneDetector;
    public InteractOnTrigger InteractDetector;
    public GameObject CurrentTarget => m_currentTarget;
    GameObject m_currentTarget;
    public UnityEvent<Loot> OnItemEnter, OnItemExit;
    public UnityEvent<GameObject> OnTargetStay;
    public UnityEvent<InteractHandle, string> OnInteracting;
    public bool Interacting => m_interacting;
    bool m_interacting;

    public string SceneBox { get { return SceneBoxInfo(SceneDetector.lastInner, false); } }
    public string SceneBoxName { get { return SceneBoxInfo(SceneDetector.lastInner, true); } }

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (InteractDetector)
        {
            InteractDetector.OnEnter.AddListener(OnInteracterEnter);
            InteractDetector.OnExit.AddListener(OnInteracterExit);
            InteractDetector.OnStay.AddListener(OnInteracterStay);
            InteractDetector.layers = layers;
        }
        if (SceneDetector) SceneDetector.layers = LayerMask.GetMask("Scene");
    }
    void OnInteracterEnter(GameObject enter)
    {
        if (enter.tag == "item") OnItemEnter?.Invoke(enter.GetComponentInParent<Loot>());
        InteractHandle interactor = enter.GetComponent<InteractHandle>();
        if (interactor) OnInteracting?.Invoke(interactor, "Enter");
    }

    void OnInteracterStay(GameObject stayer)
    {
        if (m_currentTarget == null)
        {
            m_currentTarget = stayer;
            m_currentTarget.GetComponent<EventSender>()?.events.AddListener(OnInteracterEvent);
        }

        InteractHandle interactor = m_currentTarget.GetComponent<InteractHandle>();
        if (m_currentTarget == stayer && !m_interacting)
        {
            if (interactor != null && interactor.CurrentTarget == gameObject)
            {
                m_interacting = true;
                OnInteracting?.Invoke(interactor, "Start");
                return;
            }
        }

        if (m_interacting) OnInteracting?.Invoke(interactor, "Interacting");
        else OnTargetStay?.Invoke(stayer);

    }

    void OnInteracterExit(GameObject exiter)
    {
        if (exiter.tag == "item") OnItemExit?.Invoke(exiter.GetComponentInParent<Loot>());
        if (m_currentTarget == exiter)
        {
            m_currentTarget.GetComponent<EventSender>()?.events.RemoveListener(OnInteracterEvent);
            OnInteracting?.Invoke(m_currentTarget.GetComponent<InteractHandle>(), "Stop");
            m_currentTarget = null;
            m_interacting = false;
        }
    }

    void OnInteracterEvent(GameObject sender, string eventMessage)
    {
        // if (eventMessage == "roleEvent_OnState_DEAD" && m_currentTarget == sender)
        // {
        //     //m_currentTarget = null;
        // }
        // if (eventMessage == "roleEvent_OnState_IDLE" && m_interactTarget == null)
        // {
        //     m_interactTarget = sender;
        //     GetComponentInChildren<UIRoleHud>()?.Bubble(sender.GetComponent<RoleControl>()?.Data.CharacterName);
        //     //LookAt(sender.transform);
        // }
        //Debug.Log("OnInteracterEvent: InteracterTarget= " + sender + "event= " + eventMessage);
    }
    public static string SceneBoxInfo(GameObject sceneBox, bool display)
    {
        if (!sceneBox) return display ? "空地" : "blank";
        string sceneTag = sceneBox.tag;
        switch (sceneTag)
        {
            case "road":
                return display ? "道路" : sceneTag;
            case "building":
                return display ? "建筑:" + sceneBox : sceneTag;
            case "pit":
                return display ? "坑:" + sceneBox : sceneTag;
            case "mount":
                return display ? "山体:" : sceneTag;
            case "npc":
                return display ? "npc:" + sceneBox : sceneTag;
            case "creater":
                return display ? "建造中..." : sceneTag;
            default:
                break;
        }
        return display ? "空地" : "blank";
    }
}
