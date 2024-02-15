using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryTeller : MonoBehaviour
{
    public List<StoryNode> storyNodes;
    public UnityEvent<StoryNode, string> tellerEvent;
    public StoryNode CurrentNode { get { return m_currentNode; } set { m_currentNode = value; tellerEvent?.Invoke(m_currentNode, "tellStory"); } }
    StoryNode m_currentNode;
    void Start()
    {
        GameManager.StoryListener.nodeEvents.AddListener(OnListenerEvent);
        GetComponent<InteractHandle>()?.OnInteracting.AddListener(OnInteracting);
        GetComponent<RoleControl>()?.eventSender.events.AddListener(OnRoleEvent);
    }

    void OnInteracting(InteractHandle interactor, string eventName)
    {
        //TellStory();
    }

    void OnListenerEvent(StoryNode node, string eventName)
    {
        if (m_currentNode == null && node == null || m_currentNode == node)
            TellStory();
    }

    void OnRoleEvent(GameObject role, string eventName)
    {
        if (eventName == "aiEvent_wandering" && m_currentNode == null)
            TellStory();
    }

    void TellStory()
    {
        foreach (var node in storyNodes)
        {
            if (GameManager.StoryListener.IsListening(node))
            {
                CurrentNode = node;
                return;
            }
        }
        CurrentNode = null;
    }
}
