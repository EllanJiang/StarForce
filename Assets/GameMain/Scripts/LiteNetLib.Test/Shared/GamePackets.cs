/*
* 文件名：GamePackets
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:00:19
* 修改记录：
*/

using System;
using LogicShared.LiteNetLib.Utils;
using LogicShared.TrueSync.Math;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 协议Id TODO 在proto里面确定
    /// </summary>
    public enum PacketType : byte
    {
        Movement,           //移动
        Spawn,              //生成
        ServerState,        //服务器状态
        Serialized,         //自动序列化的包
        Shoot               //射击
    }
    
    //Auto serializable packets
    /// <summary>
    /// 玩家请求加入房间
    /// </summary>
    public class JoinPacket
    {
        public string UserName { get; set; }
    }

    /// <summary>
    /// 同意玩家加入房间
    /// </summary>
    public class JoinAcceptPacket
    {
        public byte Id { get; set; }
        public ushort ServerTick { get; set; }
    }

    /// <summary>
    /// 通知玩家加入房间
    /// </summary>
    public class PlayerJoinedPacket
    {
        public string UserName { get; set; }
        public bool NewPlayer { get; set; }
        public byte Health { get; set; }
        public ushort ServerTick { get; set; }
        public PlayerState InitialPlayerState { get; set; }
    }

    /// <summary>
    /// 通知玩家离开房间
    /// </summary>
    public class PlayerLeavedPacket
    {
        public byte Id { get; set; }
    }

    //Manual serializable packets
    public struct SpawnPacket : INetSerializable
    {
        public long PlayerId;
        public FixVector2 Position;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerId);
            writer.Put(Position);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetLong();
            Position = reader.GetVector2();
        }
    }

    [Flags]
    public enum MovementKeys : byte
    {
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,
        Fire = 1 << 5
    }

    //开火Packet
    public struct ShootPacket : INetSerializable
    {
        public byte FromPlayer;
        public ushort CommandId;
        public FixVector2 Hit;
        public ushort ServerTick;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(FromPlayer);
            writer.Put(CommandId);
            writer.Put(Hit);
            writer.Put(ServerTick);
        }

        public void Deserialize(NetDataReader reader)
        {
            FromPlayer = reader.GetByte();
            CommandId = reader.GetUShort();
            Hit = reader.GetVector2();
            ServerTick = reader.GetUShort();
        }
    }
    
    public struct PlayerInputPacket : INetSerializable
    {
        public ushort Id;
        public MovementKeys Keys;
        public float Rotation;
        public ushort ServerTick;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put((byte)Keys);
            writer.Put(Rotation);
            writer.Put(ServerTick);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetUShort();
            Keys = (MovementKeys)reader.GetByte();
            Rotation = reader.GetFloat();
            ServerTick = reader.GetUShort();
        }
    }
    
    /// <summary>
    /// 玩家信息：Id，位置，Y轴旋转角度，当前帧Id
    /// </summary>
    public struct PlayerState : INetSerializable
    {
        public byte Id;
        public FixVector2 Position;
        public float Rotation;
        public ushort Tick;

        //玩家信息包体大小
        public const int Size = 1 + 8 + 4 + 2;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(Tick);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetByte();
            Position = reader.GetVector2();
            Rotation = reader.GetFloat();
            Tick = reader.GetUShort();
        }
    }

    //服务器信息
    public struct ServerState : INetSerializable
    {
        public ushort Tick;
        public ushort LastProcessedCommand;
        
        public int PlayerStatesCount;       //玩家数量
        public int StartState;              //server only
        public PlayerState[] PlayerStates;  //所有连入的玩家信息
        
        //tick 头文件大小=4
        public const int HeaderSize = sizeof(ushort)*2;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Tick);
            writer.Put(LastProcessedCommand);
            
            for (int i = 0; i < PlayerStatesCount; i++)
                PlayerStates[StartState + i].Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            Tick = reader.GetUShort();
            LastProcessedCommand = reader.GetUShort();
            
            PlayerStatesCount = reader.AvailableBytes / PlayerState.Size;
            if (PlayerStates == null || PlayerStates.Length < PlayerStatesCount)
                PlayerStates = new PlayerState[PlayerStatesCount];
            for (int i = 0; i < PlayerStatesCount; i++)
                PlayerStates[i].Deserialize(reader);
        }
    }
}