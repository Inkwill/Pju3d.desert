using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CreatorKitCode
{
	/// <summary>
	/// Base clase of all items in the game. This is an abstract class and need to be inherited to specify behaviour.
	/// The project offer 3 type of items : UsableItem, Equipment and Weapon
	/// </summary>
	public abstract class Item : ScriptableObject
	{
		public string ItemName;
		public int value = 1;
		public bool onlyOne;
		public int stackNum;
		public Sprite ItemSprite;
		public string Description;
		public GameObject WorldObjectPrefab;
		public AudioClip SpawnClip;

		/// <summary>
		/// Called by the inventory system when the object is "used" (double clicked)
		/// </summary>
		/// <param name="user">The CharacterDate that used that item</param>
		/// <returns>If it was actually used (allow the inventory to know if it can remove the object or not)</returns>
		public virtual bool UsedBy(Character user, int count = 1)
		{
			return false;
		}

		public virtual string GetDescription()
		{
			return Description;
		}
	}


#if UNITY_EDITOR
	public class ItemEditor
	{
		SerializedObject m_Target;

		SerializedProperty m_NameProperty;
		SerializedProperty m_IconProperty;
		SerializedProperty m_DescriptionProperty;
		SerializedProperty m_WorldObjectPrefabProperty;
		SerializedProperty m_SpawnClip;

		public void Init(SerializedObject target)
		{
			m_Target = target;

			m_NameProperty = m_Target.FindProperty(nameof(Item.ItemName));
			m_IconProperty = m_Target.FindProperty(nameof(Item.ItemSprite));
			m_DescriptionProperty = m_Target.FindProperty(nameof(Item.Description));
			m_WorldObjectPrefabProperty = m_Target.FindProperty(nameof(Item.WorldObjectPrefab));
			m_SpawnClip = m_Target.FindProperty(nameof(Item.SpawnClip));
		}

		public void GUI()
		{
			EditorGUILayout.PropertyField(m_IconProperty);
			EditorGUILayout.PropertyField(m_NameProperty);
			EditorGUILayout.PropertyField(m_DescriptionProperty, GUILayout.MinHeight(128));
			EditorGUILayout.PropertyField(m_WorldObjectPrefabProperty);
			EditorGUILayout.PropertyField(m_SpawnClip);
		}
	}
#endif
}