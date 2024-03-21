/*
* 文件名：Entry
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/17 19:42:53
* 修改记录：
*/


using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 热更新代码入口
/// </summary>
public static class Entry
{
    /// <summary>
    /// 热更新代码入口函数
    /// </summary>
    public static void Start()
    {
        Debug.Log("[Entry::Start] 看到这个日志表示你成功运行了热更新代码 热更新成功");
        Run_InstantiateByAddComponent();
        Run_AOTGeneric();
    }

    private static void Run_InstantiateByAddComponent()
    {
        // 代码中动态挂载脚本
        GameObject cube = new GameObject("");
        cube.AddComponent<InstantiateByAddComponent>();
    }


    struct MyVec3
    {
        public int x;
        public int y;
        public int z;
    }

    //如果AOT中没有使用过这个类型的泛型，那么必须获得补充元数据dll并执行补充元数据dll后才能使用这个类型的泛型
    //使用AOT泛型List<T>
    private static void Run_AOTGeneric()
    {
        // 泛型实例化
        var arr = new List<MyVec3>();
        arr.Add(new MyVec3 { x = 1 });
        Debug.Log("[Demos.Run_AOTGeneric] 成功运行泛型代码 热更新成功");
    }
}