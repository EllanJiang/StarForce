/*
  作者：LTH
  文件描述：
  文件名：SerializationSurrogateProviderBase
  创建时间：2023/07/16 16:07:SS
*/

using System;
using System.Runtime.Serialization;

namespace Entt.Serialization.Xml
{
    /// <summary>
    /// 序列化基类
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <typeparam name="TSurrogateType"></typeparam>
    public abstract class SerializationSurrogateProviderBase<TObject, TSurrogateType> : ISerializationSurrogateProvider
    {
        /// <summary>
        /// 反序列化的对象,并返回该对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public object? GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is TSurrogateType surrogate)
            {
                return GetDeserializedObject(surrogate);
            }

            return obj;
        }

        /// <summary>
        /// 获取指定类型的反序列化对象
        /// </summary>
        /// <param name="surrogate"></param>
        /// <returns></returns>
        public abstract TObject? GetDeserializedObject(TSurrogateType surrogate);
        
        /// <summary>
        /// 序列化对象，并返回该对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="surrogateType"></param>
        /// <returns></returns>
        public object? GetObjectToSerialize(object obj, Type surrogateType)
        {
            if (obj is TObject source)
            {
                return GetObjectToSerialize(source);
            }

            return obj;
        }

        public abstract TSurrogateType? GetObjectToSerialize(TObject obj);
        

        /// <summary>
        /// 获取代理的类型
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public Type GetSurrogateType(Type memberType)
        {
            if (memberType == typeof(TObject))
            {
                return typeof(TSurrogateType);
            }

            return memberType;
        }
    }
}