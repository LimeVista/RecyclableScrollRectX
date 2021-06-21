//
// Copyright (C) 2021 LimeVista
// Author: LimeVista(https://github.com/LimeVista/RecyclableScrollRectX)
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the The MIT License (MIT).
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See The MIT License (MIT) for more details.
//
// You should have received a copy of The MIT License (MIT)
// along with this library.
//

using System;
using RecyclableScrollRectX;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace RecyclableScrollRectXEditor
{
    [CustomEditor(typeof(RecyclableScrollRect), true)]
    [CanEditMultipleObjects]
    public class RecyclableScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty _dataSource;
        private SerializedProperty _layoutMode;
        private SerializedProperty _orthogonalCount;
        private SerializedProperty _verticalSpace;
        private SerializedProperty _horizontalSpace;
        private RecyclableScrollRect _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _dataSource = serializedObject.FindProperty("dataSource");
            _layoutMode = serializedObject.FindProperty("layoutMode");
            _orthogonalCount = serializedObject.FindProperty("orthogonalCount");
            _verticalSpace = serializedObject.FindProperty("verticalSpace");
            _horizontalSpace = serializedObject.FindProperty("horizontalSpace");
            _target = (RecyclableScrollRect) target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_dataSource);
            EditorGUILayout.PropertyField(_layoutMode);
            switch (_target.LayoutMode)
            {
                case RecyclableScrollRectLayoutMode.Vertical:
                    if (EditorGUILayout.PropertyField(_horizontalSpace, new GUIContent("Top Padding")))
                    {
                        _horizontalSpace.floatValue = Math.Max(0, _horizontalSpace.floatValue);
                    }

                    if (EditorGUILayout.PropertyField(_verticalSpace, new GUIContent("Bottom Padding")))
                    {
                        _verticalSpace.floatValue = Math.Max(0, _verticalSpace.floatValue);
                    }

                    break;

                case RecyclableScrollRectLayoutMode.Horizontal:
                    if (EditorGUILayout.PropertyField(_horizontalSpace, new GUIContent("Left Padding")))
                    {
                        _horizontalSpace.floatValue = Math.Max(0, _horizontalSpace.floatValue);
                    }

                    if (EditorGUILayout.PropertyField(_verticalSpace, new GUIContent("Right Padding")))
                    {
                        _verticalSpace.floatValue = Math.Max(0, _verticalSpace.floatValue);
                    }

                    break;

                case RecyclableScrollRectLayoutMode.GridVertical:
                    if (EditorGUILayout.PropertyField(_orthogonalCount, new GUIContent("Columns")))
                    {
                        _orthogonalCount.intValue = Math.Max(1, _orthogonalCount.intValue);
                    }

                    if (EditorGUILayout.PropertyField(_verticalSpace, new GUIContent("Vertical Space")))
                    {
                        _verticalSpace.floatValue = Math.Max(0, _verticalSpace.floatValue);
                    }

                    if (EditorGUILayout.PropertyField(_horizontalSpace, new GUIContent("Horizontal Space")))
                    {
                        _horizontalSpace.floatValue = Math.Max(0, _horizontalSpace.floatValue);
                    }

                    break;

                case RecyclableScrollRectLayoutMode.GridHorizontal:
                    if (EditorGUILayout.PropertyField(_orthogonalCount, new GUIContent("Rows")))
                    {
                        _orthogonalCount.intValue = Math.Max(1, _orthogonalCount.intValue);
                    }

                    if (EditorGUILayout.PropertyField(_verticalSpace, new GUIContent("Vertical Space")))
                    {
                        _verticalSpace.floatValue = Math.Max(0, _verticalSpace.floatValue);
                    }

                    if (EditorGUILayout.PropertyField(_horizontalSpace, new GUIContent("Horizontal Space")))
                    {
                        _horizontalSpace.floatValue = Math.Max(0, _horizontalSpace.floatValue);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}