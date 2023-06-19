/*
* 文件名：Optional
* 文件描述：可选项？
* 作者：aronliang
* 创建时间：2023/06/19 19:46:17
* 修改记录：
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Entt
{
    /// <summary>
    /// 空白占位符
    /// </summary>
    public readonly struct OptionalEmptyPlaceHolder{}
    
    public static class Optional
    {
        public static Optional<T> Empty<T>()
        {
            return new Optional<T>(false, default!);
        }

        public static OptionalEmptyPlaceHolder Empty()
        {
            return new OptionalEmptyPlaceHolder();
        }

        public static Optional<T> ValueOf<T>(T value)
        {
            return new Optional<T>(true, value);
        }

        public static Optional<T> OfNullable<T>(T? value) where T : struct
        {
            if (!value.HasValue)
            {
                return Empty();
            }

            return new Optional<T>(true, value.Value);
        }

        public static Optional<T> OfNullable<T>(T? value) where T : class
        {
            if (value == null)
            {
                return Empty();
            }

            return new Optional<T>(true, value);
        }
    }

    /// <summary>
    /// DataContracts：数据契约,一旦声明一个类型为数据契约，那么该类型就可以被序列化，以在服务端和客户端之间传送
    /// 把每一个需要传送的成员声明为DataMember，才能进行传送
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public readonly struct Optional<T> : IEquatable<Optional<T>>, IEnumerable<T>
    {
        //Order用于控制序列化顺序
        [DataMember(Order = 0)]
        public bool HasValue { get; }
        
        [DataMember(Order = 1)]
        private readonly T value;
        
        //不用于传送
        [IgnoreDataMember]
        public T Value => HasValue ? value : throw new InvalidOperationException();
        
        internal Optional(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

        public bool TryGetValue(out T v)
        {
            v = value;
            return HasValue;
        }
        
        
       
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return value;
            }
        }


        public override string ToString()
        {
            if (HasValue)
            {
                return $"Some{value}";
            }

            return "None";
        }

        public static implicit operator Optional<T>(T data)
        {
            return Optional.ValueOf(data);
        }
        
        public bool Equals(Optional<T> other)
        {
            return HasValue == other.HasValue &&
                   EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public bool Equals(T other)
        {
            return HasValue &&
                   EqualityComparer<T>.Default.Equals(value, other);
        }

        public override bool Equals(object obj)
        {
            return obj is Optional<T> optional &&
                   Equals(optional);
        }


        public override int GetHashCode()
        {
            //不检查溢出等Exception
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(value) ^ 397) ^ HasValue.GetHashCode();
            }
        }

        public T GetOrElse(T t) => HasValue ? value : t;

        public T GetOrElse(Func<T> ft) => HasValue ? value : ft();

        public Optional<TNext> Select<TNext>(Func<T, TNext> selector)
        {
            if (TryGetValue(out var v))
            {
                return selector(v);
            }
            return Optional.Empty();
        }

        public Optional<TNext> SelectMany<TNext>(Func<T, Optional<TNext>> selector)
        {
            if (TryGetValue(out var v))
            {
                return selector(v);
            }

            return Optional.Empty();
        }

        public static bool operator ==(Optional<T> left, T right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(Optional<T> left, T right)
        {
            return !left.Equals(right);
        }
        
        public static bool operator ==(Optional<T> left, Optional<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Optional<T> left, Optional<T> right)
        {
            return !left.Equals(right);
        }


        //把OptionalEmptyPlaceHolder隐式转换成Optional<T>
        public static implicit operator Optional<T>(OptionalEmptyPlaceHolder p)
        {
            return default;
        }
    }


}