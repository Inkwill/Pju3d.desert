using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StoryNode", menuName = "Data/StoryNode", order = 11)]
public class StoryNode : ScriptableObject
{
	public string StoryName;
	public string Content;
	public StoryNode Previous;
	public StoryNode Next;

	public bool IsCompletable(StoryListener listener)
	{
		return true;
	}
}
