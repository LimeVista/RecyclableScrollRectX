﻿//
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 水平布局管理器
    /// </summary>
    public class HorizontalLayoutManager : LayoutManager
    {
        public HorizontalLayoutManager(float leftPadding, float rightPadding)
        {
            LeftPadding = leftPadding;
            RightPadding = rightPadding;
        }

        /// <summary>
        /// 左部空间间隔
        /// </summary>
        public float LeftPadding { get; }

        /// <summary>
        /// 右部空间间隔
        /// </summary>
        public float RightPadding { get; }

        #region 抽象实现

        public override Direction Direction => Direction.Horizontal;

        public override float CalcContentLengthOfSlidingDirection()
        {
            var ds = Delegate.DataSource;
            var count = ds.GetCellCount();
            if (count < 1) return 0f;
            // count = index + 1
            var offset = CalcCellOffsetOfSlidingDirection(count);
            return offset + LeftPadding + RightPadding;
        }

        public override Vector2 CalcCellOffset(RecyclingSystem.ICell cell, int index)
        {
            if (index < 0) return Vector2.zero;
            return new Vector2(CalcCellOffsetOfSlidingDirection(index) + LeftPadding, 0);
        }

        public override CellZygote OnProcessCellZygote(int type)
        {
            var zygote = System.DataSource.GetCellZygote(type);
            var obj = zygote.gameObject;
            obj.SetActive(true);

            // 等比缩放
            var sizeDelta = zygote.sizeDelta;
            var cellHeight = System.Content.rect.height;
            var cellWidth = sizeDelta.x / sizeDelta.y * cellHeight;

            if (obj.scene.IsValid()) obj.SetActive(false);

            var value = new Vector2(0, 0.5f);
            return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
        }

        public override void InitLayoutManger()
        {
            var rect = Delegate.Viewport.rect;
            _edge = rect.width * (MinPoolCoverage - 1) * 0.5f;
            var ds = Delegate.DataSource;
            if (ds.GetCellCount() < 1) return;

            var maxX = rect.width * MinPoolCoverage;
            var offset = LeftPadding;
            var i = 0;

            var count = ds.GetCellCount();
            while (i < count && (offset < maxX || i < MinCellCount))
            {
                var cell = Delegate.PopFromCachedPool(i);
                var ap = cell.Transform.anchoredPosition;
                ap.x = offset;
                cell.Transform.anchoredPosition = ap;
                var item = new CellItem(i, ap.x, cell);
                Delegate.PushToUsedPool(item);
                ds.OnBindCell(cell.Cell, i);
                offset += cell.Transform.rect.width;
                i++;
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
                if (right.Index + 1 < Delegate.DataSource.GetCellCount() &&
                    max >= right.Offset + right.CalcWidth())
                {
                    BindCell(right, Mathf.Abs(dir), false);
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
                if (min <= left.Offset && left.Index > 0)
                {
                    BindCell(left, Mathf.Abs(dir), true);
                }
            }
        }

        public override void OnDataSetChanged(AbsRecyclableScrollRect scrollRect)
        {
            OnScrollToNormalizedPosition(scrollRect, scrollRect.horizontalNormalizedPosition);
        }

        public override void OnScrollToNormalizedPosition(AbsRecyclableScrollRect scrollRect, float normalized)
        {
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

            foreach (var item in Delegate.UsedPool)
            {
                var tf = item.Cell.Transform;
                var edge = Mathf.Abs(tf.anchoredPosition.y) + tf.rect.height;
                if (min <= edge) break;
                _recyclingPool.Add(item);
            }

            RecyclingCellFromList();

            for (var i = Delegate.UsedPool.Count - 1; i >= 0; i--)
            {
                var item = Delegate.UsedPool[i];
                var tf = item.Cell.Transform;
                var edge = Mathf.Abs(tf.anchoredPosition.y);
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

            var count = Delegate.DataSource.GetCellCount();
            float offset = LeftPadding, width;
            for (var i = 0; i < count; i++, offset += width)
            {
                width = Delegate.GetCellZygote(i).FixedWidth;
                if (min > offset + width) continue;
                if (max < offset) break;
                if (existMinIndex <= i && existMaxIndex >= i) continue;

                var cell = Delegate.PopFromCachedPool(i);
                var tf = cell.Transform;
                var ap = tf.anchoredPosition;
                ap.x = offset;
                tf.anchoredPosition = ap;
                var cur = new CellItem(i, ap.x, cell);
                Delegate.PushToUsedPool(cur);
                Delegate.DataSource.OnBindCell(cell.Cell, i);
            }

            scrollRect.horizontalNormalizedPosition = normalized;
        }

        #endregion

        #region 私有方法

        private void BindCell(CellItem item, float distance, bool littleIndex)
        {
            var dataSource = Delegate.DataSource;
            var o = 0f;
            var prev = item;

            while (o <= distance && (littleIndex ? prev.Index > 0 : prev.Index + 1 < dataSource.GetCellCount()))
            {
                float width;
                int index;
                Vector2 ap;
                CellExt cell;
                RectTransform tf;

                if (littleIndex)
                {
                    index = prev.Index - 1;
                    cell = Delegate.PopFromCachedPool(index);
                    tf = cell.Transform;
                    width = tf.rect.width;
                    ap = tf.anchoredPosition;
                    ap.x = prev.Offset - width;
                }
                else
                {
                    index = prev.Index + 1;
                    cell = Delegate.PopFromCachedPool(index);
                    tf = cell.Transform;
                    width = prev.Cell.Transform.rect.width;
                    ap = tf.anchoredPosition;
                    ap.x = prev.Offset + width;
                }

                tf.anchoredPosition = ap;
                var cur = new CellItem(index, ap.x, cell);
                Delegate.PushToUsedPool(cur);

                dataSource.OnBindCell(cell.Cell, index);

                prev = cur;
                o += width;
            }
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

        private float CalcCellOffsetOfSlidingDirection(int index)
        {
            if (index < 1) return 0;
            var ds = Delegate.DataSource;
            if (ds.SingleZygoteMode)
            {
                var zygote = Delegate.GetCellZygote(0);
                return zygote.FixedWidth * index;
            }

            var offset = 0f;
            for (var i = 0; i < index; i++)
            {
                offset += Delegate.GetCellZygote(i).FixedWidth;
            }

            return offset;
        }

        #endregion

        #region 私有属性

        private const float MinMovingDistance = 1f;
        private const int MinCellCount = 3;
        private readonly List<CellItem> _recyclingPool = new List<CellItem>();
        private float _prevOffset;
        private float _edge;

        #endregion
    }
}