using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;

public class StoryTeller : MonoBehaviour
{
	public List<StoryNode> storyNodes;
	public UnityEvent<StoryNode, string> tellerEvent;
	public StoryNode CurrentNode { get { return m_currentNode; } set { m_currentNode = value; tellerEvent?.Invoke(m_currentNode, "tellStory"); } }
	StoryNode m_currentNode;
	InteractHandle m_interactHandle;


	void Start()
	{
		GameManager.StoryListener.nodeEvents.AddListener(OnListenerEvent);
		RoleControl role = GetComponent<RoleControl>();
		role?.eventSender.events.AddListener(OnRoleEvent);
		m_interactHandle = GetComponent<InteractHandle>();
		m_interactHandle?.InteractEvent.AddListener(OnInteractEvent);
	}

	public void OnInteractEvent(RoleControl actor, string eventName)
	{
		if (eventName == "Completed")
		{
			if (actor.isIdle)
			{
				if (m_interactHandle) m_interactHandle.CurrentTarget = actor;
				GameManager.StoryListener.StartListening(this);
			}
		}
		if (eventName == "Exit")
		{
			GameManager.StoryListener.StopListening(this);
		}
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
