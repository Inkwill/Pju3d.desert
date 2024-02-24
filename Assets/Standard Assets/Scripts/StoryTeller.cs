using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CreatorKitCode;
using System.Linq;

public class StoryTeller : MonoBehaviour
{
	public List<StoryNode> storyNodes;
	public UnityEvent<StoryTeller, string> tellerEvent;
	public StoryNode CurrentNode { get { return m_currentNode; } }
	public string RoleName { get { return m_character.CharacterName; } }
	public List<Entrustment> entrustments;
	StoryNode m_currentNode;
	InteractHandle m_interactHandle;
	CharacterData m_character;


	void Start()
	{
		GameManager.StoryListener.storyListenerEvents.AddListener(OnListenEvent);
		m_character = GetComponent<CharacterData>();
		m_character.GetComponent<EventSender>()?.events.AddListener(OnRoleEvent);
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
				if (m_currentNode == null) m_currentNode = storyNodes.Where(story => GameManager.StoryListener.CanListen(story)).FirstOrDefault();
				TellStory(GameManager.StoryListener.LastStoryAsk(m_currentNode));
			}
		}
		if (eventName == "Exit")
		{
			GameManager.StoryListener.StopListening(this);
		}
	}

	void OnListenEvent(string eventName, string content)
	{
		if (eventName == "listenerEvent_Ask")
		{
			TellStory(content);
		}
	}

	void OnRoleEvent(GameObject role, string eventName)
	{
		// if (eventName == "aiEvent_wandering" && m_currentNode == null)
		// 	TellStory();
	}

	void TellStory(string ask)
	{
		if (ask == "") tellerEvent?.Invoke(this, CurrentNode.Content[0].Key);
		else tellerEvent?.Invoke(this, CurrentNode.GetNextContent(ask));
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

	public bool GiveItem(Item item)
	{
		m_character.Inventory.AddItem(item);
		return true;
	}
}
