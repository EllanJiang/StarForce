using System.Collections.Generic;
using System.Text;
using System;
namespace Protos
{
	public class PlayerDataTemp:BaseData
	{
		public int id;
		public float age;
		public bool sex;
		public long lv;
		public string[] myArray;
		public List<int> myList;
		public Dictionary<int,string> myDict;
		public E_HERO_TYPE myHeroType;
		public override int GetBytesNum()
		{
			int num = 0;
			num += 4;
			num += 4;
			num += 1;
			num += 8;
			num += 4;//这4个字节用来存储数组长度
			for (int i = 0; i < myArray.Length;i++)
			{
				num += 4 + Encoding.UTF8.GetByteCount(myArray[i]);
			}
			num += 4;//这4个字节用来存储列表长度
			for (int i = 0; i < myList.Count;i++)
			{
				num += 4;
			}
			num += 4;//这4个字节用来存储字典长度
			foreach (int key in myDict.Keys)
			{
				num += 4;
				num += 4 + Encoding.UTF8.GetByteCount(myDict[key]);
			}
			num += 4;
			return num;
		}
		public override byte[] Writing()
		{
			int index = 0;
			byte[] bytes = new byte[GetBytesNum()];
			WriteInt(bytes,id,ref index);;
			WriteFloat(bytes,age,ref index);;
			WriteBool(bytes,sex,ref index);;
			WriteLong(bytes,lv,ref index);;
			WriteInt(bytes,myArray.Length,ref index);
			for (int i = 0; i < myArray.Length;i++)
			{
				WriteString(bytes,myArray[i],ref index);
			}
			WriteInt(bytes,myList.Count,ref index);
			for (int i = 0; i < myList.Count;i++)
			{
				WriteInt(bytes,myList[i],ref index);
			}
			WriteInt(bytes,myDict.Count,ref index);
			foreach (int key in myDict.Keys)
			{
				WriteInt(bytes,key,ref index);
				WriteString(bytes,myDict[key],ref index);
			}
			WriteInt(bytes,Convert.ToInt32(myHeroType),ref index);;
			return bytes;
		}
		public override int Reading(byte[] bytes, int beginIndex = 0)
		{
			int index = beginIndex;
			id = ReadInt(bytes, ref index);
			age = ReadFloat(bytes, ref index);
			sex = ReadBool(bytes, ref index);
			lv = ReadLong(bytes, ref index);
			int myArrayLength = ReadInt(bytes, ref index);
			myArray = new string[myArrayLength];
			for (int i = 0; i < myArrayLength;i++)
			{
				myArray[i] = ReadString(bytes, ref index);
			}
			int myListCount = ReadInt(bytes, ref index);
			myList = new List<int>();
			for (int i = 0; i < myListCount;i++)
			{
				myList.Add(ReadInt(bytes, ref index));
			}
			int myDictCount = ReadInt(bytes, ref index);
			myDict = new Dictionary<int,string>();
			for (int i = 0; i < myDictCount;i++)
			{
				myDict.Add(ReadInt(bytes, ref index),ReadString(bytes, ref index));
			}
			myHeroType = (E_HERO_TYPE)ReadInt(bytes, ref index);
			return index - beginIndex;
		}
	}
}