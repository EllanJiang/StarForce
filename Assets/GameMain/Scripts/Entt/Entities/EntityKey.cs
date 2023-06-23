/*
  作者：LTH
  文件描述：EntityID
  文件名：EntityKey
  创建时间：2023/06/23 21:06:SS
*/

using System;
using Entt.Entities.Attributes;

namespace Entt.Entities
{
    [EntityKey]
    [Serializable]
    public readonly struct EntityKey:IEquatable<EntityKey>,IEntityKey
    {
        public static readonly int MaxAge = 255;
        private readonly uint keyData;
        
        public int Key => (int)(keyData & 0xFF_FFFF) - 1;
        public byte Age => (byte)((keyData >> 24) & 0xFF);
        
        public EntityKey(byte age, int key)
        {
            if (key < 0) throw new ArgumentException();

            keyData = (uint)(age << 24);
            keyData |= (uint)((key + 1) & 0xFF_FFFF);
        }
        
        public bool IsEmpty => keyData == 0;
        
        public bool Equals(EntityKey other)
        {
            return keyData == other.keyData;
        }

        public bool Equals(IEntityKey entityKey)
        {
            return entityKey is EntityKey other && Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKey other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)keyData;
            }
        }
        
        public static bool operator ==(EntityKey left, EntityKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKey left, EntityKey right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"EntityKey({nameof(Key)}: {Key}, {nameof(Age)}: {Age})";
        }
        
        /// <summary>
        /// 唯一创建EntityID接口
        /// </summary>
        /// <param name="age"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityKey Create(byte age, int id)
        {
            return new EntityKey(age, id);
        }
    }

    /// <summary>
    /// 跟正常的EntityKey相比，多了一个PayLoad，可以认为是加载代价
    /// </summary>
    /// <typeparam name="TPayLoad"></typeparam>
    [EntityKey]
    [Serializable]
    public readonly struct EntityKey<TPayLoad> : IEquatable<EntityKey<TPayLoad>>, IEntityKey
    {
        private readonly uint keyData;
        public readonly TPayLoad PayLoad;
        
        public int Key => (int)keyData & 0xFF_FFFF;
        public byte Age => (byte)((keyData >> 24) & 0xFF);
        
        public EntityKey(byte age, int key, TPayLoad payLoad)
        {
            if (key < 0) throw new ArgumentException();

            PayLoad = payLoad;
            keyData = (uint)(age << 24);
            keyData |= (uint)(key & 0xFF_FFFF);
        }
        public bool IsEmpty => keyData == 0;
        
        public bool Equals(EntityKey<TPayLoad> other)
        {
            return keyData == other.keyData;
        }

        public bool Equals(IEntityKey obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (int)keyData;
            }
        }

        public static bool operator ==(EntityKey<TPayLoad> left, EntityKey<TPayLoad> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityKey<TPayLoad> left, EntityKey<TPayLoad> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{nameof(Key)}: {Key}, {nameof(Age)}: {Age}";
        }
    }
}