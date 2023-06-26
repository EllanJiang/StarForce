/*
  作者：LTH
  文件描述：
  文件名：PersistentViewParallelism
  创建时间：2023/06/24 00:06:SS
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entt.Entities.Helpers;
using Serilog;

namespace Entt.Entities
{
    /// <summary>
    /// 并行处理persistent view
    /// </summary>
    public static class PersistentViewParallelism
    {
        static readonly ILogger logger = LogHelper.ForContext(typeof(PersistentViewParallelism));
        
        public static void PartitionAndRun<TEntityKey>(List<TEntityKey> p, Action<TEntityKey> action)
        {
            if (p.Count == 0)
            {
                return;
            }

            var rangeSize = Math.Max(1, p.Count / Environment.ProcessorCount);
            var partitioner = Partitioner.Create(0, p.Count, rangeSize);
            Parallel.ForEach(partitioner, range =>
            {
                var (min, max) = range;
                for (int i = min; i < max; i += 1)
                {
                    action(p[i]);
                }
            });
        }

        /// <summary>
        /// 划分列表，然后并行处理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        public static void PartitionAndRunMany<TEntityKey, TContext>(RawList<TEntityKey> p,
            TContext context,
            Action<RawList<TEntityKey>, TContext, int, int> action)
        {
            if (p.Count == 0)
            {
                return;
            }

            // Environment.ProcessorCount 当前进程可用的逻辑处理器数量（例如四核CPU，每个核有两个逻辑处理器，那么可用的逻辑处理器数量就等于8）
            var rangeSize = (int) Math.Max(1, Math.Ceiling(p.Count / (float) Environment.ProcessorCount));
            var partitioner = Partitioner.Create(0, p.Count, rangeSize);
            ParallelOptions opts = new ParallelOptions();
            //设置最大并行处理数量
            opts.MaxDegreeOfParallelism = Environment.ProcessorCount;
            
            Parallel.ForEach(partitioner, opts, range =>
            {
                var (min, max) = range;
                logger.Verbose("Executing Partition {Min} - {Max}", min, max);
                action(p, context, min, max);
            });
        }
    }
}