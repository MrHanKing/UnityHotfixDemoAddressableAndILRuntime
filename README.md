# UnityHotfixDemoAddressableAndILRuntime

一个基于 ET 框架 4.0 版本的热更演示 Demo。热更解决方案使用 Addressable 和 ILRuntime

# 仓库注意事项

1. 这个仓库只是为了演示 Addressable 和 ILRuntime 的热更解决方案以及相关的部分插件的植入。其余部分建议不要直接植入或使用。
2. 这是一个已上线项目删除了内容部分只留下热更部分做演示。所以使用的 ET 框架不是完整的。
3. 这套热更方案在线上稳定跑的版本是:Unity2019LTS + Addressable 1.17.17 + ILRuntime 内置。这个配套是稳定的 如果要换用其他版本可能会碰到一些小坑，请自行解决～
4. 提供的功能包括: 资源和代码热更新、子包加载更新、大版本强制更新、下载地址自定义、资源管理。
5. 暂未探索的:分 catalog 的热更方式暂没研究，如果有解决方案的大佬希望能告知～ Thanks
6. 部分耦合暂时没处理，等空下来了再来处理 >.<!
7. 需要了解 Unity 官方 Addressable 使用，这里不赘述。
8. Model 层代码不能热更、Hotfix 代码可以热更。详情了解 ILRuntime 机制

# 逻辑概述

- 流程: Model 层 Init -> 大版本检查 -> 主包资源和代码更新 -> 加载 Hotfix 层代码 -> Hotfix 层 Init -> 按需可能在 app 不同阶段加载子包资源
- 关键代码脚本:
  - Model 层配置文件和大版本检查: GlobalConfigComponent.cs
  - Model 层更新代码接口: AddressablesHelper.cs
  - 主包热更 UI 逻辑: UIUpdateBundleComponent.cs
  - 资源管理: AddressableComponent.cs
  - 子包热更: AddressableDownloadSystem.cs
  - 资源寻址劫持: AddressableTools.cs

# 内容
