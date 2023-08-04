/*
* 文件名：NetPacketProcessor
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 17:39:59
* 修改记录：
*/

using System;
using System.Collections.Generic;

namespace LogicShared.LiteNetLib.Utils
{
    /// <summary>
    /// 网络包处理器
    /// </summary>
    public class NetPacketProcessor
    {
        private static class HashCache<T>
        {
            public static bool Initialized;
            public static ulong Id;
        }
        
        /// <summary>
        /// 监听网络包消息委托
        /// </summary>
        protected delegate void SubscribeDelegate(NetDataReader reader, object userData);
        private readonly NetSerializer _netSerializer;
        /// <summary>
        /// 网络包消息回调
        /// key:网络包的哈希值
        /// </summary>
        private readonly Dictionary<ulong, SubscribeDelegate> _callbacks = new Dictionary<ulong, SubscribeDelegate>();
        private readonly NetDataWriter _netDataWriter = new NetDataWriter();
        
        public NetPacketProcessor()
        {
            _netSerializer = new NetSerializer();
        }
        
        public NetPacketProcessor(int maxStringLength)
        {
            _netSerializer = new NetSerializer(maxStringLength);
        }
        
        /// <summary>
        /// 获取网络包的哈希值，可以看做是协议ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //FNV-1 64 bit hash
        protected virtual ulong GetHash<T>()
        {
            if(HashCache<T>.Initialized)
                return HashCache<T>.Id;

            // ulong hash = 14695981039346656037UL; //offset
            // string typeName = typeof(T).FullName;
            // for (var i = 0; i < typeName.Length; i++)
            // {
            //     hash = hash ^ typeName[i];
            //     hash *= 1099511628211UL; //prime
            // }
            // 协议ID
            ulong hash = ProtoID.TryGetId<T>();
            HashCache<T>.Initialized = true;
            HashCache<T>.Id = hash;
            return hash;
        }
        
        /// <summary>
        /// 从网络包数据中获取回调方法
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        protected virtual SubscribeDelegate GetCallbackFromData(NetDataReader reader)
        {
            //网络包的哈希值
            var hash = reader.GetULong();
            SubscribeDelegate action;
            if (!_callbacks.TryGetValue(hash, out action))
            {
                throw new ParseException("Undefined packet in NetDataReader");
            }
            return action;
        }
        
        /// <summary>
        /// 往网络包中写入网络包哈希值
        /// </summary>
        /// <param name="writer"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void WriteHash<T>(NetDataWriter writer)
        {
            writer.Put(GetHash<T>());
        }
        
        /// <summary>
        /// Register nested property type
        /// </summary>
        /// <typeparam name="T">INetSerializable structure</typeparam>
        public void RegisterNestedType<T>() where T : struct, INetSerializable
        {
            _netSerializer.RegisterNestedType<T>();
        }

        /// <summary>
        /// Register nested property type
        /// </summary>
        /// <param name="writeDelegate"></param>
        /// <param name="readDelegate"></param>
        public void RegisterNestedType<T>(Action<NetDataWriter, T> writeDelegate, Func<NetDataReader, T> readDelegate)
        {
            _netSerializer.RegisterNestedType<T>(writeDelegate, readDelegate);
        }

        /// <summary>
        /// Register nested property type
        /// </summary>
        /// <typeparam name="T">INetSerializable class</typeparam>
        public void RegisterNestedType<T>(Func<T> constructor) where T : class, INetSerializable
        {
            _netSerializer.RegisterNestedType(constructor);
        }

        /// <summary>
        /// Reads all available data from NetDataReader and calls OnReceive delegates
        /// </summary>
        /// <param name="reader">NetDataReader with packets data</param>
        public void ReadAllPackets(NetDataReader reader)
        {
            while (reader.AvailableBytes > 0)
                ReadPacket(reader);
        }

        /// <summary>
        /// Reads all available data from NetDataReader and calls OnReceive delegates
        /// </summary>
        /// <param name="reader">NetDataReader with packets data</param>
        /// <param name="userData">Argument that passed to OnReceivedEvent</param>
        /// <exception cref="ParseException">Malformed packet</exception>
        public void ReadAllPackets(NetDataReader reader, object userData)
        {
            while (reader.AvailableBytes > 0)
                ReadPacket(reader, userData);
        }

        /// <summary>
        /// Reads one packet from NetDataReader and calls OnReceive delegate
        /// </summary>
        /// <param name="reader">NetDataReader with packet</param>
        /// <exception cref="ParseException">Malformed packet</exception>
        public void ReadPacket(NetDataReader reader)
        {
            ReadPacket(reader, null);
        }

