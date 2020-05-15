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

using UnityEngine;
using UnityEditor;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.UDE.Editor
{
    [CustomEditor(typeof(UDECurve))]
    public class UDECurveEditor : UnityEditor.Editor
    {
        SerializedProperty curveName;
        SerializedProperty points;
        SerializedProperty precision;
        SerializedProperty type;

        UDECurve targetCurve;

        GUIStyle style = new GUIStyle();

        private void OnEnable()
        {
            targetCurve = (UDECurve)target;

            curveName = serializedObject.FindProperty("curveName");
            points = serializedObject.FindProperty("points");
            precision = serializedObject.FindProperty("precision");
            type = serializedObject.FindProperty("type");

            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
        }

        GUIContent typeLbl = new GUIContent("Curve Type", "Type of the curve.\nBezier: Bezier curve\nCubic Spline: Natural cubic spline curve");
        GUIContent pointsLbl = new GUIContent("Control Points", "Control Points of the curve.");
        GUIContent precisionLbl = new GUIContent("Precision", "Number of intervals to draw the curve. Only affects the curve shape in edit mode.");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            curveName.stringValue = EditorGUILayout.TextField("Curve Name", curveName.stringValue);
            EditorGUILayout.PropertyField(type, typeLbl);
            EditorGUILayout.PropertyField(points, pointsLbl, true);
            precision.intValue = EditorGUILayout.IntSlider(precisionLbl, precision.intValue, 1, 1000);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();

            int length = points.arraySize;
            if (length == 0)
                return;

            Undo.RecordObject(targetCurve, "Adjust UDE Bezier Curve");

            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                    Handles.Label(points.GetArrayElementAtIndex(i).vector2Value, "(" + curveName.stringValue + ") 1, Begin", style);
                else if (i == length - 1)
                    Handles.Label(points.GetArrayElementAtIndex(i).vector2Value, "(" + curveName.stringValue + ") " + (i + 1) + ", End", style);
                else
                    Handles.Label(points.GetArrayElementAtIndex(i).vector2Value, "(" + curveName.stringValue + ") " + (i + 1).ToString(), style);

                points.GetArrayElementAtIndex(i).vector2Value = Handles.PositionHandle(points.GetArrayElementAtIndex(i).vector2Value, Quaternion.identity);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}