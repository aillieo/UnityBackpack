// -----------------------------------------------------------------------
// <copyright file="GridDataDrawer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(BaseGridData<>), true)]
    public class GridDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var rows = 1;
            var elements = property.FindPropertyRelative("data").arraySize;
            var width = property.FindPropertyRelative("width").intValue;
            if (elements > 0 && width > 0)
            {
                var height = elements / width;
                rows += height;
            }

            return rows * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var elements = property.FindPropertyRelative("data").arraySize;
            var width = property.FindPropertyRelative("width").intValue;
            var height = width > 0 ? elements / width : 0;

            var singleLineHeight = EditorGUIUtility.singleLineHeight;

            var firstRowRect = new Rect(position.x, position.y, position.width, singleLineHeight);
            var leftHalfRect = new Rect(firstRowRect.x, firstRowRect.y, firstRowRect.width / 2, firstRowRect.height);
            var rightHalfRect = new Rect(firstRowRect.x + (firstRowRect.width / 2), firstRowRect.y, firstRowRect.width / 2, firstRowRect.height);

            EditorGUI.BeginChangeCheck();
            width = EditorGUI.IntField(leftHalfRect, "Width", width);
            height = EditorGUI.IntField(rightHalfRect, "Height", height);
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("width").intValue = width;
                property.FindPropertyRelative("data").arraySize = width * height;
                return;
            }

            var propertyData = property.FindPropertyRelative("data");
            var secondRowStart = position.y + singleLineHeight;
            var singleElementRectWidth = position.width / width;
            singleElementRectWidth = Mathf.Min(singleElementRectWidth, 80);
            var singleElementRect = new Rect(position.x, secondRowStart, singleElementRectWidth, singleLineHeight);
            for (var y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var element = propertyData.GetArrayElementAtIndex(x + (y * width));
                    EditorGUI.PropertyField(singleElementRect, element, GUIContent.none);
                    singleElementRect.x += singleElementRectWidth;
                }

                singleElementRect.x = position.x;
                singleElementRect.y += singleLineHeight;
            }
        }
    }
}
