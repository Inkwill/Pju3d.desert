using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using TMPro;
using Unity.VisualScripting;

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
	public List<KeyValue<string, EffectData>> Effect;
	public static T GetValue<T>(List<KeyValue<string, T>> dic, string key)
	{
		foreach (KeyValue<string, T> data in dic)
		{
			if (data.Key == key) return data.Value;
		}
		return default(T);
	}

	public static Dictionary<string, int> ToDic(List<KeyValue<Item, int>> data)
	{
		Dictionary<string, int> result = new Dictionary<string, int>();
		foreach (var da in data)
		{
			result[da.Key.ItemName] = da.Value;
		}
		return result;
	}

}
