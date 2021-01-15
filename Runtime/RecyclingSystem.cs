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
using System.Collections;
using UnityEngine;

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 列表资源回收系统
    /// </summary>
    public abstract class RecyclingSystem
    {
        #region 接口

        /// <summary>
        /// 数据源
        /// </summary>
        public interface IDataSource
        {
            /// <summary>
            /// 单元格有单原型（Zygote）模式
            /// 这种情况下决定是否有且仅有一个类型 <see cref="GetCellType"/>
            /// </summary>
            bool SingleZygoteMode { get; }

            /// <summary>
            /// 单元格个数
            /// </summary>
            int GetCellCount();

            /// <summary>
            /// 获取单元格细胞原型，一般情况下为 预制体（prefab），且未执行 GameObject.Instantiate()
            /// 系统会对此数据返回结果进行缓存
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>预制体</returns>
            RectTransform GetCellZygote(int type);

            /// <summary>
            /// 获取单元格类型
            /// <see cref="SingleZygoteMode"/> 开启时，请返回固定值
            /// </summary>
            /// <param name="index">索引</param>
            /// <returns>单元格类型</returns>
            int GetCellType(int index);

            /// <summary>
            /// 绑定单元格
            /// </summary>
            /// <param name="cell">单元格</param>
            /// <param name="index">索引</param>
            void OnBindCell(ICell cell, int index);
        }

        /// <summary>
        /// 单元格
        /// </summary>
        public interface ICell
        {
            // 单元格接口
        }

        #endregion

        #region 公开成员、属性

        /// <summary>
        /// 数据源
        /// </summary>
        public IDataSource DataSource
        {
            get => Delegate.DataSource;
            protected set => Delegate.DataSource = value;
        }

        /// <summary>
        /// 视口
        /// </summary>
        public RectTransform Viewport
        {
            get => Delegate.Viewport;
            protected set => Delegate.Viewport = value;
        }

        /// <summary>
        /// 内容区域
        /// </summary>
        public RectTransform Content
        {
            get => Delegate.Content;
            protected set => Delegate.Content = value;
        }

        /// <summary>
        /// 布局管理器
        /// </summary>
        public LayoutManager LayoutManager
        {
            get => Delegate.LayoutManager;
            protected set => Delegate.LayoutManager = value;
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 使用协程初始化
        /// </summary>
        /// <param name="onInitialized">初始化完成回调</param>
        /// <returns>协程发生器</returns>
        public abstract IEnumerator InitCoroutine(Action onInitialized);

        /// <summary>
        /// <see cref="AbsRecyclableScrollRect.DataSetChanged()"/>
        /// 如需使用，请调用 <see cref="AbsRecyclableScrollRect.DataSetChanged()"/>
        /// </summary>
        /// <param name="scrollRect">滑动列表</param>
        public abstract void OnDataSetChanged(AbsRecyclableScrollRect scrollRect);

        /// <summary>
        /// <see cref="AbsRecyclableScrollRect.ScrollToNormalizedPosition(float)"/>
        ///  如需使用，请调用 <see cref="AbsRecyclableScrollRect.ScrollToNormalizedPosition(float)"/>
        /// </summary>
        /// <param name="scrollRect">滑动列表</param>
        /// <param name="normalized">基准位置</param>
        public abstract void OnScrollToNormalizedPosition(AbsRecyclableScrollRect scrollRect, float normalized);

        #endregion

        #region 保护成员、属性

        /// <summary>
        /// 数据持有者
        /// </summary>
        protected readonly RecyclingSystemDelegate Delegate = new RecyclingSystemDelegate();

        #endregion

        #region 内部成员、属性

        /// <summary>
        /// 内部成员访问代理
        /// </summary>
        internal RecyclingSystemDelegate InternalDelegate => Delegate;

        #endregion
    }
}