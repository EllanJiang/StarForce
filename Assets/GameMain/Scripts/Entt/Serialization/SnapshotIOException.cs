/*
  作者：LTH
  文件描述：
  文件名：SnapshotIOException
  创建时间：2023/07/15 22:07:SS
*/

using System;
using System.IO;
using System.Runtime.Serialization;

namespace Entt.Serialization
{
    /// <summary>
    /// 快照IO异常
    /// </summary>
    public class SnapshotIOException:IOException
    {
        public SnapshotIOException()
        {
        }
        
        protected SnapshotIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SnapshotIOException(string message) : base(message)
        {
        }

        public SnapshotIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SnapshotIOException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}