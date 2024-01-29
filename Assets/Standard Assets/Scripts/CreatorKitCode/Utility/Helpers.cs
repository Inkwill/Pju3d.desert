using UnityEngine;
using System.Collections.Generic;
using System.Text;
namespace CreatorKitCode
{
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
	}
}