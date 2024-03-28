using Random = System.Random;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// Helper class containing diverse functions that avoid redoing common things.
/// </summary>
public class Helpers
{
	public static int WrapAngle(int angle)
	{
		while (angle < 0)
			angle += 360;

		while (angle > 360)
			angle -= 360;

		return angle;
	}

	public static void RecursiveLayerChange(Transform root, int layer)
	{
		root.gameObject.layer = layer;

		foreach (Transform t in root)
			RecursiveLayerChange(t, layer);
	}
	public static string DictionaryToString(Dictionary<string, int> dictionary)
	{
		// 使用 StringBuilder 构建字符串
		StringBuilder result = new StringBuilder();

		foreach (var kvp in dictionary)
		{
			// 将键值对拼接为字符串
			result.Append($"{kvp.Key}: {kvp.Value}, ");
		}

		// 移除末尾多余的逗号和空格
		if (result.Length > 2)
		{
			result.Length -= 2;
		}

		return result.ToString();
	}

	public static void Log(object logger, string key = "", string log = "")
	{
		if (KeyValueData.GetValue<bool>(GameManager.Config.LogConfig, key))
		{
			string content = $"<color=green><{key}></color>-<color=white>{log}</color><color=grey>[{logger.ToString()}]</color>";
			Debug.Log(content);
			UIRpgWindow win = GameManager.GameUI.GetWindow<UIRpgWindow>("winRpg");
			win.chatInfo.text = content;
		}
	}

	public static void LogError(object logger, string key = "", string log = "")
	{
		string content = $"<color=red><{key}></color>-<color=white>{log}</color><color=grey>[{logger.ToString()}]</color>";
		Debug.LogError(content);
	}

	public static void ShowUIElement(GameObject element, bool value)
	{
		Animator anim = element.GetComponent<Animator>();
		if (anim)
		{
			bool hided = anim.GetCurrentAnimatorStateInfo(0).IsName("hide");
			if (value && hided) anim.SetTrigger("show");
			else if (!hided) anim.SetTrigger("hide");
		}
		else
		{
			element.SetActive(value);
		}
	}

	public static void ListRandom<T>(List<T> sources)
	{
		Random rd = new Random();
		int index = 0;
		T temp;
		for (int i = 0; i < sources.Count; i++)
		{
			index = rd.Next(0, sources.Count - 1);
			if (index != i)
			{
				temp = sources[i];
				sources[i] = sources[index];
				sources[index] = temp;
			}
		}
	}

	public static T[] AdjustElements<T>(Transform root, int Length, GameObject element)
	{
		T[] result = root.GetComponentsInChildren<T>();
		if (result.Length >= Length) return result.Take(Length).ToArray();
		else
		{
			T[] concat = new T[Length - result.Length];
			for (int i = 0; i < concat.Length; i++)
			{
				concat[i] = Object.Instantiate(element, root).GetComponent<T>();
			}
			return result.Concat(concat).ToArray();
		}
	}
}
