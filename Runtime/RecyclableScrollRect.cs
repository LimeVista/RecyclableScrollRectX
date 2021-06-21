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
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 本项目的终极目标，可循环系统列表
    /// </summary>
    public class RecyclableScrollRect : AbsRecyclableScrollRect
    {
        /// <summary>
        /// 布局模式
        /// </summary>
        public RecyclableScrollRectLayoutMode LayoutMode
        {
            get => layoutMode;
            set => layoutMode = value;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        public RecyclingSystem.IDataSource DataSource
        {
            get => _dataSource ?? dataSource;
            set
            {
                if ((value == _dataSource && _dataSource != null) ||
                    (value is RecyclableScriptableDataSource v && v == dataSource))
                {
                    return;
                }

                if (value is RecyclableScriptableDataSource rv)
                {
                    _dataSource = null;
                    dataSource = rv;
                }
                else
                {
                    dataSource = null;
                    _dataSource = value;
                }

                DataSetChanged();
            }
        }


        public override void DataSetChanged()
        {
            if (RecyclingSystem == null) return;
            RecyclingSystem.OnDataSetChanged(this);
        }

        public override void ScrollToNormalizedPosition(float normalized)
        {
            if (RecyclingSystem == null) return;
            RecyclingSystem.OnScrollToNormalizedPosition(this, normalized);
        }

        public override void RefreshVisibleCells()
        {
            RecyclingSystemDelegate @delegate;
            if (RecyclingSystem == null || (@delegate = RecyclingSystem.InternalDelegate) == null) return;

            foreach (var item in @delegate.UsedPool)
            {
                @delegate.DataSource.OnBindCell(item.Cell.Cell, item.Index);
            }
        }

        protected override bool AllowHorizontal
        {
            get
            {
                if (RecyclingSystem != null)
                {
                    return RecyclingSystem.LayoutManager.Direction == Direction.Horizontal;
                }

                switch (layoutMode)
                {
                    case RecyclableScrollRectLayoutMode.Horizontal: return true;
                    case RecyclableScrollRectLayoutMode.Vertical: return false;
                    case RecyclableScrollRectLayoutMode.GridVertical: return false;
                    case RecyclableScrollRectLayoutMode.GridHorizontal: return true;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override bool AllowVertical
        {
            get
            {
                if (RecyclingSystem != null)
                {
                    return RecyclingSystem.LayoutManager.Direction == Direction.Vertical;
                }

                switch (layoutMode)
                {
                    case RecyclableScrollRectLayoutMode.Horizontal: return false;
                    case RecyclableScrollRectLayoutMode.Vertical: return true;
                    case RecyclableScrollRectLayoutMode.GridVertical: return true;
                    case RecyclableScrollRectLayoutMode.GridHorizontal: return false;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override RecyclingSystem CreateRecyclingSystem()
        {
            LayoutManager manager;
            switch (layoutMode)
            {
                case RecyclableScrollRectLayoutMode.Horizontal:
                    manager = new HorizontalLayoutManager(horizontalSpace, verticalSpace);
                    break;

                case RecyclableScrollRectLayoutMode.Vertical:
                    manager = new VerticalLayoutManager(horizontalSpace, verticalSpace);
                    break;

                case RecyclableScrollRectLayoutMode.GridVertical:
                    manager = new GridVerticalLayoutManager(
                        Math.Max(1, orthogonalCount),
                        horizontalSpace,
                        verticalSpace
                    );
                    break;

                case RecyclableScrollRectLayoutMode.GridHorizontal:
                    manager = new GridHorizontalLayoutManager(
                        Math.Max(1, orthogonalCount),
                        horizontalSpace,
                        verticalSpace
                    );
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new DefaultRecyclingSystem(manager, DataSource, viewport, content);
        }

        #region 编辑器属性

        [SerializeField] private RecyclableScrollRectLayoutMode layoutMode;
        [SerializeField] private RecyclableScriptableDataSource dataSource;
        [SerializeField] [HideInInspector] private int orthogonalCount;
        [SerializeField] [HideInInspector] private float horizontalSpace;
        [SerializeField] [HideInInspector] private float verticalSpace;

        #endregion

        #region 内部属性

        private RecyclingSystem.IDataSource _dataSource;

        #endregion
    }
}