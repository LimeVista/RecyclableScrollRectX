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

namespace RecyclableScrollRectX.Samples
{
    [CreateAssetMenu(menuName = "Samples/GridDomeDataSource")]
    public class GridDataSourceSamples : RecyclableScriptableDataSource
    {
        public override bool SingleZygoteMode => true;

        public override int GetCellCount() => 100;

        public override RectTransform GetCellZygote(int type)
        {
            const string path = "Prefabs/DemoCell03";
            return Resources.Load<GameObject>(path).GetComponent<RectTransform>();
        }

        public override int GetCellType(int index) => 0;

        public override void OnBindCell(RecyclingSystem.ICell cell, int index)
        {
            if (cell is DemoCell dc)
            {
                dc.OnBind(index);
            }
        }
    }
}