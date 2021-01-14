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
    /// （激活型）单元格扩展
    /// </summary>
    public readonly struct CellItem
    {
        public static readonly CellItem Zero = new CellItem(0, 0, CellExt.Zero);

        public readonly int Index;
        public readonly float Offset;
        public readonly CellExt Cell;

        public CellItem(int index, float offset, CellExt cell)
        {
            Index = index;
            Offset = offset;
            Cell = cell;
        }

        /// <summary>
        /// 计算高度
        /// </summary>
        /// <returns>高度</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float CalcHeight() { return Cell.Transform.rect.height; }

        /// <summary>
        /// 计算宽度
        /// </summary>
        /// <returns>宽度</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float CalcWidth() { return Cell.Transform.rect.width; }
    }

    /// <summary>
    /// 单元格扩展（包含基础信息）
    /// </summary>
    public readonly struct CellExt
    {
        public static readonly CellExt Zero = new CellExt(null, 0);

        public readonly RecyclingSystem.ICell Cell;
        public readonly RectTransform Transform;
        public readonly int Type;

        public CellExt(RectTransform transform, int type)
        {
            Cell = transform == null ? null : transform.GetComponent<RecyclingSystem.ICell>();
            Transform = transform;
            Type = type;
        }

        public override string ToString() { return $"{nameof(Transform)}: {Transform}, {nameof(Type)}: {Type}"; }
    }

    /// <summary>
    /// 单元格细胞卵（原型，被克隆体），附加克隆变异信息
    /// </summary>
    public readonly struct CellZygote
    {
        public readonly int Type;
        public readonly float FixedWidth;
        public readonly float FixedHeight;
        public readonly Vector2 AnchorMin;
        public readonly Vector2 AnchorMax;
        public readonly Vector2 Pivot;
        public readonly RectTransform Zygote;

        public CellZygote(
            int type,
            float fixedWidth,
            float fixedHeight,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            RectTransform zygote
        )
        {
            Type = type;
            FixedWidth = fixedWidth;
            FixedHeight = fixedHeight;
            AnchorMin = anchorMin;
            AnchorMax = anchorMax;
            Pivot = pivot;
            Zygote = zygote;
        }

        public static bool operator ==(CellZygote a, CellZygote b) => a.Equals(b);

        public static bool operator !=(CellZygote a, CellZygote b) => !a.Equals(b);

        public override bool Equals(object obj) => obj is CellZygote other && Equals(other);

        public override int GetHashCode() => Type;

        private bool Equals(CellZygote other) => Type == other.Type;
    }
}