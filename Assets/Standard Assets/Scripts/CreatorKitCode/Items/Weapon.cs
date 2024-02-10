using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace CreatorKitCode
{
	/// <summary>
	/// Special case of EquipmentItem for weapon, as they have a whole attack system in addition. Like Equipment they
	/// can have minimum stats and equipped effect, but also have a list of WeaponAttackEffect that will have their
	/// OnAttack function called during a hit, and their OnPostAttack function called after all OnAttack of all effects
	/// are called.
	/// </summary>
	public class Weapon : EquipmentItem
	{
		public StatSystem.DamageType damageType;
		public Skill WeaponSkill;
		public Bullet bulletPb;
		public Transform bulletTrans;

		[System.Serializable]
		public struct Stat
		{
			public float Speed;
			public int MinimumDamage;
			public int MaximumDamage;
			public float MaxRange;
			public int AdditionalTargets;
		}
		[Header("Stats")]
		public Stat Stats = new Stat() { Speed = 1.0f, MaximumDamage = 1, MinimumDamage = 1, MaxRange = 1, AdditionalTargets = 0 };
		public List<EffectData> AttackEffects;

		[Header("Sounds")]
		public AudioClip[] HitSounds;
		public AudioClip[] SwingSounds;



		public void Attack(CharacterData attacker, CharacterData target)
		{
			Damage damage = new Damage(target, attacker);
			int damageNum = Random.Range(Stats.MinimumDamage, Stats.MaximumDamage + 1);
			damage.AddDamage(damageType, damageNum);
			if (bulletPb != null)
			{
				Vector3 bulletPos = attacker.GetComponent<RoleControl>().WeaponLocator.position;
				Bullet bullet = Instantiate(bulletPb, bulletPos, Quaternion.Euler(0, 180, 0));
				bullet.damage = damage;
				bullet.target = target;
			}
			else damage.Take();
			//foreach (var wae in AttackEffects)
			//wae.OnAttack(target, attacker, ref attackEffect);

			//target.TakeEffect(attackEffect);

			//foreach (var wae in AttackEffects)
			//wae.OnPostAttack(target, attacker, attackEffect);
		}

		public bool CanHit(CharacterData attacker, CharacterData target)
		{
			if (Vector3.SqrMagnitude(attacker.transform.position - target.transform.position) < Stats.MaxRange * Stats.MaxRange)
			{
				return true;
			}

			return false;
		}

		public override string GetDescription()
		{
			string desc = base.GetDescription();

			int minimumDPS = Mathf.RoundToInt(Stats.MinimumDamage / Stats.Speed);
			int maximumDPS = Mathf.RoundToInt(Stats.MaximumDamage / Stats.Speed);

			desc += "\n";
			desc += $"Damage: {Stats.MinimumDamage} - {Stats.MaximumDamage}\n";
			desc += $"DPS: {minimumDPS} - {maximumDPS}\n";
			desc += $"Attack Speed : {Stats.Speed}s\n";
			desc += $"Range : {Stats.MaxRange}m\n";

			return desc;
		}

		public AudioClip GetHitSound()
		{
			if (HitSounds == null || HitSounds.Length == 0)
				return SFXManager.GetDefaultHit();

			return HitSounds[Random.Range(0, HitSounds.Length)];
		}

		public AudioClip GetSwingSound()
		{
			if (SwingSounds == null || SwingSounds.Length == 0)
				return SFXManager.GetDefaultSwingSound();

			return SwingSounds[Random.Range(0, SwingSounds.Length)];
		}
	}
}

// #if UNITY_EDITOR
// [CustomEditor(typeof(Weapon))]
// public class WeaponEditor : Editor
// {
//     Weapon m_Target;

//     ItemEditor m_ItemEditor;

//     List<string> m_AvailableEquipEffectType;
//     SerializedProperty m_EquippedEffectListProperty;

//     List<string> m_AvailableWeaponAttackEffectType;
//     SerializedProperty m_WeaponAttackEffectListProperty;

//     SerializedProperty m_HitSoundProps;
//     SerializedProperty m_SwingSoundProps;

//     SerializedProperty m_MinimumStrengthProperty;
//     SerializedProperty m_MinimumAgilityProperty;
//     SerializedProperty m_MinimumDefenseProperty;

//     SerializedProperty m_WeaponStatProperty;

//     [MenuItem("Assets/Create/Data/Weapon", priority = -999)]
//     static public void CreateWeapon()
//     {
//         var newWeapon = CreateInstance<Weapon>();
//         newWeapon.Slot = (EquipmentItem.EquipmentSlot)666;

//         ProjectWindowUtil.CreateAsset(newWeapon, "weapon.asset");
//     }

//     void OnEnable()
//     {
//         m_Target = target as Weapon;
//         m_EquippedEffectListProperty = serializedObject.FindProperty(nameof(Weapon.EquippedEffects));
//         m_WeaponAttackEffectListProperty = serializedObject.FindProperty(nameof(Weapon.AttackEffects));

//         m_MinimumStrengthProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumStrength));
//         m_MinimumAgilityProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumAgility));
//         m_MinimumDefenseProperty = serializedObject.FindProperty(nameof(EquipmentItem.MinimumDefense));

