using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "StoryNode", menuName = "Data/StoryNode", order = 11)]
public class StoryNode : ScriptableObject
{
	public string StoryName;
	public KeyValueData.KeyValue<string, string>[] Content;
	public KeyValueData.KeyValue<string[], string[]>[] Chats;
	public StoryNode Previous;
	public StoryNode Next;

	public bool IsCompletable(string contentValue)
	{
		return contentValue == Content[Content.Length - 1].Value;
	}

	public bool IsContent(string content)
	{
		foreach (var dialog in Content)
		{
			if (dialog.Key == content || dialog.Value == content) return true;
		}
		return false;
	}
	public string GetNextContent(string lastask)
	{
		if (IsCompletable(lastask)) return GetChat();
		else
		{
			int askIndex = GetIndexByAsk(lastask);
			if (askIndex != -1) return Content[askIndex + 1].Key;
			else return GetChat(lastask);
		}
	}

	public string[] GetListenerTalk(string curtell, string lastAsk)
	{
		var talkList = new List<string>();
		int tellIndex = GetIndexByTell(curtell);
		if (tellIndex != -1) talkList.Add(Content[tellIndex].Value);
		else if (lastAsk == "") talkList.Add(Content[0].Value);
		else if (!IsCompletable(lastAsk))
		{
			tellIndex = GetIndexByAsk(lastAsk);
			if (tellIndex != -1) talkList.Add(Content[tellIndex].Value);
		}

		foreach (var askChats in Chats)
		{
			int randomIndex = UnityEngine.Random.Range(0, askChats.Key.Length);
			talkList.Add(askChats.Key[randomIndex]);
		}
		Helpers.ListRandom(talkList);
		string[] contents = new string[2];
		for (int i = 0; i < contents.Length; i++)
		{
			contents[i] = talkList[i];
		}
		return contents;
	}

	int GetIndexByAsk(string ask)
	{
		var dialog = Content.Where(dlog => dlog.Value == ask).FirstOrDefault();
		if (dialog == null) return -1;
		int index = Content.Select((value, idx) => new { content = value, Index = idx })
						.First(dlog => dlog.content.Value == ask)
						.Index;
		return index;
	}

	int GetIndexByTell(string tell)
	{
		var dialog = Content.Where(dlog => dlog.Key == tell).FirstOrDefault();
		if (dialog == null) return -1;
		int index = Content.Select((value, idx) => new { content = value, Index = idx })
						.First(dlog => dlog.content.Key == tell)
						.Index;
		return index;
	}

	string GetChat(string ask)
	{
		var chat = Chats.Where(c => c.Key.Contains(ask)).FirstOrDefault();
		if (chat != null)
		{
			int randIndex = UnityEngine.Random.Range(0, chat.Value.Length);
			return chat.Value[randIndex];
		}
		return "Err: get a noexist chat ask: " + ask;
	}

	string GetChat()
	{
		int chatIndex = UnityEngine.Random.Range(0, Chats.Length);
		int contentIndex = UnityEngine.Random.Range(0, Chats[chatIndex].Value.Length);
		return Chats[chatIndex].Value[contentIndex];
	}
}
