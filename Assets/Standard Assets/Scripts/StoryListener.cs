using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class StoryListener : MonoBehaviour
{
	public StoryNode testNode;
	public UnityEvent<StoryNode, string> nodeEvents;
	public UnityEvent<string> ListenerEvents;
	List<string> m_stories;
	List<StoryNode> m_completed;
	Dictionary<string, int> m_FriendlyValue;
	public StoryTeller CurrentTeller => m_currentTeller;
	StoryTeller m_currentTeller;

	void Start()
	{
		m_stories = new List<string>();
		m_completed = new List<StoryNode>();
		m_FriendlyValue = new Dictionary<string, int>();
		AddStory("main");
	}
	public void AddStory(string storyName)
	{
		if (!m_stories.Contains(storyName))
		{
			m_stories.Add(storyName);
			nodeEvents?.Invoke(null, storyName);
		}
	}

	public void CompletedNode(StoryNode node)
	{
		if (!m_completed.Contains(node))
		{
			m_completed.Add(node);
			nodeEvents?.Invoke(node, "complete");
		}
	}

	public bool IsListening(StoryNode node)
	{
		if (!m_stories.Contains(node.StoryName)) return false;
		if (m_completed.Contains(node)) return false;
		return node.Previous == null || m_completed.Contains(node.Previous);
	}

	public void StartListening(StoryTeller teller)
	{
		m_currentTeller = teller;
		ListenerEvents?.Invoke("StartListening");
	}

	public void StopListening(StoryTeller teller)
	{
		if (m_currentTeller = teller)
		{
			m_currentTeller = null;
			ListenerEvents?.Invoke("StopListening");
		}
	}
	List<StoryNode> GetCompleted(string storyName)
	{
		if (!m_stories.Contains(storyName)) return null;
		return m_completed.Where(node => node.StoryName == storyName) as List<StoryNode>;
	}

	public int GetFriendlyValue()
	{
		return GetFriendlyValue(CurrentTeller.RoleName);

	}
	public int GetFriendlyValue(string roleName)
	{
		var data = m_FriendlyValue.Where(kv => kv.Key == roleName).FirstOrDefault();
		return data.Value;
	}

	public void AddFriendlyValue(string roleName, int value)
	{
		if (m_FriendlyValue.ContainsKey(roleName)) m_FriendlyValue[roleName] += value;
		else m_FriendlyValue.Add(roleName, value);
	}
}
