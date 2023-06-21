/*
* 文件名：ISortableCollection
* 文件描述：有序列表接口
* 作者：aronliang
* 创建时间：2023/06/21 15:20:40
* 修改记录：
*/

namespace Entt.Entities.Helpers
{
    public interface ISortableCollection<out T>
    {
        int Count { get; }
        T this[int index] { get; }
        void Swap(int idxSrc, int idxTgt);
    }
}