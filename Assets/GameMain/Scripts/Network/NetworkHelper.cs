using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class NetworkHelper : NetworkHelperBase
    {
        private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();

        private void Start()
        {
            // 反射注册包和包行为。
            Type packetBaseType = typeof(ServerToClientPacketBase);
            Type packetHandlerBaseType = typeof(IPacketHandler);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }

                if (types[i].BaseType == packetBaseType)
                {
                    PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                    Type packetType = GetServerToClientPacketType(packetBase.Id);
                    if (packetType != null)
                    {
                        Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                        continue;
                    }

                    m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);
                }
                else if (packetHandlerBaseType.IsAssignableFrom(types[i]))
                {
                    IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                    GameEntry.Network.RegisterHandler(packetHandler);
                }
            }

            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkConnected, OnNetworkConnected);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkClosed, OnNetworkClosed);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkSendPacket, OnNetworkSendPacket);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkMissHeartBeat, OnNetworkMissHeartBeat);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkError, OnNetworkError);
            GameEntry.Event.Subscribe(UnityGameFramework.Runtime.EventId.NetworkCustomError, OnNetworkCustomError);
        }

        private void OnDestroy()
        {
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkConnected, OnNetworkConnected);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkClosed, OnNetworkClosed);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkSendPacket, OnNetworkSendPacket);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkMissHeartBeat, OnNetworkMissHeartBeat);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkError, OnNetworkError);
            GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.EventId.NetworkCustomError, OnNetworkCustomError);
        }

        /// <summary>
        /// 发送心跳协议包。
        /// </summary>
        public override bool SendHeartBeat(INetworkChannel networkChannel)
        {
            CSHeartBeat packet = new CSHeartBeat();
            networkChannel.Send(packet);

            return true;
        }

        /// <summary>
        /// 序列化协议包。
        /// </summary>
        /// <typeparam name="T">协议包类型。</typeparam>
        /// <param name="destination">要序列化的目标流。</param>
        /// <param name="packet">要序列化的协议包。</param>
        public override void Serialize<T>(INetworkChannel networkChannel, Stream destination, T packet)
        {
            PacketBase packetImpl = packet as PacketBase;
            if (packetImpl == null)
            {
                Log.Warning("Packet is invalid.");
                return;
            }

            if (packetImpl.PacketType != PacketType.ClientToServer)
            {
                Log.Warning("Send packet invalid.");
                return;
            }

            CSPacketHead packetHead = new CSPacketHead(packetImpl.PacketId);
            Serializer.SerializeWithLengthPrefix(destination, packetHead, PrefixStyle.Fixed32);
            Serializer.Serialize(destination, packet);
        }

        /// <summary>
        /// 反序列化协议包。
        /// </summary>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义网络错误数据。</param>
        /// <returns>反序列化后的协议包。</returns>
        public override Packet Deserialize(INetworkChannel networkChannel, Stream source, out object customErrorData)
        {
            // 注意：此函数并不在主线程调用！
            customErrorData = null;
            SCPacketHead packetHead = Serializer.DeserializeWithLengthPrefix<SCPacketHead>(source, PrefixStyle.Fixed32);
            if (packetHead == null)
            {
                throw new GameFrameworkException("Can not deserialize packet header.");
            }

            Type packetType = GetServerToClientPacketType(packetHead.Id);
            if (packetType == null)
            {
                PacketType pt = PacketType.Undefined;
                int pid = 0;
                GameEntry.Network.ParseOpCode(packetHead.Id, out pt, out pid);
                throw new GameFrameworkException(string.Format("Can not deserialize packet for packet type '{0}', packet id '{1}'.", pt.ToString(), pid.ToString()));
            }

            return (PacketBase)RuntimeTypeModel.Default.Deserialize(source, null, packetType);
        }

        private Type GetServerToClientPacketType(int opCode)
        {
            Type packetType = null;
            if (m_ServerToClientPacketTypes.TryGetValue(opCode, out packetType))
            {
                return packetType;
            }

            return null;
        }

        private void OnNetworkConnected(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkConnectedEventArgs ne = (UnityGameFramework.Runtime.NetworkConnectedEventArgs)e;
            Log.Info("Network channel '{0}' connected, local address '{1}:{2}', remote address '{3}:{4}'.", ne.NetworkChannel.Name, ne.NetworkChannel.LocalIPAddress, ne.NetworkChannel.LocalPort.ToString(), ne.NetworkChannel.RemoteIPAddress, ne.NetworkChannel.RemotePort.ToString());
        }

        private void OnNetworkClosed(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkClosedEventArgs ne = (UnityGameFramework.Runtime.NetworkClosedEventArgs)e;
            Log.Info("Network channel '{0}' closed.", ne.NetworkChannel.Name);
        }

        private void OnNetworkSendPacket(object sender, GameEventArgs e)
        {
            //UnityGameFramework.Runtime.NetworkSendPacketEventArgs ne = (UnityGameFramework.Runtime.NetworkSendPacketEventArgs)e;
        }

        private void OnNetworkMissHeartBeat(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs ne = (UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)e;
            Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

            if (ne.MissCount < 2)
            {
                return;
            }

            ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, GameEventArgs e)
        {
            UnityGameFramework.Runtime.NetworkErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkErrorEventArgs)e;
            Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);

            ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, GameEventArgs e)
        {
            //UnityGameFramework.Runtime.NetworkCustomErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkCustomErrorEventArgs)e;
        }
    }
}
