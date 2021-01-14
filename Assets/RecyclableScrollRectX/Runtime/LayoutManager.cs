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

namespace RecyclableScrollRectX
{
    /// <summary>
    /// 布局管理器
    /// </summary>
    public abstract class LayoutManager
    {
        /// <summary>
        /// <see cref="RecyclingSystem"/>
        /// </summary>
        public virtual RecyclingSystem System { get; set; }

        /// <summary>
        /// 最小覆盖率 viewPort * MinPoolCoverage
        /// </summary>
        public virtual float MinPoolCoverage => 1.64f;

        /// <summary>
        /// <see cref="RecyclingSystemDelegate"/>
        /// </summary>
        protected RecyclingSystemDelegate Delegate => System.InternalDelegate;

        /// <summary>
        /// 计算内容在滑动方向的长度，此接口开销不小，请勿频繁请求
        /// </summary>
        /// <returns>长度</returns>
        public abstract float CalcContentLengthOfSlidingDirection();

        /// <summary>
        /// 计算单元格位置偏移，此接口的开销不定，不建议频繁请求
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="index">单元格索引</param>
        /// <returns></returns>
        public abstract Vector2 CalcCellOffset(RecyclingSystem.ICell cell, int index);

        /// <summary>
        /// 预处理单元格原型（预制体）
        /// </summary>
        /// <param name="type">单元格（预制体）类型</param>
        /// <returns>单元格原型（预制体）</returns>
        public abstract CellZygote OnProcessCellZygote(int type);

        /// <summary>
        /// 滑动方向
        /// </summary>
        public abstract Direction Direction { get; }

        /// <summary>
        /// 初始化单元格
        /// </summary>
        public abstract void InitLayoutManger();

        /// <summary>
        /// 变化监听 <see cref="UnityEngine.UI.ScrollRect.onValueChanged"/>
        /// </summary>
        /// <param name="scrollRect">滑动部件</param>
        /// <param name="v"><see cref="UnityEngine.UI.ScrollRect.onValueChanged"/></param>
        public abstract void OnValueChanged(AbsRecyclableScrollRect scrollRect, Vector2 v);
    }
}