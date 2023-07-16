/*
  作者：LTH
  文件描述：
  文件名：IEntityKeyMapper
  创建时间：2023/07/15 21:07:SS
*/

namespace Entt.Serialization
{
    /// <summary>
    /// entity数据mapper
    /// </summary>
    public interface IEntityKeyMapper
    {
        public TEntityKey EntityKeyMapper<TEntityKey>(EntityKeyData entityKeyData);
    }
}