using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StoryNode", menuName = "Data/StoryNode", order = 11)]
public class StoryNode : ScriptableObject
{
	public string StoryName;
	public KeyValueData.KeyValue<string, string>[] Content;
	public KeyValueData.KeyValue<string[], string[]>[] Chats;
	public StoryNode Previous;
	public StoryNode Next;
	List<string> m_listenerTalkList;

	public bool IsCompletable(StoryListener listener)
	{
		return true;
	}

	public string[] GetListenerTalk()
	{
		m_listenerTalkList = new List<string>();
		m_listenerTalkList.Add(Content[0].Value);
		foreach (var chat in Chats)
		{
			m_listenerTalkList.Add(chat.Key[0]);
		}
		Helpers.ListRandom(m_listenerTalkList);
		string[] contents = new string[2];
		for (int i = 0; i < contents.Length; i++)
		{
			contents[i] = m_listenerTalkList[i];
		}
		return contents;
	}

}
