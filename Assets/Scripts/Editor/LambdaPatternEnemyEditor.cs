using System.Collections;
using System.Collections.Generic;

using SansyHuman.Enemy;

using UnityEditor;

using UnityEngine;

namespace SansyHuman.Editor
{
    [CustomEditor(typeof(LambdaPatternEnemy), true)]
    public class LambdaPatternEnemyEditor : EnemyBaseEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (canBeDamaged.boolValue)
                enemyType = 0;
            else
                enemyType = 1;

            showEnemyProperties = EditorGUILayout.Foldout(showEnemyProperties, "Enemy Properties", true);
            if (showEnemyProperties)
            {
                EditorGUILayout.Space();
                scoreOnDeath.intValue = EditorGUILayout.IntField("Score on Death", scoreOnDeath.intValue);
                enemyType = EditorGUILayout.Popup("Enemy Type", enemyType, new string[] { "Destroyable", "Invincible" });
                switch (enemyType)
                {
                    case 0:
                        canBeDamaged.boolValue = true;
                        break;
                    case 1:
                        canBeDamaged.boolValue = false;
                        break;
                }
            }
            if (scoreOnDeath.intValue < 0)
                scoreOnDeath.intValue = 0;

            EditorGUILayout.PropertyField(death, true);
            EditorGUILayout.PropertyField(dropPowerItems, true);
            EditorGUILayout.PropertyField(dropItems, true);
            itemDropRange.floatValue = EditorGUILayout.Slider(itemDropRangeLbl, itemDropRange.floatValue, 0.1f, 5f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}