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
	public string RoleName { get { return m_role.Data.CharacterName; } }
	public List<Entrustment> entrustments;
	StoryNode m_currentNode;
	InteractHandle m_interactHandle;
	RoleControl m_role;


	void Start()
	{
		GameManager.StoryListener.nodeEvents.AddListener(OnListenerEvent);
		m_role = GetComponent<RoleControl>();
		m_role?.eventSender.events.AddListener(OnRoleEvent);
		m_interactHandle = GetComponent<InteractHandle>();
		m_interactHandle?.InteractEvent.AddListener(OnInteractEvent);
		entrustments = new List<Entrustment>();
		AddEntrustment();
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

	void AddEntrustment()
	{
		for (int i = 0; i < 5; i++)
		{
			Entrustment entrust = new Entrustment(this);
			entrustments.Add(entrust);
		}
		entrustments.Sort();
	}
}