        /// <summary>
        /// Reads one packet from NetDataReader and calls OnReceive delegate
        /// </summary>
        /// <param name="reader">NetDataReader with packet</param>
        /// <param name="userData">Argument that passed to OnReceivedEvent</param>
        /// <exception cref="ParseException">Malformed packet</exception>
        public void ReadPacket(NetDataReader reader, object userData)
        {
            GetCallbackFromData(reader)(reader, userData);
        }
        
        /// <summary>
        /// 给指定客户端发送网络包，自动序列化网络包
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="packet"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        public void Send<T>(NetPeer peer, T packet, DeliveryMethod options) where T : class, new()
        {
            _netDataWriter.Reset();
            Write(_netDataWriter, packet);
            peer.Send(_netDataWriter, options);
        }

        /// <summary>
        ///  给指定客户端发送网络包，使用自己手写的序列化方法来序列化网络包
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="packet"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        public void SendNetSerializable<T>(NetPeer peer, T packet, DeliveryMethod options) where T : INetSerializable
        {
            _netDataWriter.Reset();
            WriteNetSerializable(_netDataWriter, packet);
            peer.Send(_netDataWriter, options);
        }

        /// <summary>
        /// 给所有客户端发送网络包，自动序列化网络包
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packet"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        public void Send<T>(NetManager manager, T packet, DeliveryMethod options) where T : class, new()
        {
            _netDataWriter.Reset();
            Write(_netDataWriter, packet);
            manager.SendToAll(_netDataWriter, options);
        }
        
        /// <summary>
        /// 给所有客户端发送网络包，使用自己手写的序列化方法来序列化网络包s
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="packet"></param>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        public void SendNetSerializable<T>(NetManager manager, T packet, DeliveryMethod options) where T : INetSerializable
        {
            _netDataWriter.Reset();
            WriteNetSerializable(_netDataWriter, packet);
            manager.SendToAll(_netDataWriter, options);
        }

        /// <summary>
        /// Write和WriteNetSerializable的区别是：
        /// Write方法自动对网络包进行序列化和反序列化，WriteNetSerializable方法使用网络包自己手写的序列化方法和反序列化方法进行序列化和反序列化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="packet"></param>
        /// <typeparam name="T"></typeparam>
        public void Write<T>(NetDataWriter writer, T packet) where T : class, new()
        {
            WriteHash<T>(writer);       //先写入哈希值，当做协议ID，用来区分不同网络包
            _netSerializer.Serialize(writer, packet);
        }

        /// <summary>
        /// Write和WriteNetSerializable的区别是：
        /// Write方法自动对网络包进行序列化和反序列化，WriteNetSerializable方法使用网络包自己手写的序列化方法和反序列化方法进行序列化和反序列化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="packet"></param>
        /// <typeparam name="T"></typeparam>
        public void WriteNetSerializable<T>(NetDataWriter writer, T packet) where T : INetSerializable
        {
            WriteHash<T>(writer);
            packet.Serialize(writer);
        }

        /// <summary>
        /// Write和WriteNetSerializable的区别是：
        /// Write方法自动对网络包进行序列化和反序列化，WriteNetSerializable方法使用网络包自己手写的序列化方法和反序列化方法进行序列化和反序列化
        /// </summary>
        /// <param name="packet"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>返回序列化得到的字节数组(复制版)</returns>
        public byte[] Write<T>(T packet) where T : class, new()
        {
            _netDataWriter.Reset();
            WriteHash<T>(_netDataWriter);
            _netSerializer.Serialize(_netDataWriter, packet);
            return _netDataWriter.CopyData();
        }

        /// <summary>
        /// Write和WriteNetSerializable的区别是：
        /// Write方法自动对网络包进行序列化和反序列化，WriteNetSerializable方法使用网络包自己手写的序列化方法和反序列化方法进行序列化和反序列化
        /// </summary>
        /// <param name="packet"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>返回序列化得到的字节数组(复制版)</returns>
        public byte[] WriteNetSerializable<T>(T packet) where T : INetSerializable
        {
            _netDataWriter.Reset();
            WriteHash<T>(_netDataWriter);
            packet.Serialize(_netDataWriter);
            return _netDataWriter.CopyData();
        }
        
