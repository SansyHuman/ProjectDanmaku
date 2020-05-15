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

using System;
using UnityEditor;
using UnityEngine;
using SansyHuman.UDE.Object;

namespace SansyHuman.UDE.Editor
{
    [CustomEditor(typeof(UDEBaseBullet))]
    public class UDEBaseBulletEditor : UnityEditor.Editor
    {
        SerializedProperty originCharacter;
        SerializedProperty originShotPattern;
        SerializedProperty damage;

        SerializedProperty origin;
        SerializedProperty setOriginToCharacter;
        SerializedProperty position;
        SerializedProperty rotation;

        SerializedProperty r;
        SerializedProperty angle;

        SerializedProperty summonTime;
        SerializedProperty isSummoning;

        protected UDEBaseBullet targetBullet;

        protected virtual void OnEnable()
        {
            targetBullet = (UDEBaseBullet)target;

            originCharacter = serializedObject.FindProperty("originCharacter");
            originShotPattern = serializedObject.FindProperty("originShotPattern");
            damage = serializedObject.FindProperty("damage");

            origin = serializedObject.FindProperty("origin");
            setOriginToCharacter = serializedObject.FindProperty("setOriginToCharacter");
            position = serializedObject.FindProperty("position");
            rotation = serializedObject.FindProperty("rotation");

            r = serializedObject.FindProperty("r");
            angle = serializedObject.FindProperty("angle");

            summonTime = serializedObject.FindProperty("summonTime");
            isSummoning = serializedObject.FindProperty("isSummoning");
        }

        bool showBulletProperties = true;
        bool showInformations = false;
        bool showParents = true;
        bool showCoordinates = true;
        bool showPolarCoordinates = true;
        bool showStatus = true;

        GUIContent damageLbl = new GUIContent("Damage", "Damage dealt by the bullet.");
        GUIContent summonTimeLbl = new GUIContent("Summon Time", "Time the bullet is in summon phase when it activated.");
        GUIContent characterLbl = new GUIContent("Character", "Character who shot the bullet.");
        GUIContent patternLbl = new GUIContent("Shot Pattern", "Pattern which shot the bullet.");
        GUIContent originLbl = new GUIContent("Origin", "Origin of the bullet in polar coordinate system.");
        GUIContent absPositionLbl = new GUIContent("Absolute Position", "Position in world space");
        GUIContent relPositionLbl = new GUIContent("Relative Position", "Position in system of origin.");
        GUIContent rotationLbl = new GUIContent("Bullet's Rotation", "Angle of self rotation of the bullet.");
        GUIContent radiusLbl = new GUIContent("Radius", "Distance from the origin.");
        GUIContent angleLbl = new GUIContent("Angle", "Angle in polar coordinate system.\nHorizontal right is 0 degrees.");

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
                EditorGUILayout.Space();

                showCoordinates = EditorGUILayout.Foldout(showCoordinates, "Coordinates", true);
                if (showCoordinates)
                {
                    EditorGUILayout.LabelField(originLbl, new GUIContent(origin == null ? "NaV" : origin.vector2Value.ToString()));
                    if (setOriginToCharacter != null && setOriginToCharacter.boolValue)
                        EditorGUILayout.LabelField("The bullets origin is following its parent character.");
                    EditorGUILayout.LabelField(absPositionLbl, new GUIContent(position == null ? "NaV" : position.vector2Value.ToString()));
                    string relativePosition = (position != null && origin != null) ? (position.vector2Value - origin.vector2Value).ToString() : "NaV";
                    EditorGUILayout.LabelField(relPositionLbl, new GUIContent(relativePosition));
                    EditorGUILayout.LabelField(rotationLbl, new GUIContent(rotation == null ? "NaN" : rotation.floatValue.ToString()));
                }
                EditorGUILayout.Space();

                showPolarCoordinates = EditorGUILayout.Foldout(showPolarCoordinates, "Polar Coordinates", true);
                if (showPolarCoordinates)
                {
                    EditorGUILayout.LabelField("All angles are in degrees.");
                    EditorGUILayout.LabelField(radiusLbl, new GUIContent(r == null ? "NaN" : r.floatValue.ToString()));
                    EditorGUILayout.LabelField(angleLbl, new GUIContent(angle == null ? "NaN" : angle.floatValue.ToString()));
                }
                EditorGUILayout.Space();

                showStatus = EditorGUILayout.Foldout(showStatus, "Status", true);
                if (showStatus)
                {
                    string status = "";
                    if (!targetBullet.gameObject.activeSelf)
                        status = "Disabled";
                    else
                    {
                        if (isSummoning.boolValue)
                            status = "Summoning";
                        else
                            status = "Active";
                    }
                    EditorGUILayout.LabelField("Status", status);
                    if (status.Equals("Active"))
                    {
                        try
                        {
                            EditorGUILayout.LabelField("Movement Info");
                            EditorGUILayout.TextArea(targetBullet.CurrentMovement.ToString());
                        }
                        catch (NullReferenceException e)
                        {
                            EditorGUILayout.LabelField("Exception: There are no movements.");
                            Debug.LogException(e);
                            Debug.LogErrorFormat("There are no movements registered in bullet {0}. The bullet is prefab or not initialized.", targetBullet.name);
                        }
                    }
                }
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}