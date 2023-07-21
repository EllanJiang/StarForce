/*
* 文件名：ConnectionRequest
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 20:33:42
* 修改记录：
*/

using System.Net;
using System.Threading;
using LogicShared.LiteNetLib.Utils;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// 连接请求结果类型
    /// </summary>
    internal enum ConnectionRequestResult
    {
        None,
        Accept,
        Reject,
        RejectForce
    }
    
    /// <summary>
    /// 连接请求包
    /// </summary>
    public class ConnectionRequest
    {
        private readonly NetManager _listener;
        private int _used;

        public readonly NetDataReader Data;

        internal ConnectionRequestResult Result { get; private set; }
        internal long ConnectionTime;
        internal byte ConnectionNumber;
        public readonly IPEndPoint RemoteEndPoint;
        
        //尝试激活连接
        private bool TryActivate()
        {
            //Compares two values for equality and, if they are equal, replaces the first value.
            //如果_used == 0 ，则将_used的值设为1
            //返回值：返回_used的原始值
            return Interlocked.CompareExchange(ref _used, 1, 0) == 0;
        }
        
        //更新请求连接数据（连接id和连接次数）
        internal void UpdateRequest(NetConnectRequestPacket connRequest)
        {
            if (connRequest.ConnectionId >= ConnectionTime)
            {
                ConnectionTime = connRequest.ConnectionId;
                ConnectionNumber = connRequest.ConnectionNumber;
            }
        }
        
        internal ConnectionRequest(
            long connectionId,
            byte connectionNumber,
            NetDataReader netDataReader,
            IPEndPoint endPoint,
            NetManager listener)
        {
            ConnectionTime = connectionId;
            ConnectionNumber = connectionNumber;
            RemoteEndPoint = endPoint;
            Data = netDataReader;
            _listener = listener;
        }
        
        /// <summary>
        /// 如果Key相等，则同意连接请求
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public NetPeer AcceptIfKey(string key)
        {
            if (!TryActivate())
                return null;
            try
            {
                if (Data.GetString() == key)
                    Result = ConnectionRequestResult.Accept;
            }
            catch
            {
                Logger.Error("[AC] Invalid incoming data");
            }
            if (Result == ConnectionRequestResult.Accept)
                return _listener.OnConnectionSolved(this, null, 0, 0);

            Result = ConnectionRequestResult.Reject;
            _listener.OnConnectionSolved(this, null, 0, 0);
            return null;
        }

        /// <summary>
        /// Accept connection and get new NetPeer as result
        /// </summary>
        /// <returns>Connected NetPeer</returns>
        public NetPeer Accept()
        {
            if (!TryActivate())
                return null;
            Result = ConnectionRequestResult.Accept;
            return _listener.OnConnectionSolved(this, null, 0, 0);
        }
        
        public void Reject(byte[] rejectData, int start, int length, bool force)
        {
            if (!TryActivate())
                return;
            Result = force ? ConnectionRequestResult.RejectForce : ConnectionRequestResult.Reject;
            _listener.OnConnectionSolved(this, rejectData, start, length);
        }

        public void Reject(byte[] rejectData, int start, int length)
        {
            Reject(rejectData, start, length, false);
        }


        public void RejectForce(byte[] rejectData, int start, int length)
        {
            Reject(rejectData, start, length, true);
        }

        public void RejectForce()
        {
            Reject(null, 0, 0, true);
        }

        public void RejectForce(byte[] rejectData)
        {
            Reject(rejectData, 0, rejectData.Length, true);
        }

        public void RejectForce(NetDataWriter rejectData)
        {
            Reject(rejectData.Data, 0, rejectData.Length, true);
        }

        public void Reject()
        {
            Reject(null, 0, 0, false);
        }

        public void Reject(byte[] rejectData)
        {
            Reject(rejectData, 0, rejectData.Length, false);
        }

        public void Reject(NetDataWriter rejectData)
        {
            Reject(rejectData.Data, 0, rejectData.Length, false);
        }
    }
}