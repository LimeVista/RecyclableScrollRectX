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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 垂直布局管理器
    /// </summary>
    public class VerticalLayoutManager : LayoutManager
    {
        #region 抽象实现

        public override Direction Direction => Direction.Vertical;

        public override float CalcContentLengthOfSlidingDirection()
        {
            var ds = Delegate.DataSource;
            var count = ds.GetCellCount();
            if (count < 1) return 0f;
            if (ds.SingleZygoteMode)
            {
                var zygote = Delegate.GetCellZygote(0);
                var rows = count;
                return zygote.FixedHeight * rows;
            }

            var offset = 0f;
            for (var i = 0; i < count; i++)
            {
                offset += Delegate.GetCellZygote(i).FixedHeight;
            }

            return offset;
        }

        public override Vector2 CalcCellOffset(RecyclingSystem.ICell cell, int index)
        {
            if (index < 1) return Vector2.zero;
            var ds = Delegate.DataSource;
            if (ds.SingleZygoteMode)
            {
                var zygote = Delegate.GetCellZygote(0);
                return new Vector2(0, zygote.FixedHeight * index);
            }

            var offset = 0f;
            for (var i = 0; i < index; i++)
            {
                offset += Delegate.GetCellZygote(i).FixedHeight;
            }

            return new Vector2(0, offset);
        }

        public override CellZygote OnProcessCellZygote(int type)
        {
            var zygote = System.DataSource.GetCellZygote(type);
            var obj = zygote.gameObject;
            obj.SetActive(true);

            // 等比缩放
            var sizeDelta = zygote.sizeDelta;
            var cellWidth = System.Content.rect.width;
            var cellHeight = sizeDelta.y / sizeDelta.x * cellWidth;

            if (obj.scene.IsValid()) obj.SetActive(false);
            
            var value = new Vector2(0.5f, 1);
            return new CellZygote(type, cellWidth, cellHeight, value, value, value, zygote);
        }

        public override void InitLayoutManger()
        {
            var rect = Delegate.Viewport.rect;
            _edge = rect.height * (MinPoolCoverage - 1) * 0.5f;
            var ds = Delegate.DataSource;
            if (ds.GetCellCount() < 1) return;

            var maxY = rect.height * MinPoolCoverage;
            var offset = 0f;
            var i = 0;

            var count = ds.GetCellCount();
            while (i < count && (offset < maxY || i < MinCellCount))
            {
                var cell = Delegate.PopFromCachedPool(i);
                var ap = cell.Transform.anchoredPosition;
                ap.y = -offset;
                cell.Transform.anchoredPosition = ap;
                var item = new CellItem(i, ap.y, cell);
                Delegate.PushToUsedPool(item);
                ds.OnBindCell(cell.Cell, i);
                offset += cell.Transform.rect.height;
                i++;
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
                if (bottom.Index + 1 < Delegate.DataSource.GetCellCount() &&
                    max >= Mathf.Abs(bottom.Offset) + bottom.CalcHeight())
                {
                    BindCell(bottom, Mathf.Abs(dir), false);
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
                if (min <= Mathf.Abs(top.Offset) && top.Index > 0)
                {
                    BindCell(top, Mathf.Abs(dir), true);
                }
            }
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
                float height;
                int index;
                Vector2 ap;
                CellExt cell;
                RectTransform tf;

                if (littleIndex)
                {
                    index = prev.Index - 1;
                    cell = Delegate.PopFromCachedPool(index);
                    tf = cell.Transform;
                    height = tf.rect.height;
                    ap = tf.anchoredPosition;
                    ap.y = prev.Offset + height;
                }
                else
                {
                    index = prev.Index + 1;
                    cell = Delegate.PopFromCachedPool(index);
                    tf = cell.Transform;
                    height = prev.Cell.Transform.rect.height;
                    ap = tf.anchoredPosition;
                    ap.y = prev.Offset - height;
                }

                tf.anchoredPosition = ap;
                var cur = new CellItem(index, ap.y, cell);
                Delegate.PushToUsedPool(cur);

                dataSource.OnBindCell(cell.Cell, index);

                prev = cur;
                o += height;
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