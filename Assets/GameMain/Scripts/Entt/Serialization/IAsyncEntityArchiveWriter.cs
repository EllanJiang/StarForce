/*
  作者：LTH
  文件描述：
  文件名：IAsyncEntityArchiveWriter
  创建时间：2023/07/15 22:07:SS
*/

using System.Threading.Tasks;

namespace Entt.Serialization
{
    /// <summary>
    /// 异步保存entity数据
    /// </summary>
    public interface IAsyncEntityArchiveWriter<TEntityKey>
    {
        Task WriteStartEntityAsync(in int entityCount);
        Task WriteEntityAsync(in TEntityKey entityKey);
        Task WriteEndEntityAsync();

        Task WriteStartComponentAsync<TComponent>(in int entityCount);
        Task WriteComponentAsync<TComponent>(in TEntityKey entityKey, in TComponent c);
        Task WriteEndComponentAsync<TComponent>();

        Task WriteTagAsync<TComponent>(in TEntityKey entityKey, in TComponent c);
        Task WriteMissingTagAsync<TComponent>();

        Task WriteStartDestroyedAsync(in int entityCount);
        Task WriteDestroyedAsync(in TEntityKey entityKey);
        
        Task WriteEndDestroyedAsync();
        Task FlushFrameAsync();
    }
}