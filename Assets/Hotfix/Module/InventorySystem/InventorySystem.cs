using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 需要适配接入自己的物品仓库
/// </summary>
namespace ETHotfix
{
    // public enum InventoryId
    // {
    //     //金币
    //     GoldCoin = 1,
    // }
    // /// <summary>
    // /// 物品掉落结果
    // /// </summary>
    // public class InventoryDropResult
    // {
    //     /// <summary>
    //     /// 物品id
    //     /// </summary>
    //     public InventoryId inventoryId { get; }
    //     /// <summary>
    //     /// 获得数量
    //     /// </summary>
    //     public int getNum { get; }
    //     public InventoryDropResult(InventoryId inventoryId, int getNum)
    //     {
    //         this.inventoryId = inventoryId;
    //         this.getNum = getNum;
    //     }
    // }
    // /// <summary>
    // /// 库存
    // /// </summary>
    // public class InventorySystem
    // {
    //     private static InventoryObject inventory;

    //     public static async Task Init()
    //     {
    //         // 获取服务器数据 生成本地库存
    //         // 没有多库存的需求 登陆的时候初始化唯一份inventory就可以
    //         // var serverResult = new Dictionary<int, int>();
    //         var inventoryUser = GlobalGameObjectComponent.Instance.inventoryUser;
    //         inventory = inventoryUser.inventory;
    //         var csvdatabase = inventory.database;
    //         if (inventory == null)
    //         {
    //             Debug.LogError("没有库存载体");
    //             return;
    //         }

    //         // 接入服务器后要清理
    //         inventory.Clear();
    //         inventory.Init(inventoryUser);

    //         var serverResult = await InventoryRequest.GetUserBackpack();
    //         // 替换成服务器数据
    //         foreach (var serverItem in serverResult)
    //         {
    //             // csvdatabase.ItemObjects.
    //             // 获得item数据
    //             AddAndCheckInventory(serverItem.itemDefId, serverItem.itemCount, serverItem.itemCount);
    //         }
    //     }
    //     /// <summary>
    //     /// 检查并且修正物品的数量
    //     /// </summary>
    //     /// <param name="inventoryId"></param>
    //     /// <param name="amout">增加的数量</param>
    //     /// <param name="serverAllCount">服务器给的所有数据</param>
    //     private static void AddAndCheckInventory(InventoryId inventoryId, int amout, int serverAllCount)
    //     {
    //         AddInventory(inventoryId, amout);
    //         var nowAll = GetInventoryNum(inventoryId);
    //         if (nowAll != serverAllCount)
    //         {
    //             // 强制刷新
    //             inventory?.ForceRefresh((int)inventoryId, serverAllCount);
    //         }
    //     }

    //     /// <summary>
    //     /// 根据物品获得物品总数据
    //     /// 此接口太危险了 不暴露给外部使用
    //     /// </summary>
    //     /// <param name="inventoryId"></param>
    //     /// <returns></returns>
    //     private static InventoryItem FindTargetItem(InventoryId inventoryId)
    //     {
    //         foreach (var item in inventory.GetAllItems)
    //         {
    //             if (item.item.Id == (int)inventoryId)
    //             {
    //                 return item;
    //             }
    //         }
    //         return null;
    //     }

    //     /// <summary>
    //     /// 获得物品的UI显示数据
    //     /// </summary>
    //     public static ItemObject GetItemUIInfo(InventoryId inventoryId)
    //     {
    //         ItemObject itemRes = inventory?.database?.GetItemObjectById((int)inventoryId);
    //         return itemRes;
    //     }

    //     /// <summary>
    //     /// 增加物品
    //     /// </summary>
    //     /// <param name="inventoryId"></param>
    //     /// <param name="amout"></param>
    //     private static void AddInventory(InventoryId inventoryId, int amout)
    //     {
    //         // 查找资源库中是否有这个物品
    //         ItemObject itemRes = inventory?.database?.GetItemObjectById((int)inventoryId);
    //         if (itemRes != null)
    //         {
    //             AddInventory(itemRes.data, amout);
    //         }
    //     }
    //     private static void AddInventory(Item item, int amout)
    //     {
    //         inventory?.AddItem(item, amout);
    //     }

    //     /// <summary>
    //     /// 获得物品数量
    //     /// </summary>
    //     /// <param name="inventoryId"></param>
    //     /// <returns></returns>
    //     public static int GetInventoryNum(InventoryId inventoryId)
    //     {
    //         var result = 0;
    //         var targetInventory = FindTargetItem(inventoryId);
    //         if (targetInventory != null)
    //         {
    //             result = targetInventory.amount;
    //         }

