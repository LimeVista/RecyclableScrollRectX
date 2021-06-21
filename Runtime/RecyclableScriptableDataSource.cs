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
    /// 可序列化列表数据源
    /// </summary>
    public abstract class RecyclableScriptableDataSource : ScriptableObject, RecyclingSystem.IDataSource
    {
        /// <summary>
        /// <see cref="RecyclingSystem.IDataSource.SingleZygoteMode"/>
        /// </summary>
        public abstract bool SingleZygoteMode { get; }

        /// <summary>
        /// <see cref="RecyclingSystem.IDataSource.GetCellCount"/>
        /// </summary>
        public abstract int GetCellCount();

        /// <summary>
        /// <see cref="RecyclingSystem.IDataSource.GetCellZygote"/>
        /// </summary>
        public abstract RectTransform GetCellZygote(int type);

        /// <summary>
        /// <see cref="RecyclingSystem.IDataSource.GetCellType"/>
        /// </summary>
        public abstract int GetCellType(int index);

        /// <summary>
        /// <see cref="RecyclingSystem.IDataSource.OnBindCell"/>
        /// </summary>
        public abstract void OnBindCell(RecyclingSystem.ICell cell, int index);
    }
}