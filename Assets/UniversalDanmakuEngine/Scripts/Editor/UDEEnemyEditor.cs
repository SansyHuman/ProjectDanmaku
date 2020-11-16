// Copyright (c) 2019 Subo Lee (KAIST HAJE)
// Please direct any bugs/comments/suggestions to suboo0308@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using SansyHuman.UDE.Object;

namespace SansyHuman.UDE.Editor
{
    [CustomEditor(typeof(UDEEnemy))]
    public class UDEEnemyEditor : UnityEditor.Editor
    {
        protected SerializedProperty shotPatterns;
        protected SerializedProperty scoreOnDeath;
        protected SerializedProperty canBeDamaged;

        UDEEnemy targetEnemy;

        protected virtual void OnEnable()
        {
            targetEnemy = (UDEEnemy)target;

            shotPatterns = serializedObject.FindProperty("shotPatterns");
            scoreOnDeath = serializedObject.FindProperty("scoreOnDeath");
            canBeDamaged = serializedObject.FindProperty("canBeDamaged");
        }

        protected bool showEnemyProperties = true;

        protected int enemyType = 0; // 0: Destroyable 1: Invincible

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (canBeDamaged.boolValue)
                enemyType = 0;
            else
                enemyType = 1;

            showEnemyProperties = EditorGUILayout.Foldout(showEnemyProperties, "Enemy Properties", true);
            if(showEnemyProperties)
            {
                EditorGUILayout.PropertyField(shotPatterns, true);
                EditorGUILayout.Space();
                scoreOnDeath.intValue = EditorGUILayout.IntField("Score on Death", scoreOnDeath.intValue);
                enemyType = EditorGUILayout.Popup("Enemy Type", enemyType, new string[] { "Destroyable", "Invincible" });
                switch(enemyType)
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
            serializedObject.ApplyModifiedProperties();
        }
    }
}