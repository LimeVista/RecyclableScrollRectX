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
    /// 列表资源回收系统代理类
    /// </summary>
    public sealed class RecyclingSystemDelegate : IComparer<CellItem>
    {
        /// <summary>
        /// 已使用的单元格，此列表总是以索引从小到大排序
        /// </summary>
        public readonly List<CellItem> UsedPool = new List<CellItem>();

        /// <summary>
        /// 内容区域
        /// </summary>
        public RectTransform Content;

        /// <summary>
        /// 视口
        /// </summary>
        public RectTransform Viewport;

        /// <summary>
        /// 数据源
        /// </summary>
        public RecyclingSystem.IDataSource DataSource;

        /// <summary>
        /// 布局管理器
        /// </summary>
        public LayoutManager LayoutManager;

        /// <summary>
        /// 返回索引小端的单元格
        /// </summary>
        /// <returns>单元格</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellItem LittleIndexFromUsedPool() => UsedPool.Count > 0 ? UsedPool[0] : CellItem.Zero;

        /// <summary>
        /// 返回索引大端的单元格
        /// </summary>
        /// <returns>单元格</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellItem BigIndexFromUsedPool() => UsedPool.Count > 0 ? UsedPool[UsedPool.Count - 1] : CellItem.Zero;

        /// <summary>
        /// 缓存池个数
        /// </summary>
        public int CachedPoolCount => CachedPool.Count;

        /// <summary>
        /// 单元格原型（预制体）目前个数
        /// </summary>
        public int ZygotesCount => Zygotes.Count;

        /// <summary>
        /// 缓存池（未使用的单元格）
        /// </summary>
        internal readonly List<CellExt> CachedPool = new List<CellExt>();

        /// <summary>
        /// 单元格原型（预制体）
        /// </summary>
        internal readonly Dictionary<int, CellZygote> Zygotes = new Dictionary<int, CellZygote>();
        
        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            foreach (var item in UsedPool)
            {
                Object.Destroy(item.Cell.Transform.gameObject);
            }

            UsedPool.Clear();
            
            foreach (var ext in CachedPool)
            {
                Object.Destroy(ext.Transform.gameObject);
            }

            CachedPool.Clear();
            Zygotes.Clear();
        }

        /// <summary>
        /// 获取原型（预制体）
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>原型（预制体）</returns>
        public CellZygote GetCellZygote(int index)
        {
            var type = DataSource.GetCellType(index);
            if (!Zygotes.TryGetValue(type, out var value))
            {
                value = LayoutManager.OnProcessCellZygote(type);
                Zygotes[type] = value;
            }

            return value;
        }

        /// <summary>
        /// 将单元格从缓存池中退栈
        /// </summary>
        /// <param name="index">绑定索引</param>
        /// <returns>单元格扩展结构</returns>
        public CellExt PopFromCachedPool(int index)
        {
            var cell = CellExt.Zero;
            var type = DataSource.GetCellType(index);
            var count = CachedPool.Count;
            var i = 0;

            for (; i < count; i++)
            {
                cell = CachedPool[i];
                if (cell.Type != type) continue;
                break;
            }

            if (count > 0 && i < count)
            {
                CachedPool.RemoveAt(i);
            }
            else
            {
                var zygote = GetCellZygote(index);
                cell = CreateCell(zygote);
                cell.Transform.SetParent(Content, false);
            }

            return cell;
        }

        /// <summary>
        /// 将单元格置入已使用标记池
        /// </summary>
        /// <param name="item">单元格扩展结构</param>
        public void PushToUsedPool(CellItem item)
        {
#if RECYCLABLE_SCROLL_RECT_TEST && UNITY_EDITOR
            item.Cell.Transform.gameObject.name = $"Cell-{item.Index:00}(使用中)";
#endif
            var index = UsedPool.BinarySearch(item, this);
            if (index < 0)
            {
                UsedPool.Insert(~index, item);
            }
        }

        /// <summary>
        /// 回收单元格
        /// </summary>
        /// <param name="item">单元格扩展结构</param>
        public void MoveToCachedPool(CellItem item)
        {
#if RECYCLABLE_SCROLL_RECT_TEST && UNITY_EDITOR
            item.Cell.Transform.gameObject.name = "Cell-(已经回收)";
#endif
            UsedPool.Remove(item);
            CachedPool.Add(item.Cell);
        }

        public int Compare(CellItem x, CellItem y) => x.Index.CompareTo(y.Index);

        /// <summary>
        /// 构建单元格
        /// </summary>
        /// <param name="zygote">单元格原型（预制体）</param>
        /// <returns>单元格扩展结构</returns>
        private static CellExt CreateCell(CellZygote zygote)
        {
            var cellObj = Object.Instantiate(zygote.Zygote.gameObject);
            var cell = cellObj.GetComponent<RectTransform>();
            cellObj.name = "Cell";
            cell.anchoredPosition = Vector2.zero;
            cell.anchorMin = zygote.AnchorMin;
            cell.anchorMax = zygote.AnchorMax;
            cell.pivot = zygote.Pivot;
            cell.sizeDelta = new Vector2(zygote.FixedWidth, zygote.FixedHeight);
            cellObj.SetActive(true);
            return new CellExt(cell, zygote.Type);
        }
    }
}