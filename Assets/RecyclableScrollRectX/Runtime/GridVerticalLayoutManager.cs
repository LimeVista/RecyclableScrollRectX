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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 网格垂直布局管理器
    /// </summary>
    public class GridVerticalLayoutManager : LayoutManager
    {
        public GridVerticalLayoutManager(int column)
        {
            if (column < 1) column = 1;
            Column = column;
        }

        /// <summary>
        /// 列
        /// </summary>
        public int Column { get; private set; }

        public override float MinPoolCoverage => 1.5f;

        #region 抽象实现

        public override Direction Direction => Direction.Vertical;

        public override float CalcContentLengthOfSlidingDirection()
        {
            var ds = Delegate.DataSource;
            var count = ds.GetCellCount();
            if (count < 1) return 0f;
            if (!ds.SingleZygoteMode)
            {
                throw new ArgumentException("GridVertical is unsupported multiple zygote mode.");
            }

            var zygote = Delegate.GetCellZygote(0);
            var rows = (count - 1) / Column + 1;
            return zygote.FixedHeight * rows;
        }

        public override Vector2 CalcCellOffset(RecyclingSystem.ICell cell, int index)
        {
            if (index < 1) return Vector2.zero;
            if (!Delegate.DataSource.SingleZygoteMode)
            {
                throw new ArgumentException("GridVertical is unsupported multiple zygote mode.");
            }

            var zygote = Delegate.GetCellZygote(0);
            var row = index / Column;
            return new Vector2(0, zygote.FixedHeight * row);
        }

        public override CellZygote OnProcessCellZygote(int type)
        {
            var zygote = System.DataSource.GetCellZygote(type);
            var obj = zygote.gameObject;
            obj.SetActive(true);

            // 等比缩放
            var sizeDelta = zygote.sizeDelta;
            var cellWidth = System.Content.rect.width / Column;
            var cellHeight = sizeDelta.y / sizeDelta.x * cellWidth;

            if (obj.scene.IsValid()) obj.SetActive(false);

            var value = new Vector2(0f, 1);
            return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
        }

        public override void InitLayoutManger()
        {
            var rect = Delegate.Viewport.rect;
            _edge = rect.height * (MinPoolCoverage - 1) * 0.5f;
            var ds = Delegate.DataSource;
            if (ds.GetCellCount() < 1) return;

            var maxY = rect.height * MinPoolCoverage;
            var count = ds.GetCellCount();
            var zygote = Delegate.GetCellZygote(0);
            var cellHeight = zygote.FixedHeight;
            var cellWidth = zygote.FixedWidth;

            var offset = 0f;
            var i = 0;
            while (i < count && (offset < maxY || i < MinCellCount))
            {
                var cell = PopCachedPoolPoolAndPushToUsedPool(i, cellWidth, cellHeight, i / Column);
                ds.OnBindCell(cell.Cell.Cell, i);
                i++;
                offset = Mathf.Abs(cell.Offset);
                if (i % Column == 0) offset += cellHeight;
            }
        }

        public override void OnValueChanged(AbsRecyclableScrollRect scrollRect, Vector2 v)
        {
            var normalizedPosition = scrollRect.verticalNormalizedPosition;
            var contentHeight = Delegate.Content.rect.height;
            var viewportHeight = Delegate.Viewport.rect.height;
            if (contentHeight <= viewportHeight) return;

            normalizedPosition = Mathf.Clamp(normalizedPosition, 0f, 1f);
            normalizedPosition = 1f - normalizedPosition;
            var posY = normalizedPosition * (contentHeight - viewportHeight);
            var dir = posY - _prevOffset;

            // 少于最小移动距离则不移动
            if (Mathf.Abs(dir) < MinMovingDistance) return;

            _prevOffset = posY;
            var min = posY - _edge;
            var max = posY + _edge + viewportHeight;

            var zygote = Delegate.GetCellZygote(0);
            var dataSource = Delegate.DataSource;
            var cellHeight = zygote.FixedHeight;
            var cellWidth = zygote.FixedWidth;
            var cellCount = dataSource.GetCellCount();

            // top >> bottom
            if (dir > 0)
            {
                foreach (var item in Delegate.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y) + tf.rect.height;
                    if (min <= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var bottom = Delegate.BigIndexFromUsedPool();
                while (bottom.Index + 1 < cellCount && max >= Mathf.Abs(bottom.Offset) + cellHeight)
                {
                    var index = bottom.Index + 1;
                    var row = index / Column;
                    while (row == index / Column && index < cellCount)
                    {
                        bottom = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight, row);
                        dataSource.OnBindCell(bottom.Cell.Cell, index);
                        index++;
                    }
                }
            }

            // bottom >> top
            if (dir < 0)
            {
                for (var i = Delegate.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Delegate.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = Mathf.Abs(tf.anchoredPosition.y);
                    if (max >= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var top = Delegate.LittleIndexFromUsedPool();
                while (min <= Mathf.Abs(top.Offset) && top.Index > 0)
                {
                    var index = top.Index - 1;
                    var row = index / Column;
                    while (row == index / Column && index >= 0)
                    {
                        top = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight, row);
                        dataSource.OnBindCell(top.Cell.Cell, index);
                        index--;
                    }
                }
            }
        }

        #endregion

        #region 私有方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CellItem PopCachedPoolPoolAndPushToUsedPool(int index, float cellWidth, float cellHeight, int row)
        {
            var cell = Delegate.PopFromCachedPool(index);
            var tf = cell.Transform;
            var ap = tf.anchoredPosition;
            ap.x = index % Column * cellWidth;
            ap.y = -cellHeight * row;
            tf.anchoredPosition = ap;
            var cur = new CellItem(index, ap.y, cell);
            Delegate.PushToUsedPool(cur);
            return cur;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecyclingCellFromList()
        {
            if (_recyclingPool.Count < 1) return;
            foreach (var cellExt in _recyclingPool)
            {
                Delegate.MoveToCachedPool(cellExt);
            }

            _recyclingPool.Clear();
        }

        #endregion

        #region 私有属性

        private const float MinMovingDistance = 1f;
        private const int MinCellCount = 6;
        private readonly List<CellItem> _recyclingPool = new List<CellItem>();
        private float _prevOffset;
        private float _edge;

        #endregion
    }
}