/*
  作者：LTH
  文件描述：
  文件名：DefaultEntityKeyMapper
  创建时间：2023/07/15 22:07:SS
*/

using System;
using System.Collections.Generic;

namespace Entt.Serialization
{
    /// <summary>
    /// 默认entity映射器
    /// </summary>
    public class DefaultEntityKeyMapper:IEntityKeyMapper
    {
        private readonly Dictionary<Type, object> data;

        public DefaultEntityKeyMapper()
        {
            data = new Dictionary<Type, object>();
        }

        /// <summary>
        /// 注册映射器
        /// </summary>
        /// <param name="converter">这是一个函数，参数类型EntityKeyData，返回类型是TEntityKey</param>
        /// <typeparam name="TEntityKey">指定的类型</typeparam>
        /// <returns></returns>
        public DefaultEntityKeyMapper Register<TEntityKey>(Func<EntityKeyData, TEntityKey> converter)
        {
            data[typeof(TEntityKey)] = converter;
            return this;
        }

        /// <summary>
        /// 获取映射器，并将entityKeyData作为参数传入，然后调用映射器，返回映射器的值
        /// </summary>
        /// <param name="entityKeyData"></param>
        /// <typeparam name="TEntityKey"></typeparam>
        /// <returns></returns>
        public TEntityKey EntityKeyMapper<TEntityKey>(EntityKeyData entityKeyData)
        {
            if (this.data.TryGetValue(typeof(TEntityKey), out var converter) &&
                converter is Func<EntityKeyData,TEntityKey> func)
            {
                return func(entityKeyData);
            }

            throw new SnapshotIOException();
        }
    }
}