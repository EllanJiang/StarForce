/*
* 文件名：CanPoolList
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/07 16:03:47
* 修改记录：
*/

using System.Collections.Generic;

namespace LogicShared
{
    public class CanPoolList<T> : IObjectPool
    {
        public readonly List<T> List = new List<T>();
        public void PutBackPool()
        {
            List.Clear();
        }
    }
}