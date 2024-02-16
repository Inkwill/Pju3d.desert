using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class StoryListener : MonoBehaviour
{
	public StoryNode testNode;
	public UnityEvent<StoryNode, string> nodeEvents;
	List<string> m_stories;
	List<StoryNode> m_completed;
	public StoryTeller CurrentTeller => m_currentTeller;
	StoryTeller m_currentTeller;

	void Start()
	{
		m_stories = new List<string>();
		m_completed = new List<StoryNode>();
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
		GameManager.GameUI.SwitchWindow("winTalk");
		m_currentTeller = teller;
	}

	public void StopListening(StoryTeller teller)
	{
		if (GameManager.GameUI.win_LastOpen.winName == "winTalk") GameManager.GameUI.BackWindow();
		if (m_currentTeller = teller) m_currentTeller = null;
	}
	List<StoryNode> GetCompleted(string storyName)
	{
		if (!m_stories.Contains(storyName)) return null;
		return m_completed.Where(node => node.StoryName == storyName) as List<StoryNode>;
	}
}
