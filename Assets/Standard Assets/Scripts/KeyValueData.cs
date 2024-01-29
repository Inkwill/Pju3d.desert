using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

[CreateAssetMenu(fileName = "DemoData", menuName = "Data/KeyValueData", order = 1)]
public class KeyValueData : ScriptableObject
{
	[Serializable]
	public class KeyValue<TKey, TValue>
	{
		public TKey Key;
		public TValue Value;
	}

	public List<KeyValue<string, AudioClip>> AudioDic;
	public List<KeyValue<string, Item>> Item;
	public static T GetValue<T>(List<KeyValue<string, T>> dic, string key)
	{
		foreach (KeyValue<string, T> data in dic)
		{
			if (data.Key == key) return data.Value;
		}
		return default(T);
	}
}
