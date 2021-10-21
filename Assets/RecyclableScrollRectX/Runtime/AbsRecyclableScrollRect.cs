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

using UnityEngine;
using UnityEngine.UI;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 循环型列表抽象类
    /// </summary>
    public abstract class AbsRecyclableScrollRect : ScrollRect
    {
        /// <summary>
        /// 用于判定是否存在数据源
        /// </summary>
        public interface IWithDataSource
        {
            /// <summary>
            /// 数据源
            /// </summary>
            RecyclingSystem.IDataSource DataSource { get; }
        }

        /// <summary>
        /// <see cref="RecyclingSystem"/>
        /// </summary>
        protected RecyclingSystem RecyclingSystem;

        /// <summary>
        /// 是否允许水平移动
        /// </summary>
        protected abstract bool AllowHorizontal { get; }

        /// <summary>
        /// 是否允许垂直移动
        /// </summary>
        protected abstract bool AllowVertical { get; }

        /// <summary>
        /// 创建 <see cref="RecyclingSystem"/>
        /// </summary>
        /// <returns><see cref="RecyclingSystem"/></returns>
        protected abstract RecyclingSystem CreateRecyclingSystem();

        /// <summary>
        /// 数据源发生大改变，重置整个滑动列表
        /// </summary>
        public abstract void DataSetChanged();

        /// <summary>
        /// 功能类似于 <see cref="ScrollRect.SetNormalizedPosition(float, int)"/>
        /// 区别在于这里只关注滑动方向
        /// </summary>
        /// <param name="normalized">基准位置</param>
        public abstract void ScrollToNormalizedPosition(float normalized);

        /// <summary>
        /// 刷新可见单元（会触发可视视图重新绑定）
        /// 
        /// 用于数据状态发生改变，但数据源未发生增删改
        /// </summary>
        public abstract void RefreshVisibleCells();

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void Initialize()
        {
            onValueChanged.RemoveListener(OnValueChanged);
            StartCoroutine(RecyclingSystem.InitCoroutine(() =>
            {
                // 增加监听
                onValueChanged.AddListener(OnValueChanged);
            }));
        }

        /// <summary>
        /// 初始化各项参数，并且调用 <see cref="Initialize"/>
        /// </summary>
        protected override void Start()
        {
            base.Start();
            vertical = AllowVertical;
            horizontal = AllowHorizontal;

            if (!Application.isPlaying) return;
            RecyclingSystem = CreateRecyclingSystem();
            Initialize();
        }

        /// <summary>
        /// 监听滑动变化 <see cref="ScrollRect.onValueChanged"/>
        /// </summary>
        /// <param name="v"><see cref="ScrollRect.onValueChanged"/></param>
        private void OnValueChanged(Vector2 v)
        {
            // 监听滑动变化
            RecyclingSystem.LayoutManager.OnValueChanged(this, v);
        }
    }
}