//         m_WeaponStatProperty = serializedObject.FindProperty(nameof(Weapon.Stats));

//         m_HitSoundProps = serializedObject.FindProperty(nameof(Weapon.HitSounds));
//         m_SwingSoundProps = serializedObject.FindProperty(nameof(Weapon.SwingSounds));

//         m_ItemEditor = new ItemEditor();
//         m_ItemEditor.Init(serializedObject);

//         var lookup = typeof(EffectData);
//         m_AvailableEquipEffectType = System.AppDomain.CurrentDomain.GetAssemblies()
//             .SelectMany(assembly => assembly.GetTypes())
//             .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
//             .Select(type => type.Name)
//             .ToList();

//         lookup = typeof(EffectData);
//         m_AvailableWeaponAttackEffectType = System.AppDomain.CurrentDomain.GetAssemblies()
//             .SelectMany(assembly => assembly.GetTypes())
//             .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
//             .Select(type => type.Name)
//             .ToList();
//     }

//     public override void OnInspectorGUI()
//     {
//         m_ItemEditor.GUI();

//         EditorGUILayout.PropertyField(m_MinimumStrengthProperty);
//         EditorGUILayout.PropertyField(m_MinimumAgilityProperty);
//         EditorGUILayout.PropertyField(m_MinimumDefenseProperty);

//         //EditorGUILayout.PropertyField(m_WeaponStatProperty, true);
//         var child = m_WeaponStatProperty.Copy();
//         var depth = child.depth;
//         child.NextVisible(true);

//         EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);
//         while (child.depth > depth)
//         {
//             EditorGUILayout.PropertyField(child, true);
//             child.NextVisible(false);
//         }

//         EditorGUILayout.PropertyField(m_HitSoundProps, true);
//         EditorGUILayout.PropertyField(m_SwingSoundProps, true);

//         int choice = EditorGUILayout.Popup("Add new Equipment Effect", -1, m_AvailableEquipEffectType.ToArray());

//         if (choice != -1)
//         {
//             var newInstance = ScriptableObject.CreateInstance(m_AvailableEquipEffectType[choice]);

//             AssetDatabase.AddObjectToAsset(newInstance, target);

//             m_EquippedEffectListProperty.InsertArrayElementAtIndex(m_EquippedEffectListProperty.arraySize);
//             m_EquippedEffectListProperty.GetArrayElementAtIndex(m_EquippedEffectListProperty.arraySize - 1).objectReferenceValue = newInstance;
//         }


//         Editor ed = null;
//         int toDelete = -1;
//         for (int i = 0; i < m_EquippedEffectListProperty.arraySize; ++i)
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.BeginVertical();
//             var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(i);           
//             SerializedObject obj = new SerializedObject(item.objectReferenceValue);

//             Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

//             ed.OnInspectorGUI();
//             EditorGUILayout.EndVertical();

//             if (GUILayout.Button("-", GUILayout.Width(32)))
//             {
//                 toDelete = i;
//             }
//             EditorGUILayout.EndHorizontal();
//         }

//         if (toDelete != -1)
//         {
//             var item = m_EquippedEffectListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
//             DestroyImmediate(item, true);

//             //need to do it twice, first time just nullify the entry, second actually remove it.
//             m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
//             m_EquippedEffectListProperty.DeleteArrayElementAtIndex(toDelete);
//         }

//         //attack
//         choice = EditorGUILayout.Popup("Add new Weapon Attack Effect", -1, m_AvailableWeaponAttackEffectType.ToArray());

//         if (choice != -1)
//         {
//             var newInstance = ScriptableObject.CreateInstance(m_AvailableWeaponAttackEffectType[choice]);

//             AssetDatabase.AddObjectToAsset(newInstance, target);

//             m_WeaponAttackEffectListProperty.InsertArrayElementAtIndex(m_WeaponAttackEffectListProperty.arraySize);
//             m_WeaponAttackEffectListProperty.GetArrayElementAtIndex(m_WeaponAttackEffectListProperty.arraySize - 1).objectReferenceValue = newInstance;
//         }

//         toDelete = -1;
//         for (int i = 0; i < m_WeaponAttackEffectListProperty.arraySize; ++i)
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.BeginVertical();
//             var item = m_WeaponAttackEffectListProperty.GetArrayElementAtIndex(i);           
//             SerializedObject obj = new SerializedObject(item.objectReferenceValue);

//             Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

//             ed.OnInspectorGUI();
//             EditorGUILayout.EndVertical();

//             if (GUILayout.Button("-", GUILayout.Width(32)))
//             {
//                 toDelete = i;
//             }
//             EditorGUILayout.EndHorizontal();
//         }

//         if (toDelete != -1)
//         {
//             var item = m_WeaponAttackEffectListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
//             DestroyImmediate(item, true);

//             //need to do it twice, first time just nullify the entry, second actually remove it.
//             m_WeaponAttackEffectListProperty.DeleteArrayElementAtIndex(toDelete);
//             m_WeaponAttackEffectListProperty.DeleteArrayElementAtIndex(toDelete);
//         }

//         serializedObject.ApplyModifiedProperties();
//     }
// }
// #endif