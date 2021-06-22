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

namespace RecyclableScrollRectXEditor
{
    public static class RecyclableScrollViewMenu
    {
        [MenuItem("GameObject/UI/Recyclable Scroll View")]
        private static void CreateRecyclableScrollView()
        {
            var selected = Selection.activeGameObject;

            if (!selected || !(selected.transform is RectTransform))
            {
                selected = Object.FindObjectOfType<Canvas>().gameObject;
            }

            if (!selected) return;

            var asset = LoadPrefab("RecyclableScrollView");
            var item = Object.Instantiate(asset, selected.transform);
            item.name = "Recyclable Scroll View";
            item.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = item;
            Undo.RegisterCreatedObjectUndo(item, "Create Recyclable Scroll View");
        }


        private static GameObject LoadPrefab(string name)
        {
            var fullPath = $"Packages/com.limice.recyclable.scrollrect/Editor/Prefabs/{name}.prefab";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
            if (asset != null) return asset;
            fullPath = $"Assets/RecyclableScrollRectX/Editor/Prefabs/{name}.prefab";
            asset = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
            return asset;
        }
    }
}