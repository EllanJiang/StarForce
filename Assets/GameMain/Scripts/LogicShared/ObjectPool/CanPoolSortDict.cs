/*
* 文件名：CanPoolSortDict
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/07 16:04:29
* 修改记录：
*/

using System;

namespace LogicShared
{
    public class CanPoolSortDict<K,V> : IObjectPool
    {
        public readonly SortDict<K, V> SortDict = new SortDict<K, V>();
        public void PutBackPool()
        {
            SortDict.Clear();
        }
    }
}