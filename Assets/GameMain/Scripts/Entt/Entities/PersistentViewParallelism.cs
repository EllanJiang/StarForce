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

        public static void PartitionAndRunMany<TEntityKey, TContext>(RawList<TEntityKey> p,
            TContext context,
            Action<RawList<TEntityKey>, TContext, int, int> action)
        {
            if (p.Count == 0)
            {
                return;
            }

            var rangeSize = (int) Math.Max(1, Math.Ceiling(p.Count / (float) Environment.ProcessorCount));
            var partitioner = Partitioner.Create(0, p.Count, rangeSize);
            ParallelOptions opts = new ParallelOptions();
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