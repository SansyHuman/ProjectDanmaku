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
using SansyHuman.UDE.Management;

namespace SansyHuman.UDE.Editor
{
    [CustomEditor(typeof(UDETime))]
    public class UDETimeEditor : UnityEditor.Editor
    {
        SerializedProperty enemyTimeScale;
        SerializedProperty playerTimeScale;

        private void OnEnable()
        {
            enemyTimeScale = serializedObject.FindProperty("enemyTimeScale");
            playerTimeScale = serializedObject.FindProperty("playerTimeScale");
        }

        bool showTimeScales = true;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showTimeScales = EditorGUILayout.Foldout(showTimeScales, "Time Scales");
            if (showTimeScales)
            {
                enemyTimeScale.floatValue = EditorGUILayout.Slider("Enemy Time Scale", enemyTimeScale.floatValue, 0, 2);
                playerTimeScale.floatValue = EditorGUILayout.Slider("Player Time Scale", playerTimeScale.floatValue, 0, 2);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}