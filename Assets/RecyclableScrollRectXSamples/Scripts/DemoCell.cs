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

namespace RecyclableScrollRectX.Samples
{
    public class DemoCell : MonoBehaviour, RecyclingSystem.ICell
    {
        [SerializeField] private Text title;
        [SerializeField] private Text subTitle;

        public void OnBind(int index)
        {
            title.text = $"Title-{index:000}";
            subTitle.text = $"{index:000}";
        }
    }
}