using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SansyHuman.UDE.ECS.Object;

namespace SansyHuman.UDE.Editor
{
    [CustomEditor(typeof(UDEBulletECS))]
    public class UDEBulletECSEditor : UnityEditor.Editor
    {
        SerializedProperty originCharacter;
        SerializedProperty originShotPattern;
        SerializedProperty damage;

        SerializedProperty summonTime;

        UDEBulletECS targetBullet;

        protected virtual void OnEnable()
        {
            targetBullet = (UDEBulletECS)target;

            originCharacter = serializedObject.FindProperty("originCharacter");
            originShotPattern = serializedObject.FindProperty("originShotPattern");
            damage = serializedObject.FindProperty("damage");

            summonTime = serializedObject.FindProperty("summonTime");
        }

        bool showBulletProperties = true;
        bool showInformations = false;
        bool showParents = true;

        GUIContent damageLbl = new GUIContent("Damage", "Damage dealt by the bullet.");
        GUIContent summonTimeLbl = new GUIContent("Summon Time", "Time the bullet is in summon phase when it activated.");
        GUIContent characterLbl = new GUIContent("Character", "Character who shot the bullet.");
        GUIContent patternLbl = new GUIContent("Shot Pattern", "Pattern which shot the bullet.");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showBulletProperties = EditorGUILayout.Foldout(showBulletProperties, "Bullet Properties", true);
            if (showBulletProperties)
            {
                damage.floatValue = EditorGUILayout.Slider(damageLbl, damage.floatValue, 0, 10);
                summonTime.floatValue = EditorGUILayout.Slider(summonTimeLbl, summonTime.floatValue, 0, 0.5f);
            }
            EditorGUILayout.Space();

            showInformations = EditorGUILayout.Foldout(showInformations, "Informations", true);
            if (showInformations)
            {
                showParents = EditorGUILayout.Foldout(showParents, "Parents", true);
                if (showParents)
                {
                    EditorGUILayout.LabelField(characterLbl, new GUIContent(originCharacter.objectReferenceValue == null ? "None" : originCharacter.objectReferenceValue.name));
                    EditorGUILayout.LabelField(patternLbl, new GUIContent(originShotPattern.objectReferenceValue == null ? "None" : originShotPattern.objectReferenceValue.name));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}