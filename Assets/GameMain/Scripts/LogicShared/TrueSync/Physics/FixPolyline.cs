/*
* 文件名：FPPolyline
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:57:47
* 修改记录：
*/

using System;
using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    [Serializable]
    public struct FixKeypoint
    {
        public Fix64 time;
        public Fix64 value;
    }

    /// <summary>
    /// 折线,FPCurve的简易版本，只保存关键帧
    /// </summary>
    [Serializable]
    public class FixPolyline
    {
        public FixKeypoint[] keys;

        public bool empty => !(keys != null && keys.Length > 0);

        public Fix64 Evaluate(Fix64 t)
        {
            if (keys == null)
                return Fix64.Zero;
            if (keys.Length <= 0)
                return Fix64.Zero;

            if (keys[keys.Length - 1].time <= t)
            {
                return keys[keys.Length - 1].value;
            }

            Fix64 segStartTime = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                Fix64 segEndTime = keys[i].time;
                if (t < segEndTime)
                {
                    return FixMath.Lerp(
                        i == 0 ? Fix64.Zero : keys[i - 1].value,
                        keys[i].value,
                        (t - segStartTime) / (segEndTime - segStartTime));
                }

                segStartTime = segEndTime;
            }

            return keys[keys.Length - 1].value;
        }

        public Fix64 EvaluateDelta(Fix64 t1, Fix64 t2)
        {
            return Evaluate(t2) - Evaluate(t1);
        }

    }
}