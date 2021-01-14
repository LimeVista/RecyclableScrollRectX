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