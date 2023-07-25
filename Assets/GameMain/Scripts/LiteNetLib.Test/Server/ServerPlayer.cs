/*
* 文件名：ServerPlayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:07:19
* 修改记录：
*/

using LiteNetLib.Test.Shared;
using LogicShared.LiteNetLib;
using UnityEngine;

namespace LiteNetLib.Test.Server
{
   /// <summary>
   /// 连入的客户端在服务端的代表
   /// </summary>
   public class ServerPlayer : BasePlayer
   {
       private readonly ServerPlayerManager _playerManager;
       public readonly NetPeer AssociatedPeer; //关联的Peer
       public PlayerState NetworkState;        //玩家信息
       public ushort LastProcessedCommandId { get; private set; }
   
       public ServerPlayer(ServerPlayerManager playerManager, string name, NetPeer peer) : base(playerManager, name, (byte)peer.Id)
       {
           _playerManager = playerManager;
           peer.Tag = this;
           AssociatedPeer = peer;
           NetworkState = new PlayerState {Id = (byte) peer.Id};
       }

       //设置客户端新的位置
       public override void ApplyInput(PlayerInputPacket command, float delta)
       {
           //命令落后了，直接丢弃
           if (NetworkGeneral.SeqDiff(command.Id, LastProcessedCommandId) <= 0)
           {
               return;
           }
           LastProcessedCommandId = command.Id;
           base.ApplyInput(command, delta);
       }

       //更新服务器玩家位置和旋转角度
       public override void Update(float delta)
       {
           base.Update(delta);
           NetworkState.Position = _position;
           NetworkState.Rotation = _rotation;
           NetworkState.Tick = LastProcessedCommandId;
       
           //Draw cross as server player
           const float sz = 0.1f;
           Debug.DrawLine(
               new Vector2(Position.x - sz, Position.y ),
               new Vector2(Position.x + sz, Position.y ), 
               Color.white);
           Debug.DrawLine(
               new Vector2(Position.x, Position.y - sz ),
               new Vector2(Position.x, Position.y + sz ), 
               Color.white);
       
       }
   }
}