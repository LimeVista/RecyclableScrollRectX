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

using UnityEditor;
using UnityEngine;

namespace RecyclableScrollRectX.Editor
{
    public static class RecyclableScrollViewMenu
    {
        private const string PrefabPath = "Assets/RecyclableScrollRectX/Editor/Prefabs/RecyclableScrollView.prefab";

        [MenuItem("GameObject/UI/Recyclable Scroll View")]
        private static void CreateRecyclableScrollView()
        {
            var selected = Selection.activeGameObject;

            if (!selected || !(selected.transform is RectTransform))
            {
                selected = Object.FindObjectOfType<Canvas>().gameObject;
            }

            if (!selected) return;

            var asset = AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject)) as GameObject;
            var item = Object.Instantiate(asset, selected.transform);
            item.name = "Recyclable Scroll View";
            item.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = item;
            Undo.RegisterCreatedObjectUndo(item, "Create Recyclable Scroll View");
        }
    }
}