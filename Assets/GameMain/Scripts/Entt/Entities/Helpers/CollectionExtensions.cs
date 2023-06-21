/*
* 文件名：CollectionExtensions
* 文件描述：
* 作者：aronliang
* 创建时间：2023/06/21 17:53:06
* 修改记录：
*/

using System;
using System.Collections.Generic;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 容器拓展类
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 把数据data存储到列表的index索引处
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        internal static void StoreAt<T>(this List<T> list, int index, T data)
        {
            if (list.Count <= index)
            {
                list.Capacity = Math.Max(list.Capacity, index + 1);
                while (list.Count <= index)
                {
                    list.Add(default!);
                }
            }

            list[index] = data;
        }
    }
}