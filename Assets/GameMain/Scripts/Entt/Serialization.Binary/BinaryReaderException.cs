/*
  作者：LTH
  文件描述：
  文件名：BinaryReaderException
  创建时间：2023/07/16 21:07:SS
*/

using System;
using System.Runtime.Serialization;

namespace Entt.Serialization.Binary
{
    public class BinaryReaderException:Exception
    {
        public BinaryReaderException()
        {
        }

        protected BinaryReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BinaryReaderException(string message) : base(message)
        {
        }

        public BinaryReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}