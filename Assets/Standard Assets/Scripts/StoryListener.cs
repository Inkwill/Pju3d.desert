using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class StoryListener : MonoBehaviour
{
	public StoryNode testNode;
	public UnityEvent<string, string> storyListenerEvents;
	List<string> m_stories;
	Dictionary<StoryNode, string> m_storyNodes;
	Dictionary<string, int> m_FriendlyValue;
	public StoryTeller CurrentTeller => m_currentTeller;
	StoryTeller m_currentTeller;

	void Start()
	{
		m_stories = new List<string>();
		m_storyNodes = new Dictionary<StoryNode, string>();
		m_FriendlyValue = new Dictionary<string, int>();
		AddStory("main");
	}
	public void AddStory(string storyName)
	{
		if (!m_stories.Contains(storyName))
		{
			m_stories.Add(storyName);
			storyListenerEvents?.Invoke(null, storyName);
		}
	}
	public void Ask(string content)
	{
		StoryNode node = CurrentTeller.CurrentNode;
		if (node.IsContent(content))
		{
			if (m_storyNodes.ContainsKey(node)) m_storyNodes[node] = content;
			else m_storyNodes.Add(node, content);
		}
		storyListenerEvents?.Invoke("listenerEvent_Ask", content);
	}
	// public void CompletedNode(StoryNode node)
	// {
	// 	if (!m_storyNodes.ContainsKey(node))
	// 	{
	// 		m_storyNodes.Add(node, node.Content[0].Key);
	// 		storyListenerEvents?.Invoke(node, "storyEvent_Complete");
	// 	}
	// }

	public bool CanListen(StoryNode node)
	{
		if (!m_stories.Contains(node.StoryName)) return false;
		if (!m_storyNodes.ContainsKey(node))
		{
			return node.Previous == null || m_storyNodes.ContainsKey(node.Previous);
		}
		else if (node.IsCompletable(m_storyNodes[node])) return false;
		return true;
	}

	public void StartListening(StoryTeller teller)
	{
		m_currentTeller = teller;
		storyListenerEvents?.Invoke("listenerEvent_Start", "");
	}

	public void StopListening(StoryTeller teller)
	{
		if (m_currentTeller = teller)
		{
			storyListenerEvents?.Invoke("listenerEvent_Stop", "");
			m_currentTeller = null;
		}
	}
	public string LastStoryAsk(StoryNode node)
	{
		if (m_storyNodes.ContainsKey(node)) return m_storyNodes[node];
		else
		{
			m_storyNodes.Add(node, "");
			return "";
		}
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
