/*
* 文件名：ClientPlayer
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:59:20
* 修改记录：
*/

using LiteNetLib.LiteNetLib.Protos;
using LiteNetLib.Test.Shared;
using LogicShared.LiteNetLib;
using LogicShared.LiteNetLib.Helpers;
using LogicShared.TrueSync.Math;
using UnityEngine;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 客户端玩家
    /// </summary>
    public class ClientPlayer : BasePlayer
    {
        private PlayerInputPacket _nextCommand;                 //玩家当前输入命令
        private readonly ClientLogic _clientLogic;              //玩家逻辑
        private readonly ClientPlayerManager _playerManager;    //玩家管理器
        private readonly LiteRingBuffer<PlayerInputPacket> _predictionPlayerStates; //预测玩家输入命令
        private ServerState _lastServerState;                   //最后一次服务器下发数据
        private const int MaxStoredCommands = 60;               //最多预测命令个数
        private bool _firstStateReceived;                       //是否成功接收过服务器命令
        private int _updateCount;                               //更新次数

        public FixVector2 LastPosition { get; private set; }       //玩家上一帧位置
        public float LastRotation { get; private set; }         //玩家上一帧旋转角度

        public int StoredCommands => _predictionPlayerStates.Count; //预测命令个数

        public ClientPlayer(ClientLogic clientLogic, ClientPlayerManager manager, string name, byte id) : base(manager, name, id)
        {
            _playerManager = manager;
            _predictionPlayerStates = new LiteRingBuffer<PlayerInputPacket>(MaxStoredCommands);
            _clientLogic = clientLogic;
        }

        //收到服务器信息
        public void ReceiveServerState(ServerState serverState, PlayerState ourState)
        {
            if (!_firstStateReceived)
            {
                if (serverState.LastProcessedCommand == 0)
                    return;
                _firstStateReceived = true;
            }
            //重复发送命令（判断依据是帧Id相同或最后一次处理的命令ID相同）
            if (serverState.Tick == _lastServerState.Tick || 
                serverState.LastProcessedCommand == _lastServerState.LastProcessedCommand)
                return;

            _lastServerState = serverState;

            //同步位置和旋转角度
            _position = ourState.Position;
            _rotation = ourState.Rotation;
            if (_predictionPlayerStates.Count == 0)
                return;

            //服务器最后一次处理的命令Id
            ushort lastProcessedCommand = serverState.LastProcessedCommand;
            int diff = NetworkGeneral.SeqDiff(lastProcessedCommand,_predictionPlayerStates.First.Id);
            //apply prediction  
            //服务器命令领先本地预测的命令的差值小于diff时，应用预测的结果
            if (diff >= 0 && diff < _predictionPlayerStates.Count)
            {
                //Debug.Log($"[OK]  SP: {serverState.LastProcessedCommand}, OUR: {_predictionPlayerStates.First.Id}, DF:{diff}");
                _predictionPlayerStates.RemoveFromStart(diff+1);   //先去掉已经无效的预测命令（之所以是无效，是因为在[_predictionPlayerStates.First,_predictionPlayerStates.First+diff]范围内的
                                                                        //服务器的命令Id已经到达了，自然而然的，该范围内的预测命令肯定失效了，因为直接用服务器的命令就可以了）
                foreach (var state in _predictionPlayerStates) //但是剩下的预测命令还是继续执行
                    ApplyInput(state, LogicTimer.FixedDelta);
            }
            else if(diff >= _predictionPlayerStates.Count)  //预测命令落后服务器命令，直接清空预测命令
            {
                Debug.Log($"[C] Player input lag st: {_predictionPlayerStates.First.Id} ls:{lastProcessedCommand} df:{diff}");
                //lag
                _predictionPlayerStates.FastClear();
                _nextCommand.Id = lastProcessedCommand;
            }
            else
            {
                Debug.LogError($"[ERR] SP: {serverState.LastProcessedCommand}, OUR: {_predictionPlayerStates.First.Id}, DF:{diff}, STORED: {StoredCommands}");
            }
        }
        
        //获取玩家输入
        public void SetInput(Vector2 velocity, float rotation, bool fire)
        {
            _nextCommand.Keys = 0;
            if(fire)
                _nextCommand.Keys |= MovementKeys.Fire;
            
            if (velocity.x < -0.5f)
                _nextCommand.Keys |= MovementKeys.Left;
            if (velocity.x > 0.5f)
                _nextCommand.Keys |= MovementKeys.Right;
            if (velocity.y < -0.5f)
                _nextCommand.Keys |= MovementKeys.Up;
            if (velocity.y > 0.5f)
                _nextCommand.Keys |= MovementKeys.Down;

            _nextCommand.Rotation = rotation;
        }

        //
        public override void Update(float delta)
        {
            LastPosition = _position;
            LastRotation = _rotation;
            
            _nextCommand.Id = (ushort)((_nextCommand.Id + 1) % NetworkGeneral.MaxGameSequence); //命令Id不能大于MaxGameSequence
            _nextCommand.ServerTick = _lastServerState.Tick;                                    //服务器帧Id
            ApplyInput(_nextCommand, delta);                                                    //本地使用命令
            if (_predictionPlayerStates.IsFull)                                                 //预测命令满了，
            {
                _nextCommand.Id = (ushort)(_lastServerState.LastProcessedCommand+1);            //设置当前玩家命令Id为最后一次服务器命令Id+1
                _predictionPlayerStates.FastClear();                                            //然后清空预测命令列表
            }
            _predictionPlayerStates.Add(_nextCommand);                                          //之后把当前命令保存到预测命令列表中

            _updateCount++;
            if (_updateCount == 3)                                                              //每更新3次，就向服务器发送该玩家所有的预测命令
            {
                _updateCount = 0;
                foreach (var t in _predictionPlayerStates)
                    _clientLogic.WritePacket(t, DeliveryMethod.Unreliable);
            }

            base.Update(delta);
        }
    }
}