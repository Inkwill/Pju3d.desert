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
		m_interactHandle = GetComponent<InteractHandle>();
		entrustments = new List<Entrustment>();
		AddEntrustment();
	}

	public void OnInteractEvent(GameObject actor, string eventName)
	{
		// var character = actor.GetComponent<CharacterData>();
		// if (eventName == "Completed" && character != null)
		// {
		// 	if (character.BaseAI.isIdle)
		// 	{
		// 		//if (m_interactHandle) m_interactHandle.CurrentTarget = actor;
		// 		GameManager.StoryListener.StartListening(this);
		// 		if (m_currentNode == null) m_currentNode = storyNodes.Where(story => GameManager.StoryListener.CanListen(story)).FirstOrDefault();
		// 		TellStory(GameManager.StoryListener.LastStoryAsk(m_currentNode));
		// 	}
		// }
		// if (eventName == "Exit")
		// {
		// 	GameManager.StoryListener.StopListening(this);
		// }
	}

	void OnListenEvent(string eventName, string content)
	{
		if (eventName == "listenerEvent_Ask")
		{
			TellStory(content);
		}
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
