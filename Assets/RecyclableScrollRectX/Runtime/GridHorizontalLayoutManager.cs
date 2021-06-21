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
    /// 网格水平布局管理器
    /// </summary>
    public class GridHorizontalLayoutManager : LayoutManager
    {
        public GridHorizontalLayoutManager(int row, float horizontalSpace, float verticalSpace)
        {
            if (row < 1) row = 1;
            Row = row;
            HorizontalSpace = horizontalSpace;
            VerticalSpace = verticalSpace;
        }

        /// <summary>
        /// 行
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// 水平间隔
        /// </summary>
        public float HorizontalSpace { get; }

        /// <summary>
        /// 垂直间隔
        /// </summary>
        public float VerticalSpace { get; }

        public override float MinPoolCoverage => 1.5f;

        #region 抽象实现

        public override Direction Direction => Direction.Horizontal;

        public override float CalcContentLengthOfSlidingDirection()
        {
            var ds = Delegate.DataSource;
            var count = ds.GetCellCount();
            if (count < 1) return 0f;
            if (!ds.SingleZygoteMode)
            {
                throw new ArgumentException("GridHorizontal is unsupported multiple zygote mode.");
            }

            var zygote = Delegate.GetCellZygote(0);
            var column = (count - 1) / Row + 1;
            return zygote.FixedWidth * column + (column + 1) * HorizontalSpace;
        }

        public override Vector2 CalcCellOffset(RecyclingSystem.ICell cell, int index)
        {
            if (index < 1) return Vector2.zero;
            if (!Delegate.DataSource.SingleZygoteMode)
            {
                throw new ArgumentException("GridHorizontal is unsupported multiple zygote mode.");
            }

            var zygote = Delegate.GetCellZygote(0);
            return CalcCellOffset(index, zygote.FixedWidth, zygote.FixedHeight);
        }

        public override CellZygote OnProcessCellZygote(int type)
        {
            var zygote = System.DataSource.GetCellZygote(type);
            var obj = zygote.gameObject;
            obj.SetActive(true);

            // 等比缩放
            var sizeDelta = zygote.sizeDelta;
            var cellHeight = (System.Content.rect.height - VerticalSpace * (Row + 1)) / Row;
            var cellWidth = sizeDelta.x / sizeDelta.y * cellHeight;

            if (obj.scene.IsValid()) obj.SetActive(false);

            var value = new Vector2(0f, 1);
            return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
        }

        public override void InitLayoutManger()
        {
            var rect = Delegate.Viewport.rect;
            _edge = rect.width * (MinPoolCoverage - 1) * 0.5f;
            var ds = Delegate.DataSource;
            if (ds.GetCellCount() < 1) return;

            var maxX = rect.width * MinPoolCoverage;
            var count = ds.GetCellCount();
            var zygote = Delegate.GetCellZygote(0);
            var cellHeight = zygote.FixedHeight;
            var cellWidth = zygote.FixedWidth;

            var offset = 0f;
            var i = 0;
            while (i < count && (offset < maxX || i < MinCellCount))
            {
                var cell = PopCachedPoolPoolAndPushToUsedPool(i, cellWidth, cellHeight);
                ds.OnBindCell(cell.Cell.Cell, i);
                i++;
                offset = cell.Offset;
                if (i % Row == 0) offset += cellWidth;
            }
        }

        public override void OnValueChanged(AbsRecyclableScrollRect scrollRect, Vector2 v)
        {
            var normalizedPosition = scrollRect.horizontalNormalizedPosition;
            var contentWidth = Delegate.Content.rect.width;
            var viewportWidth = Delegate.Viewport.rect.width;
            if (contentWidth <= viewportWidth) return;

            normalizedPosition = Mathf.Clamp(normalizedPosition, 0f, 1f);
            var posX = normalizedPosition * (contentWidth - viewportWidth);
            var dir = posX - _prevOffset;

            // 少于最小移动距离则不移动
            if (Mathf.Abs(dir) < MinMovingDistance) return;

            _prevOffset = posX;
            var min = posX - _edge;
            var max = posX + _edge + viewportWidth;

            var zygote = Delegate.GetCellZygote(0);
            var dataSource = Delegate.DataSource;
            var cellHeight = zygote.FixedHeight;
            var cellWidth = zygote.FixedWidth;
            var cellCount = dataSource.GetCellCount();

            // left >> right
            if (dir > 0)
            {
                foreach (var item in Delegate.UsedPool)
                {
                    var tf = item.Cell.Transform;
                    var edge = tf.anchoredPosition.x + tf.rect.width;
                    if (min <= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var right = Delegate.BigIndexFromUsedPool();
                while (right.Index + 1 < cellCount && max >= right.Offset + cellWidth)
                {
                    var index = right.Index + 1;
                    var column = index / Row;
                    while (column == index / Row && index < cellCount)
                    {
                        right = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight);
                        dataSource.OnBindCell(right.Cell.Cell, index);
                        index++;
                    }
                }
            }

            // right >> left
            if (dir < 0)
            {
                for (var i = Delegate.UsedPool.Count - 1; i >= 0; i--)
                {
                    var item = Delegate.UsedPool[i];
                    var tf = item.Cell.Transform;
                    var edge = tf.anchoredPosition.x;
                    if (max >= edge) break;
                    _recyclingPool.Add(item);
                }

                RecyclingCellFromList();

                var left = Delegate.LittleIndexFromUsedPool();
                while (min <= left.Offset && left.Index > 0)
                {
                    var index = left.Index - 1;
                    var column = index / Row;
                    while (column == index / Row && index >= 0)
                    {
                        left = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight);
                        dataSource.OnBindCell(left.Cell.Cell, index);
                        index--;
                    }
                }
            }
        }

        public override void OnDataSetChanged(AbsRecyclableScrollRect scrollRect)
        {
            OnScrollToNormalizedPosition(scrollRect, scrollRect.horizontalNormalizedPosition);
        }

        public override void OnScrollToNormalizedPosition(AbsRecyclableScrollRect scrollRect, float normalized)
        {
            var dataSource = Delegate.DataSource;
            var contentWidth = Delegate.Content.rect.width;
            var viewportWidth = Delegate.Viewport.rect.width;
            var posX = 0f;
            if (contentWidth > viewportWidth)
            {
                normalized = Mathf.Clamp(normalized, 0f, 1f);
                posX = normalized * (contentWidth - viewportWidth);
            }

            _prevOffset = posX;
            var min = posX - _edge;
            var max = posX + _edge + viewportWidth;

            var zygote = Delegate.GetCellZygote(0);
            var cellHeight = zygote.FixedHeight;
            var cellWidth = zygote.FixedWidth;

            foreach (var item in Delegate.UsedPool)
            {
                var tf = item.Cell.Transform;
                var edge = tf.anchoredPosition.x + tf.rect.width;
                if (min <= edge) break;
                _recyclingPool.Add(item);
            }

            RecyclingCellFromList();

            for (var i = Delegate.UsedPool.Count - 1; i >= 0; i--)
            {
                var item = Delegate.UsedPool[i];
                var tf = item.Cell.Transform;
                var edge = tf.anchoredPosition.x;
                if (max >= edge) break;
                _recyclingPool.Add(item);
            }

            RecyclingCellFromList();

            var existMinIndex = int.MaxValue;
            var existMaxIndex = int.MinValue;
            if (Delegate.UsedPool.Count > 0)
            {
                existMinIndex = Delegate.LittleIndexFromUsedPool().Index;
                existMaxIndex = Delegate.BigIndexFromUsedPool().Index;
            }

            var minCol = Mathf.Max((int) (min / cellWidth), 0);
            var offset = cellWidth * minCol;
            var index = minCol * Row;

            var cellCount = dataSource.GetCellCount();
            while (index < cellCount && max >= offset + cellWidth)
            {
                // 填充行
                var col = index / Row;
                while (col == index / Row && index < cellCount)
                {
                    if (existMinIndex <= index && existMaxIndex >= index)
                    {
                        index++;
                        var curCol = index / Row;
                        offset = curCol * cellWidth;
                        continue;
                    }

                    var cur = PopCachedPoolPoolAndPushToUsedPool(index, cellWidth, cellHeight);
                    dataSource.OnBindCell(cur.Cell.Cell, index);
                    offset = Mathf.Abs(cur.Offset);
                    index++;
                }
            }

            scrollRect.horizontalNormalizedPosition = normalized;
        }

        #endregion

        #region 私有方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CellItem PopCachedPoolPoolAndPushToUsedPool(int index, float cellWidth, float cellHeight)
        {
            var cell = Delegate.PopFromCachedPool(index);
            var tf = cell.Transform;
            var ap = CalcCellOffset(index, cellWidth, cellHeight);
            tf.anchoredPosition = ap;
            var cur = new CellItem(index, ap.x, cell);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2 CalcCellOffset(int index, float cellWidth, float cellHeight)
        {
            var col = index / Row;
            return new Vector2(
                cellWidth * col + (col + 1) * HorizontalSpace,
                -(index % Row * (cellHeight + VerticalSpace) + VerticalSpace)
            );
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