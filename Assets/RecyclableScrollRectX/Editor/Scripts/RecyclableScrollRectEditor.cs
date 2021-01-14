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
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace RecyclableScrollRectX.Editor
{
    [CustomEditor(typeof(RecyclableScrollRect), true)]
    [CanEditMultipleObjects]
    public class RecyclableScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty _dataSource;
        private SerializedProperty _layoutMode;
        private SerializedProperty _orthogonalCount;
        private RecyclableScrollRect _target;

        protected override void OnEnable()
        {
            base.OnEnable();
            _dataSource = serializedObject.FindProperty("dataSource");
            _layoutMode = serializedObject.FindProperty("layoutMode");
            _orthogonalCount = serializedObject.FindProperty("orthogonalCount");
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
                    break;
                case RecyclableScrollRectLayoutMode.Horizontal:
                    break;
                case RecyclableScrollRectLayoutMode.GridVertical:
                    EditorGUILayout.PropertyField(_orthogonalCount, new GUIContent("Columns"));
                    _orthogonalCount.intValue = Math.Max(1, _orthogonalCount.intValue);
                    break;
                case RecyclableScrollRectLayoutMode.GridHorizontal:
                    EditorGUILayout.PropertyField(_orthogonalCount, new GUIContent("Rows"));
                    _orthogonalCount.intValue = Math.Max(1, _orthogonalCount.intValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}