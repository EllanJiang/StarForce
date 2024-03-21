/*
* 文件名：BasePlayerManager
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 11:05:13
* 修改记录：
*/

using System.Collections;
using System.Collections.Generic;
using LogicShared.TrueSync.Math;
using UnityEngine;

namespace LiteNetLib.Test.Shared
{
    /// <summary>
    /// 玩家管理器基类
    /// </summary>
    public abstract class BasePlayerManager : IEnumerable<BasePlayer>
    {
        public abstract IEnumerator<BasePlayer> GetEnumerator();
        public abstract int Count { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        //发起射线，用于判断是否射到敌人
        public BasePlayer CastToPlayer(FixVector2 from, FixVector2 dir, float length, BasePlayer exclude)
        {
            BasePlayer result = null;
            FixVector2 target = from + dir * length;
            foreach(var p in this)  //遍历所有玩家
            {
                if(p == exclude)
                    continue;
                //如果相交，那么返回该玩家
                if (Collisions.CheckIntersection(from.x, from.y, target.x, target.y, p))
                {
                    //TODO: check near
                    if(result == null)
                        result = p;
                }
            }
            
            return result;
        }

        public abstract void LogicUpdate();
        //射击玩家
        public abstract void OnShoot(BasePlayer from, FixVector2 to, BasePlayer hit);
    }
}