/*
* 文件名：AntiLagSystem
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:58:34
* 修改记录：
*/

using System.Collections.Generic;
using LogicShared.TrueSync.Math;


namespace LiteNetLib.Test.Server
{
    public struct StateInfo
    {
        public FixVector2 Position;
    }

    /// <summary>
    /// 抗延迟/抗滞后系统
    /// </summary>
    public class AntiLagSystem
    {
        private readonly Dictionary<int, StateInfo>[] _storedPositions;
        private readonly Dictionary<int, StateInfo> _savedStates;
        private int _currentArrayPos;
        private ushort _lastTick;
        private readonly int _maxTicks;

        public AntiLagSystem(int maxTicks, int maxPlayers)
        {
            int dictSize = (maxPlayers + 1)*3;

            _maxTicks = maxTicks;
            _storedPositions = new Dictionary<int, StateInfo>[maxTicks];
            _savedStates = new Dictionary<int, StateInfo>(dictSize);
            
            for (int i = 0; i < _storedPositions.Length; i++)
            {
                _storedPositions[i] = new Dictionary<int, StateInfo>(dictSize);
            }
        }

        private Dictionary<int, StateInfo> GetStates(ushort tick)
        {
            if (tick < _lastTick - _maxTicks || _lastTick < _maxTicks)
                return null;
            return _storedPositions[(_currentArrayPos - _lastTick + tick - 1) % _maxTicks];
        }

        public void StorePositions(ushort serverTick, ServerPlayer[] players)
        {
            var currentDict = _storedPositions[_currentArrayPos];
            currentDict.Clear();

            foreach (var p in players)
            {
                if (!p.IsAlive) 
                    continue;
                StateInfo si = new StateInfo
                {
                    Position = p.Position
                };
                currentDict.Add(p.AssociatedPeer.Id, si);
            }

            _lastTick = serverTick;
            _currentArrayPos = (_currentArrayPos + 1) % _maxTicks;
        }

        public bool TryApplyAntiLag(ServerPlayer[] players, ushort tick, int exceptId)
        {
            var antilagStates = GetStates(tick);
            if (antilagStates == null)
                return false;

            _savedStates.Clear();

            foreach (var p in players)
            {
                int id = p.AssociatedPeer.Id;
                if (id == exceptId) 
                    continue;
                //Save current states
                StateInfo state = new StateInfo
                {
                    Position = p.Position
                };
                //Console.WriteLine("Save state {0} = {1} {2}", id, state.Position, state.Pose);
                _savedStates[id] = state;

                //Apply antilag
                StateInfo antilagState;
                if (antilagStates.TryGetValue(id, out antilagState))
                {
                    //serverController.Player.ChangeState(antilagState.Position, antilagState.Pose, true);
                }
            }

            return true;
        }

        public void RevertAntiLag(ServerPlayer[] players)
        {
            //Revert states
            foreach (var p in players)
            {
                StateInfo state;
                if (_savedStates.TryGetValue(p.AssociatedPeer.Id, out state))
                {
                    //Console.WriteLine("Load state {0} = {1} {2}", serverController.ServerId, state.Position, state.Pose);
                    //p.ChangeState(state.Position, state.Pose, true);
                }
            }
        }
    }
}