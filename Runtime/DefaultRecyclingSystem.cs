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

using System;
using System.Collections;
using UnityEngine;

namespace RecyclableScrollRectX
{
    public class DefaultRecyclingSystem : RecyclingSystem
    {
        public DefaultRecyclingSystem(
            LayoutManager manager,
            IDataSource dataSource,
            RectTransform viewport,
            RectTransform content
        )
        {
            manager.System = this;
            LayoutManager = manager;
            DataSource = dataSource;
            Viewport = viewport;
            Content = content;
        }

        public override IEnumerator InitCoroutine(Action onInitialized)
        {
            yield return null;  // 等待一帧，保证获取到宽高
            
            if (LayoutManager.Direction == Direction.Vertical)
                Content.AnchorToTop();
            else
                Content.AnchorToLeft();

            Content.anchoredPosition = Vector3.zero;
            
            // yield return null;  // 等待一帧，确保生效

            var contentSize = LayoutManager.CalcContentLengthOfSlidingDirection();
            var sizeDelta = Content.sizeDelta;
            sizeDelta = LayoutManager.Direction == Direction.Vertical
                ? new Vector2(sizeDelta.x, contentSize)
                : new Vector2(contentSize, sizeDelta.y);

            Content.sizeDelta = sizeDelta;

            LayoutManager.InitLayoutManger();

            onInitialized?.Invoke();
        }
    }
}