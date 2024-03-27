using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Special Item than can be equipped. They can have a minimum stats value needed to equip them, and you can add
/// EquippedEffect which will be executed when the object is equipped and unequipped, allowing to code special
/// behaviour when the player equipped those object, like raising stats.
/// </summary>
[CreateAssetMenu(fileName = "EquipmentItem", menuName = "Data/Equipment Item", order = -999)]
public class EquipmentItem : Item
{
	public enum EquipmentSlot
	{
		Head,
		Torso,
		Legs,
		Feet,
		Accessory,
		Weapon,
		ViceWeapon
	}

	public EquipmentSlot Slot;
	public StatSystem.StatModifier EquippedStat;

	public override string GetDescription()
	{
		string desc = base.GetDescription();
		desc += StatsDescription();
		// foreach (var effect in EquippedEffects)
		// 	desc += "\n" + effect.GetDescription();

		// bool requireStrength = MinimumStrength > 0;
		// bool requireDefense = MinimumDefense > 0;
		// bool requireAgility = MinimumAgility > 0;

		// if (requireStrength || requireAgility || requireDefense)
		// {
		// 	desc += "\nRequire : \n";

		// 	if (requireStrength)
		// 		desc += $"Strength : {MinimumStrength}";

		// 	if (requireAgility)
		// 	{
		// 		if (requireStrength) desc += " & ";
		// 		desc += $"Defense : {MinimumDefense}";
		// 	}

		// 	if (requireDefense)
		// 	{
		// 		if (requireStrength || requireAgility) desc += " & ";
		// 		desc += $"Agility : {MinimumAgility}";
		// 	}
		// }
		return desc;
	}
	public void EquippedBy(Character user)
	{
		user.Stats.AddModifier(EquippedStat);
	}

	public void UnequippedBy(Character user)
	{
		user.Stats.RemoveModifier(EquippedStat);
	}

	string StatsDescription()
	{
		string desc = "\n";
		string unit = EquippedStat.ModifierMode == StatSystem.StatModifier.Mode.Percentage ? "%" : "";

		if (EquippedStat.Stats.strength != 0)
			desc += $"Str : {EquippedStat.Stats.strength:+0;-#}{unit}\n"; //format specifier to force the + sign to appear
		if (EquippedStat.Stats.agility != 0)
			desc += $"Agi : {EquippedStat.Stats.agility:+0;-#}{unit}\n";
		if (EquippedStat.Stats.defense != 0)
			desc += $"Def : {EquippedStat.Stats.defense:+0;-#}{unit}\n";
		if (EquippedStat.Stats.health != 0)
			desc += $"HP : {EquippedStat.Stats.health:+0;-#}{unit}\n";

		return desc;
	}
}


// #if UNITY_EDITOR
// [CustomEditor(typeof(EquipmentItem))]
// public class EquipmentItemEditor : Editor
// {
// 	EquipmentItem m_Target;

// 	ItemEditor m_ItemEditor;

// 	List<string> m_AvailableEquipEffectType;
// 	SerializedProperty m_EquippedEffectListProperty;

// 	SerializedProperty m_SlotProperty;

// 	SerializedProperty m_MinimumStrengthProperty;
// 	SerializedProperty m_MinimumAgilityProperty;
// 	SerializedProperty m_MinimumDefenseProperty;

// 	void OnEnable()
// 	{
// 		m_Target = target as EquipmentItem;
// 		m_EquippedEffectListProperty = serializedObject.FindProperty(nameof(EquipmentItem.EquippedEffects));

// 		m_SlotProperty = serializedObject.FindProperty(nameof(EquipmentItem.Slot));

// 		m_MinimumStrengthProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumStrength));
// 		m_MinimumAgilityProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumAgility));
// 		m_MinimumDefenseProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumDefense));

// 		m_ItemEditor = new ItemEditor();
// 		m_ItemEditor.Init(serializedObject);

// 		var lookup = typeof(EffectData);
// 		m_AvailableEquipEffectType = System.AppDomain.CurrentDomain.GetAssemblies()
// 			.SelectMany(assembly => assembly.GetTypes())
// 			.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
// 			.Select(type => type.Name)
// 			.ToList();
// 	}

// 	public override void OnInspectorGUI()
// 	{
// 		m_ItemEditor.GUI();

// 		EditorGUILayout.PropertyField(m_SlotProperty);

// 		EditorGUILayout.PropertyField(m_MinimumStrengthProperty);
// 		EditorGUILayout.PropertyField(m_MinimumAgilityProperty);
// 		EditorGUILayout.PropertyField(m_MinimumDefenseProperty);

// 		int choice = EditorGUILayout.Popup("Add new Effect", -1, m_AvailableEquipEffectType.ToArray());

// 		if (choice != -1)
// 		{
// 			var newInstance = ScriptableObject.CreateInstance(m_AvailableEquipEffectType[choice]);

// 			AssetDatabase.AddObjectToAsset(newInstance, target);

// 			m_EquippedEffectListProperty.InsertArrayElementAtIndex(m_EquippedEffectListProperty.arraySize);
// 			m_EquippedEffectListProperty.GetArrayElementAtIndex(m_EquippedEffectListProperty.arraySize - 1).objectReferenceValue = newInstance;
// 		}

// 		Editor ed = null;
// 		int toDelete = -1;
// 		for (int i = 0; i < m_EquippedEffectListProperty.arraySize; ++i)
// 		{
// 			EditorGUILayout.BeginHorizontal();
// 			EditorGUILayout.BeginVertical();
// 			var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(i);
// 			SerializedObject obj = new SerializedObject(item.objectReferenceValue);

// 			Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

// 			ed.OnInspectorGUI();
// 			EditorGUILayout.EndVertical();

// 			if (GUILayout.Button("-", GUILayout.Width(32)))
// 			{
// 				toDelete = i;
// 			}
// 			EditorGUILayout.EndHorizontal();
// 		}

// 		if (toDelete != -1)
// 		{
// 			var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
// 			DestroyImmediate(item, true);

// 			//need to do it twice, first time just nullify the entry, second actually remove it.
// 			m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
// 			m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
// 		}

// 		serializedObject.ApplyModifiedProperties();
// 	}
// }
// #endif