        /// <summary>
        /// Register and subscribe to packet receive event
        /// </summary>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        /// <param name="packetConstructor">Method that constructs packet instead of slow Activator.CreateInstance</param>
        /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields</exception>
        public void Subscribe<T>(Action<T> onReceive, Func<T> packetConstructor) where T : class, new()
        {
            _netSerializer.Register<T>();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                var reference = packetConstructor();
                _netSerializer.Deserialize(reader, reference);
                onReceive(reference);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event (with userData)
        /// </summary>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        /// <param name="packetConstructor">Method that constructs packet instead of slow Activator.CreateInstance</param>
        /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields</exception>
        public void Subscribe<T, TUserData>(Action<T, TUserData> onReceive, Func<T> packetConstructor) where T : class, new()
        {
            _netSerializer.Register<T>();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                var reference = packetConstructor();
                _netSerializer.Deserialize(reader, reference);
                onReceive(reference, (TUserData)userData);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event
        /// This method will overwrite last received packet class on receive (less garbage)
        /// </summary>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields</exception>
        public void SubscribeReusable<T>(Action<T> onReceive) where T : class, new()
        {
            _netSerializer.Register<T>();
            var reference = new T();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                _netSerializer.Deserialize(reader, reference);
                onReceive(reference);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event (with userData)
        /// This method will overwrite last received packet class on receive (less garbage)
        /// </summary>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        /// <exception cref="InvalidTypeException"><typeparamref name="T"/>'s fields are not supported, or it has no fields</exception>
        public void SubscribeReusable<T, TUserData>(Action<T, TUserData> onReceive) where T : class, new()
        {
            _netSerializer.Register<T>();
            var reference = new T();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                _netSerializer.Deserialize(reader, reference);
                onReceive(reference, (TUserData)userData);
            };
        }
        
        /// <summary>
        /// Register and subscribe to packet receive event (with userData)
        /// 与Subscribe的区别是：Subscribe自动进行序列化和反序列化，而SubscribeNetSerializable需要自己提供序列化和反序列化方法
        /// </summary>
        /// <param name="onReceive">收到消息时会触发该回调函数</param>
        /// <param name="packetConstructor">需要自己提供网络包的构造函数</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TUserData"></typeparam>
        public void SubscribeNetSerializable<T, TUserData>(
            Action<T, TUserData> onReceive, 
            Func<T> packetConstructor) where T : INetSerializable
        {
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                var pkt = packetConstructor();
                pkt.Deserialize(reader);
                onReceive(pkt, (TUserData)userData);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event
        /// 与Subscribe的区别是：Subscribe自动进行序列化和反序列化，而SubscribeNetSerializable需要自己提供序列化和反序列化方法
        /// </summary>
        /// <param name="onReceive">收到消息时会触发该回调函数</param>
        /// <param name="packetConstructor">需要自己提供网络包的构造函数/param>
        /// <typeparam name="T"></typeparam>
        public void SubscribeNetSerializable<T>(
            Action<T> onReceive,
            Func<T> packetConstructor) where T : INetSerializable
        {
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                var pkt = packetConstructor();
                pkt.Deserialize(reader);
                onReceive(pkt);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event
        /// 与Subscribe的区别是：Subscribe自动进行序列化和反序列化，而SubscribeNetSerializable需要自己提供序列化和反序列化方法
        /// </summary>
        /// <param name="onReceive">收到消息时会触发该回调函数</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TUserData"></typeparam>
        public void SubscribeNetSerializable<T, TUserData>(
            Action<T, TUserData> onReceive) where T : INetSerializable, new()
        {
            var reference = new T();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                reference.Deserialize(reader);
                onReceive(reference, (TUserData)userData);
            };
        }

        /// <summary>
        /// Register and subscribe to packet receive event
        /// 与Subscribe的区别是：Subscribe自动进行序列化和反序列化，而SubscribeNetSerializable需要自己提供序列化和反序列化方法
        /// </summary>
        /// <param name="onReceive">收到消息时会触发该回调函数</param>
        /// <typeparam name="T"></typeparam>
        public void SubscribeNetSerializable<T>(
            Action<T> onReceive) where T : INetSerializable, new()
        {
            var reference = new T();
            _callbacks[GetHash<T>()] = (reader, userData) =>
            {
                reference.Deserialize(reader);
                onReceive(reference);
            };
        }

        
        /// <summary>
        /// Remove any subscriptions by type
        /// </summary>
        /// <typeparam name="T">Packet type</typeparam>
        /// <returns>true if remove is success</returns>
        public bool RemoveSubscription<T>()
        {
            return _callbacks.Remove(GetHash<T>());
        }
    }
}