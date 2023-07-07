/*
  作者：LTH
  文件描述：
  文件名：EntityComponentRegistration
  创建时间：2023/07/07 18:07:SS
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Entt.Annotations
{
    /// <summary>
    /// 监听组件相关事件（添加组件到Entity，从Entity身上移除组件）
    /// </summary>
    public class EntityComponentRegistration
    {
        private readonly Dictionary<Type, object> data;
        
        public TypeInfo TypeInfo { get; private set; }

        public EntityComponentRegistration(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;
            data = new Dictionary<Type, object>();
        }

        public bool IsEmpty => data.Count == 0;

        /// <summary>
        /// 把数据value存储到data字典中
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TData"></typeparam>
        public void Store<TData>(TData value)
        {
            object? o = value;
            if (o != null)
            {
                data[typeof(TData)] = o;
            }
        }

        //从data字典中获取值
        public bool TryGet<TData>([MaybeNullWhen(false)]out TData result)
        {
            if (data.TryGetValue(typeof(TData), out var rawData) &&
                rawData is TData realData)
            {
                result = realData;
                return true;
            }

            result = default;
            return false;
        }

        public override string ToString()
        {
            return $"EntityComponentRegistration({nameof(TypeInfo)}:{TypeInfo},{nameof(IsEmpty)}:{IsEmpty})";
        }
    }
}