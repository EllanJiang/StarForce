/*
* 文件名：HeapSorter
* 文件描述：堆排序
    参考 https://github.com/aalhour/C-Sharp-Algorithms/blob/master/Algorithms/Sorting/HeapSorter.cs (MIT licensed)
* 作者：aronliang
* 创建时间：2023/06/21 15:47:28
* 修改记录：
*/

using System.Collections.Generic;

namespace Entt.Entities.Helpers
{
    /// <summary>
    /// 原址堆排序
    /// </summary>
    public static class HeapSorter
    {
        /// <summary>
        /// Public API: Default functionality.
        /// Sorts in ascending order（升序）. Uses Max-Heaps（最大堆）.
        /// </summary>
        public static void HeapSort<T>(this ISortableCollection<T> collection, IComparer<T>? comparer = null)
        {
            collection.HeapSortAscending(comparer);
        }

        /// <summary>
        /// Public API: Sorts ascending
        /// Uses Max-Heaps
        /// </summary>
        public static void HeapSortAscending<T>(this ISortableCollection<T> collection, IComparer<T>? comparer = null)
        {
            // Handle the comparer's default null value
            comparer = comparer ?? Comparer<T>.Default;

            int lastIndex = collection.Count - 1;
            //构建大顶锥,最大值保存到索引0处
            collection.BuildMaxHeap(0, lastIndex, comparer);
            while (lastIndex >= 0)
            {
                //把最大值放到数组最后
                collection.Swap(0, lastIndex);
                lastIndex--;
                //然后将剩下的元素重新构建大顶锥，剩下元素的最大值又保存在0处，以此类推
                collection.MaxHeapify(0, lastIndex, comparer);
            }
        }

        /// <summary>
        /// Public API: Sorts descending
        /// Uses Min-Heaps
        /// </summary>
        public static void HeapSortDescending<T>(this ISortableCollection<T> collection, IComparer<T>? comparer = null)
        {
            // Handle the comparer's default null value
            comparer = comparer ?? Comparer<T>.Default;

            int lastIndex = collection.Count - 1;
            collection.BuildMinHeap(0, lastIndex, comparer);

            while (lastIndex >= 0)
            {
                collection.Swap(0, lastIndex);
                lastIndex--;
                collection.MinHeapify(0, lastIndex, comparer);
            }
        }

        /****************************************************************************/

        /// <summary>
        /// Private Max-Heap Builder.
        /// Builds a max heap from an IList[T] collection.
        /// </summary>
        static void BuildMaxHeap<T>(this ISortableCollection<T> collection, int firstIndex, int lastIndex, IComparer<T> comparer)
        {
            int lastNodeWithChildren = lastIndex / 2;

            for (int node = lastNodeWithChildren; node >= firstIndex; --node)
            {
                collection.MaxHeapify(node, lastIndex, comparer);
            }
        }

        /// <summary>
        /// Private Max-Heapifier. Used in BuildMaxHeap.
        /// Heapfies the elements between two indexes (inclusive), maintaining the maximum at the top.
        /// </summary>
        static void MaxHeapify<T>(this ISortableCollection<T> collection, int nodeIndex, int lastIndex, IComparer<T> comparer)
        {
            // assume left(i) and right(i) are max-heaps
            int left = (nodeIndex * 2) + 1;
            int right = left + 1;
            int largest = nodeIndex;

            // If collection[left] > collection[nodeIndex]
            if (left <= lastIndex && comparer.Compare(collection[left], collection[nodeIndex]) > 0)
                largest = left;

            // If collection[right] > collection[largest]
            if (right <= lastIndex && comparer.Compare(collection[right], collection[largest]) > 0)
                largest = right;

            // Swap and heapify
            if (largest != nodeIndex)
            {
                collection.Swap(nodeIndex, largest);
                collection.MaxHeapify(largest, lastIndex, comparer);
            }
        }

        /// <summary>
        /// Private Min-Heap Builder.
        /// Builds a min heap from an IList[T] collection.
        /// </summary>
        static void BuildMinHeap<T>(this ISortableCollection<T> collection, int firstIndex, int lastIndex, IComparer<T> comparer)
        {
            int lastNodeWithChildren = lastIndex / 2;

            for (int node = lastNodeWithChildren; node >= firstIndex; --node)
            {
                collection.MinHeapify(node, lastIndex, comparer);
            }
        }

        /// <summary>
        /// Private Min-Heapifier. Used in BuildMinHeap.
        /// Heapfies the elements between two indexes (inclusive), maintaining the minimum at the top.
        /// </summary>
        static void MinHeapify<T>(this ISortableCollection<T> collection, int nodeIndex, int lastIndex, IComparer<T> comparer)
        {
            // assume left(i) and right(i) are max-heaps
            int left = (nodeIndex * 2) + 1;
            int right = left + 1;
            int smallest = nodeIndex;

            // If collection[left] > collection[nodeIndex]
            if (left <= lastIndex && comparer.Compare(collection[left], collection[nodeIndex]) < 0)
                smallest = left;

            // If collection[right] > collection[largest]
            if (right <= lastIndex && comparer.Compare(collection[right], collection[smallest]) < 0)
                smallest = right;

            // Swap and heapify
            if (smallest != nodeIndex)
            {
                collection.Swap(nodeIndex, smallest);
                collection.MinHeapify(smallest, lastIndex, comparer);
            }
        }
    }
}