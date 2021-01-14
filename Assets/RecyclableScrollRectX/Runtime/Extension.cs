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

using System.Runtime.CompilerServices;
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 扩展
    /// </summary>
    internal static class Extension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float MinX(this RectTransform rt) => rt.GetCorners()[0].x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float MaxX(this RectTransform rt) => rt.GetCorners()[2].x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float MinY(this RectTransform rt) => rt.GetCorners()[0].y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float MaxY(this RectTransform rt) => rt.GetCorners()[1].y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3[] GetCorners(this RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return corners;
        }

        /// <summary>
        /// 设置组件锚点为顶部
        /// </summary>
        /// <param name="rt">组件</param>
        internal static void AnchorToTop(this RectTransform rt)
        {
            var rect = rt.rect;
            float width = rect.width, height = rect.height;

            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.pivot = new Vector2(0.5f, 1);

            rt.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// 设置组件锚点为左上角
        /// </summary>
        /// <param name="rt">组件</param>
        internal static void AnchorToTopLeft(this RectTransform rt)
        {
            var rect = rt.rect;
            float width = rect.width, height = rect.height;

            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);

            rt.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// 设置组件锚点为左部
        /// </summary>
        /// <param name="rt">组件</param>
        internal static void AnchorToLeft(this RectTransform rt)
        {
            var rect = rt.rect;
            float width = rect.width, height = rect.height;

            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0, 0.5f);

            rt.sizeDelta = new Vector2(width, height);
        }
    }
}