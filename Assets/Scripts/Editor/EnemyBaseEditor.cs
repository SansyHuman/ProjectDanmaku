using System.Collections;
using System.Collections.Generic;

using SansyHuman.Enemy;
using UnityEditor;
using UnityEngine;

namespace SansyHuman.Editor
{
    [CustomEditor(typeof(EnemyBase), true)]
    public class EnemyBaseEditor : SansyHuman.UDE.Editor.UDEEnemyEditor
    {
        protected SerializedProperty death;
        protected SerializedProperty dropItems;
        protected SerializedProperty dropPowerItems;
        protected SerializedProperty itemDropRange;

        protected override void OnEnable()
        {
            base.OnEnable();

            death = serializedObject.FindProperty("death");
            dropItems = serializedObject.FindProperty("dropItems");
            dropPowerItems = serializedObject.FindProperty("dropPowerItems");
            itemDropRange = serializedObject.FindProperty("itemDropRange");
        }

        protected GUIContent itemDropRangeLbl = new GUIContent("Item Drop Range", "The radius of the area where the items will spawn randomly around the enemy.");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(death, true);
            EditorGUILayout.PropertyField(dropPowerItems, true);
            EditorGUILayout.PropertyField(dropItems, true);
            itemDropRange.floatValue = EditorGUILayout.Slider(itemDropRangeLbl, itemDropRange.floatValue, 0.1f, 5f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}