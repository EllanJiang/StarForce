/*
* 文件名：NetUtils
* 文件描述：工具类
* 作者：aronliang
* 创建时间：2023/07/20 20:10:51
* 修改记录：
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace LogicShared.LiteNetLib
{
    /// <summary>
    /// Address type that you want to receive from NetUtils.GetLocalIp method
    /// </summary>
    [Flags]
    public enum LocalAddrType
    {
        IPv4 = 1,
        IPv6 = 2,
        All = IPv4 | IPv6
    }
    
    public static class NetUtils
    {
        //创建IPEndPoint
        public static IPEndPoint MakeEndPoint(string hostStr, int port)
        {
            return new IPEndPoint(ResolveAddress(hostStr), port);
        }
        
        //解析IP地址
        public static IPAddress ResolveAddress(string hostStr)
        {
            if(hostStr == "localhost")
                return IPAddress.Loopback;
            
            IPAddress ipAddress;
            if (!IPAddress.TryParse(hostStr, out ipAddress))
            {
                if (NetSocket.IPv6Support)
                    ipAddress = ResolveAddress(hostStr, AddressFamily.InterNetworkV6);
                if (ipAddress == null)
                    ipAddress = ResolveAddress(hostStr, AddressFamily.InterNetwork);
            }
            if (ipAddress == null)
                throw new ArgumentException("Invalid address: " + hostStr);

            return ipAddress;
        }
        
        private static IPAddress ResolveAddress(string hostStr, AddressFamily addressFamily)
        {
            IPAddress[] addresses = ResolveAddresses(hostStr);
            foreach (IPAddress ip in addresses)
            {
                if (ip.AddressFamily == addressFamily)
                {
                    return ip;
                }
            }
            return null;
        }
        
        private static IPAddress[] ResolveAddresses(string hostStr)
        {
#if NETSTANDARD || NETCOREAPP
            var hostTask = Dns.GetHostEntryAsync(hostStr);
            hostTask.GetAwaiter().GetResult();
            var host = hostTask.Result;
#else
            var host = Dns.GetHostEntry(hostStr);
#endif
            return host.AddressList;
        }
        
        /// <summary>
        /// 获取所有本地IP地址
        /// </summary>
        /// <param name="addrType">type of address (IPv4, IPv6 or both)</param>
        /// <returns>List with all local ip addresses</returns>
        public static List<string> GetLocalIpList(LocalAddrType addrType)
        {
            List<string> targetList = new List<string>();
            GetLocalIpList(targetList, addrType);
            return targetList;
        }
        
        /// <summary>
        /// Get all local ip addresses (non alloc version)
        /// 获取IP地址列表
        /// </summary>
        /// <param name="targetList">result list</param>
        /// <param name="addrType">type of address (IPv4, IPv6 or both)</param>
        public static void GetLocalIpList(IList<string> targetList, LocalAddrType addrType)
        {
            bool ipv4 = (addrType & LocalAddrType.IPv4) == LocalAddrType.IPv4;
            bool ipv6 = (addrType & LocalAddrType.IPv6) == LocalAddrType.IPv6;
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    //Skip loopback and disabled network interfaces
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback || 
                        ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    var ipProps = ni.GetIPProperties();

                    //Skip address without gateway
                    if (ipProps.GatewayAddresses.Count == 0)
                        continue;

                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        var address = ip.Address;
                        if ((ipv4 && address.AddressFamily == AddressFamily.InterNetwork) ||
                            (ipv6 && address.AddressFamily == AddressFamily.InterNetworkV6))
                            targetList.Add(address.ToString());
                    }
                }
            }
            catch
            {
                //ignored
            }

            //Fallback mode (unity android)
            if (targetList.Count == 0)
            {
                IPAddress[] addresses = ResolveAddresses(Dns.GetHostName());
                foreach (IPAddress ip in addresses)
                {
                    if((ipv4 && ip.AddressFamily == AddressFamily.InterNetwork) ||
                       (ipv6 && ip.AddressFamily == AddressFamily.InterNetworkV6))
                        targetList.Add(ip.ToString());
                }
            }
            if (targetList.Count == 0)
            {
                if(ipv4)
                    targetList.Add("127.0.0.1");
                if(ipv6)
                    targetList.Add("::1");
            }
        }
        
        private static readonly List<string> IpList = new List<string>();
        /// <summary>
        /// Get first detected local ip address
        /// 获取第一个类型为addrType的IP地址
        /// </summary>
        /// <param name="addrType">type of address (IPv4, IPv6 or both)</param>
        /// <returns>IP address if available. Else - string.Empty</returns>
        public static string GetLocalIp(LocalAddrType addrType)
        {
            lock (IpList)
            {
                IpList.Clear();
                GetLocalIpList(IpList, addrType);
                return IpList.Count == 0 ? string.Empty : IpList[0];
            }
        }
        
        //获取number和expected的差值
        //为什么不直接用number - expected？
        //因为要处理某些特殊情况：例如 number = 1，expected = NetConstants.MaxSequence - 1.这种情况下，直接用number - expected得出的结果是错误的（结果是2-NetConstants.MaxSequence）
        //正确的结果应该是2，这是因为SequenceNumber最大值只能是NetConstants.MaxSequence，一旦SequenceNumber超过最大值，就会掉过头来，从0开始计数，因此number和expected的差值之间的差值就是2
        internal static int RelativeSequenceNumber(int number, int expected)
        {
            return (number - expected + NetConstants.MaxSequence + NetConstants.HalfMaxSequence) % NetConstants.MaxSequence - NetConstants.HalfMaxSequence;
        }
    }
}