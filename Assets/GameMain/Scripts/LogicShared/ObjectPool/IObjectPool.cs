/*
* 文件名：IObjectPool
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/07 14:44:58
* 修改记录：
*/

namespace LogicShared
{
    public interface IObjectPool
    {
        /// <summary>
        /// 放回对象池前调用该方法，重置数据
        /// </summary>
        void PutBackPool();
    }
}