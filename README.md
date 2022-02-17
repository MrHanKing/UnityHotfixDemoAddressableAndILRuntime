# UnityHotfixDemoAddressableAndILRuntime

一个基于 ET 框架 4.0 版本的热更演示 Demo。热更解决方案使用 Addressable 和 ILRuntime

# 仓库注意事项

1. 这个仓库只是为了演示 Addressable 和 ILRuntime 的热更解决方案以及相关的部分插件的植入。其余部分建议不要直接植入或使用。
2. 这是一个已上线项目删除了内容部分只留下热更部分做演示。所以使用的 ET 框架不是完整的。
3. 这套热更方案在线上稳定跑的版本是:Unity2019LTS + Addressable 1.17.17 + ILRuntime 内置。这个配套是稳定的 如果要换用其他版本可能会碰到一些小坑，请自行解决～
4. 提供的功能演示包括: 资源和代码热更新、子包加载更新、大版本强制更新、下载地址自定义、资源管理。
5. 暂未探索的:分 catalog 的热更方式暂没研究，如果有解决方案的大佬希望能告知～ Thanks
6. 部分耦合暂时没处理，等空下来了再来处理 >.<!
7. 需要了解 Unity 官方 Addressable 使用，这里不赘述。
8. Model 层代码不能热更、Hotfix 代码可以热更。详情了解 ILRuntime 机制

# 逻辑概述

- 流程: Model 层 Init -> 大版本检查 -> 主包资源和代码更新 -> 加载 Hotfix 层代码（这里开始都是可热更代码） -> Hotfix 层 Init -> 按需可能在 app 不同阶段加载子包资源
- 关键代码脚本:
  - Model 层配置文件和大版本检查: GlobalConfigComponent.cs
  - Model 层更新代码接口: AddressablesHelper.cs
  - 主包热更 UI 逻辑: UIUpdateBundleComponent.cs
  - 资源管理（统一加载和释放）: AddressableComponent.cs
  - 子包热更: AddressableDownloadSystem.cs
  - 资源寻址劫持: AddressableTools.cs

# 内容

## 大版本强制更新和配置文件管理

- 重要的是 GlobalConfigComponent.cs 文件内的配置。

```
public class GlobalConfigComponent : Component
{
    // 配置的远程配置路径 唯一写死的网络配置的路径 不可热更
    // 这里放置配置的远程路径
    private static string configUrl = "";
}
```

- 从这里读取远程配置后比较版本确认是否覆盖本地配置以实现配置的热更新，其他所有配置读取都从更新完的配置中读取。
- 配置的格式可以自行定义修改。现存仓库里的配置有 2 个 key 比较重要，hotfixUrl 和 needClientVersion

```
public GlobalConfigType defaultConfig { get; private set; } = new GlobalConfigType()
        {
            version = "1.0.0",
            hotfixUrl = "http://10.1.2.161:8080",
            needClientVersion = "1.0",
            iosUrl = "",
            androidUrl = "",
            pcUrl = "",
        };
```

- 如上代码所示 hotfixUrl 会确定热更资源所在的服务器地址。我这里放了本地的 ip 地址服务做测试。当 config 被远程的所覆盖则 hotfixUrl 也会被覆盖，以此实现热更地址变化。
- needClientVersion 默认会带出去一个版本。它描述进入 app 所需要的最低版本是多少。假设带出去的是 1.0，而远端的配置是 1.1，那么就会触发强制更新弹窗，根据平台对应的 url 配置跳转对应商店如这里的 iosUrl 可以配置成 ios 平台的下载链接。

## 下载地址自定义

- Addressable 地址的替换的实现逻辑在 AddressableTools.cs 中。原理是替换 Addressable 资源寻址的函数。把所有 http 带头的网络资源寻址转换为配置中 hotfixUrl 所指向的地址。
- 仓库内现在所有热更新资源都生成在根目录的 HotFixData 文件夹内，只要把此文件夹内的所有资源放到 hotfixUrl 所指向的地址内便可以实现热更。比如我本地测试 hotfixUrl 配置了 http://10.1.2.161:8080，我直接把HotFixData文件夹开放为 http://10.1.2.161:8080 服务便可以进行局域网的热更新测试了。

## 资源和代码热更新

- 通过一个"Preload"的主包 label 来确认主包资源和代码。
- 详细逻辑查看 UIUpdateBundleComponent.cs 文件中的 DownLoadMainBundles 函数。

## 子包加载更新

- 子包的 group 配置是以远程包的方式配置的，即对应 Assets/AddressableAssetsData/AssetGroupTemplates/HotfixPacked.asset 配置
- Assets/AddressableAssetsData/AssetGroupTemplates/HotfixVideos.asset 配置特殊，给视频远程包用。因为视频不能压缩，不然安卓平台解码器可能会解不出来。
- 子包的下载调用通过在热更层调用 AddressableDownloadSystem.cs 脚本的 DownLoadCourse 函数下载，传入的参数是 Addressable 系统的 label。所以总的来说是你的资源 lable 如何管理。

## 资源 label 管理

- 加入了第三方插件
  unity-addressable-importer（https://github.com/favoyang/unity-addressable-importer/blob/master/Documentation~/AddressableImporter.md）
- 用规则的方式来委托管理你的资源分组和 Label 减少人力。
- 具体可以查看上面插件官方文档和仓库内 Assets/AddressableAssetsData/AddressableImportSettings.asset 的配置来参考如何使用。
