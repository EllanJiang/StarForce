/*
* 文件名：NetDataWriter
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/21 15:40:47
* 修改记录：
*/

using System;
using System.Net;
using System.Text;

namespace LogicShared.LiteNetLib.Utils
{
    /// <summary>
    /// 数据写入器
    /// </summary>
    public class NetDataWriter
    {
        protected byte[] _data;                 //原始数据
        protected int _position;                //有效数据终点位置,即[0,_position]之间的数据是有效的
        private const int InitialSize = 64;     //原始数据默认大小
        private readonly bool _autoResize;      //自动重置原始数据

        /// <summary>
        /// 字节数组容量
        /// </summary>
        public int Capacity
        {
            get { return _data.Length; }
        }

        public NetDataWriter() : this(true, InitialSize)
        {
        }

        public NetDataWriter(bool autoResize) : this(autoResize, InitialSize)
        {
        }

        public NetDataWriter(bool autoResize, int initialSize)
        {
            _data = new byte[initialSize];
            _autoResize = autoResize;
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="copy">Copy array to new location or use existing</param>
        public static NetDataWriter FromBytes(byte[] bytes, bool copy)
        {
            if (copy)
            {
                var netDataWriter = new NetDataWriter(true, bytes.Length);
                netDataWriter.Put(bytes);
                return netDataWriter;
            }
            return new NetDataWriter(true, 0) {_data = bytes};
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray (always copied data)
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="offset">Offset of array</param>
        /// <param name="length">Length of array</param>
        public static NetDataWriter FromBytes(byte[] bytes, int offset, int length)
        {
            var netDataWriter = new NetDataWriter(true, bytes.Length);
            netDataWriter.Put(bytes, offset, length);
            return netDataWriter;
        }

        public static NetDataWriter FromString(string value)
        {
            var netDataWriter = new NetDataWriter();
            netDataWriter.Put(value);
            return netDataWriter;
        }

        /// <summary>
        /// 重置数组大小，按照原始大小2的倍数递增
        /// </summary>
        /// <param name="newSize"></param>
        public void ResizeIfNeed(int newSize)
        {
            int len = _data.Length;
            if (len < newSize)
            {
                while (len < newSize)
                    len *= 2;
                Array.Resize(ref _data, len);
            }
        }

        public void Reset(int size)
        {
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset()
        {
            _position = 0;
        }

        
        public byte[] CopyData()
        {
            byte[] resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        public byte[] Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 有效数据长度
        /// </summary>
        public int Length
        {
            get { return _position; }
        }
        
        public void Put(float value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(double value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(long value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(ulong value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(int value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(uint value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(char value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(ushort value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(short value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(sbyte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)value;
            _position++;
        }

        public void Put(byte value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }

        public void Put(byte[] data, int offset, int length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void Put(byte[] data)
        {
            if (_autoResize)
                ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }
        
        public void PutSBytesWithLength(sbyte[] data, int offset, int length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + length + 4);
            FastBitConverter.GetBytes(_data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 4, length);
            _position += length + 4;
        }
        
        public void PutSBytesWithLength(sbyte[] data)
        {
            if (_autoResize)
                ResizeIfNeed(_position + data.Length + 4);
            FastBitConverter.GetBytes(_data, _position, data.Length);
            Buffer.BlockCopy(data, 0, _data, _position + 4, data.Length);
            _position += data.Length + 4;
        }

        /// <summary>
        /// 不仅保存数组，还保存数组长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void PutBytesWithLength(byte[] data, int offset, int length)
        {
            if (_autoResize)
                ResizeIfNeed(_position + length + 4);
            FastBitConverter.GetBytes(_data, _position, length);
            Buffer.BlockCopy(data, offset, _data, _position + 4, length);
            _position += length + 4;
        }

        public void PutBytesWithLength(byte[] data)
        {
            if (_autoResize)
                ResizeIfNeed(_position + data.Length + 4);
            FastBitConverter.GetBytes(_data, _position, data.Length);
            Buffer.BlockCopy(data, 0, _data, _position + 4, data.Length);
            _position += data.Length + 4;
        }

        public void Put(bool value)
        {
            if (_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)(value ? 1 : 0);
            _position++;
        }

        private void PutArray(Array arr, int size)
        {
            ushort length = arr == null ? (ushort) 0 : (ushort)arr.Length;
            size *= length;
            if (_autoResize)
                ResizeIfNeed(_position + size + 2);
            FastBitConverter.GetBytes(_data, _position, length);
            if (arr != null)
                Buffer.BlockCopy(arr, 0, _data, _position + 2, size);
            _position += size + 2;
        }

        public void PutArray(float[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(double[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(long[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(ulong[] value)
        {
            PutArray(value, 8);
        }

        public void PutArray(int[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(uint[] value)
        {
            PutArray(value, 4);
        }

        public void PutArray(ushort[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(short[] value)
        {
            PutArray(value, 2);
        }

        public void PutArray(bool[] value)
        {
            PutArray(value, 1);
        }

        public void PutArray(string[] value)
        {
            ushort len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for (int i = 0; i < len; i++)
                Put(value[i]);
        }

        public void PutArray(string[] value, int maxLength)
        {
            ushort len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for (int i = 0; i < len; i++)
                Put(value[i], maxLength);
        }

        public void Put(IPEndPoint endPoint)
        {
            Put(endPoint.Address.ToString());
            Put(endPoint.Port);
        }

        public void Put(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            //put bytes count
            int bytesCount = Encoding.UTF8.GetByteCount(value);
            if (_autoResize)
                ResizeIfNeed(_position + bytesCount + 4);
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, value.Length, _data, _position);
            _position += bytesCount;
        }

        public void Put(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            int length = value.Length > maxLength ? maxLength : value.Length;
            //calculate max count
            int bytesCount = Encoding.UTF8.GetByteCount(value);
            if (_autoResize)
                ResizeIfNeed(_position + bytesCount + 4);

            //put bytes count
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, length, _data, _position);

            _position += bytesCount;
        }

        public void Put<T>(T obj) where T : INetSerializable
        {
            obj.Serialize(this);
        }
    }
}