# Recyclable Scroll Rect X
A recyclable scroll view for Unity.  
一个用于 Unity 的复用性滑动列表。

## 特性
* 支持多种布局模式，水平滑动、垂直滑动、网格模式
* `UI` 布局复用
* 数据源加载使用适配器模式
* 支持列表跳转

## 引入
* Unity 2019.3+
* 替换下面链接 `ver` 为你选择的版本
* `UPM` -> `https://github.com/LimeVista/RecyclableScrollRectX.git#ver`

## 使用

* 使用 `UI` -> `Recyclable Scroll View` 创建 `Recyclable Scroll View`
* 创建单元格 `prefabs` 用于展示，如 [`DemoCell03`](./Assets/RecyclableScrollRectXSamples/Resources/Prefabs/DemoCell03.prefab)
* 创建单元格类，如 [`DemoCell`](./Assets/RecyclableScrollRectXSamples/Scripts/DemoCell.cs)，并绑定到 `DemoCell03`
* 创建数据源类，如 [`GridDataSourceSamples`](./Assets/RecyclableScrollRectXSamples/Scripts/GridDataSourceSamples.cs)
* 将数据源制作为 `ScriptableObject`， 如 [`GridDataSourceSamples.asset`](./Assets/RecyclableScrollRectXSamples/ScriptableObject/GridDataSourceSamples.asset)
* 将数据源绑定到 `Recyclable Scroll View`
* 详情使用参考 [`RecyclableScrollRectXSamples`](./Assets/RecyclableScrollRectXSamples)

## 核心类
* [`RecyclingSystem`](./Assets/RecyclableScrollRectX/Runtime/RecyclingSystem.cs)：整个复用性滑动列表的核心抽象。
* [`RecyclableScriptableDataSource`](./Assets/RecyclableScrollRectX/Runtime/RecyclableScriptableDataSource.cs)：`IDataSource` 的 `ScriptableObject` 版本，便于在编辑器快速绑定。
* [`RecyclableScrollRect`](./Assets/RecyclableScrollRectX/Runtime/RecyclableScrollRect.cs)：复用性滑动列表 `UI` 承载。
* [`LayoutManager`](./Assets/RecyclableScrollRectX/Runtime/LayoutManager.cs)：布局系统，其中拥有默认实现 `GridHorizontalLayoutManager`、`GridVerticalLayoutManager`、`VerticalLayoutManager`、`HorizontalLayoutManager`。