    //         return result;
    //     }
    //     /// <summary>
    //     /// 修正本地库存数据
    //     /// </summary>
    //     /// <param name="serverResult"></param>
    //     private static void FixLocalInventory(ServerInventoryDropPostHandler serverResult)
    //     {
    //         if (serverResult != null && serverResult.count > 0)
    //         {
    //             AddAndCheckInventory(serverResult.itemDefId, serverResult.count, serverResult.total);
    //         }
    //     }

    //     /// <summary>
    //     /// 掉落奖励结果
    //     /// </summary>
    //     /// <param name="lessonNo">哪个课</param>
    //     /// <param name="dropScene">掉落场景是什么</param>
    //     /// <returns>返回实际获得的数量</returns>
    //     public static async Task<InventoryDropResult> DropInventroyItem(InventoryDropType dropScene, string lessonNo = null, string fragmentNo = null)
    //     {
    //         var result = await InventoryRequest.PostDropInventoryItem(dropScene, lessonNo, fragmentNo);
    //         if (result != null)
    //         {
    //             // 服务器暂时只掉落一个东西
    //             // var getItems = new List<InventoryDropResult>();
    //             var getItems = new InventoryDropResult(result.itemDefId, result.count);
    //             FixLocalInventory(result);
    //             return getItems;
    //         }
    //         return null;
    //     }

    // }

    // /// <summary>
    // /// 库存系统的请求
    // /// </summary>
    // public class InventoryRequest
    // {
    //     /// <summary>
    //     /// 请求掉落奖励结果
    //     /// </summary>
    //     /// <param name="data"></param>
    //     /// <returns></returns>
    //     public static async Task<ServerInventoryDropPostHandler> PostDropInventoryItem(InventoryDropType dropScene, string lessonNo = null, string fragmentNo = null)
    //     {
    //         var postData = new ServerInventoryDropPostInfo() { lessonNo = lessonNo, scenes = dropScene, fragmentNo = fragmentNo };
    //         var handler = await WebRequestSystem.AsyncPostWebRequest("/inventory/drop", JsonHelper.ToJson(postData));
    //         Log.Info(handler.text);
    //         var result = JsonHelper.FromJson<ServerInventoryDropPostHandler>(handler.text);
    //         return result;
    //     }
    //     /// <summary>
    //     /// 获得服务器背包数据
    //     /// </summary>
    //     /// <returns></returns>
    //     public static async Task<ServerInventoryInfo[]> GetUserBackpack()
    //     {
    //         var handler = await WebRequestSystem.AsyncGetWebRequest("/inventory/backpack");
    //         Log.Info(handler.text);
    //         var result = JsonHelper.FromJson<ServerInventoryInfo[]>(handler.text);
    //         return result;
    //     }
    // }



    // #region 数据结构
    // /// <summary>
    // /// 掉落节点类型
    // /// </summary>
    // public enum InventoryDropType
    // {
    //     /// <summary>
    //     /// 视频交互掉落
    //     /// </summary>
    //     VideoOperation = 1,
    //     /// <summary>
    //     /// 视频最后题目重做
    //     /// </summary>
    //     VideoErrorQuestionReplay = 2,
    //     /// <summary>
    //     /// 做题练习
    //     /// </summary>
    //     ExerciseQuestion = 3,
    //     /// <summary>
    //     /// 课后对弈
    //     /// </summary>
    //     GameFight = 4,
    //     /// <summary>
    //     /// 学习报告
    //     /// </summary>
    //     StudyReport = 5,
    // }
    // /// <summary>
    // /// 请求掉落奖励
    // /// </summary>
    // public class ServerInventoryDropPostInfo
    // {
    //     /// <summary>
    //     /// 课程名字
    //     /// </summary>
    //     public string lessonNo;
    //     /// <summary>
    //     /// 掉落场景
    //     /// </summary>
    //     public InventoryDropType scenes;
    //     /// <summary>
    //     /// 哪个段落
    //     /// </summary>
    //     public string fragmentNo;
    // }
    // /// <summary>
    // /// 掉落奖励结果
    // /// </summary>
    // public class ServerInventoryDropPostHandler
    // {
    //     /// <summary>
    //     /// 掉落数量
    //     /// </summary>
    //     public int count;
    //     /// <summary>
    //     /// 无用id
    //     /// </summary>
    //     public int dropId;
    //     /// <summary>
    //     /// 物品id
    //     /// </summary>
    //     public InventoryId itemDefId;
    //     /// <summary>
    //     /// 拥有的总数
    //     /// </summary>
    //     public int total;
    // }
    // /// <summary>
    // /// 服务器物品库存定义结构
    // /// </summary>
    // public class ServerInventoryInfo
    // {
    //     /// <summary>
    //     /// 物品id
    //     /// </summary>
    //     public InventoryId itemDefId;
    //     /// <summary>
    //     /// 物品数量
    //     /// </summary>
    //     public int itemCount;
    // }
    // #endregion
}