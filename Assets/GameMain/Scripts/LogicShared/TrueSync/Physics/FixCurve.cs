/*
* 文件名：FPCurve
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 15:49:23
* 修改记录：
*/

using LogicShared.TrueSync.Math;

namespace LogicShared.TrueSync.Physics
{
    [System.Flags]
    [System.Serializable]
    public enum FixWeightedMode
    {
        None = 0,
        In = 1 << 1,
        Out = 1 << 2,
        Both = In | Out,
    }

    /// <summary>
    /// UnityEngine.Keyframe made with Fix64
    /// </summary>
    [System.Serializable]
    public struct FixKeyframe
    {
        public FixWeightedMode weightedMode;
        public Fix64 time;
        public Fix64 value;
        public Fix64 inSlope;
        public Fix64 outSlope;
        public Fix64 inWeight;
        public Fix64 outWeight;

    }

    /// <summary>
    /// UnityEngine.AnimationCurve made with Fix64
    /// </summary>
    [System.Serializable]
    public class FixCurve
    {
        public static readonly Fix64 FPCurveEpsilon = Fix64.EN3;
        public static readonly Fix64 FPCurveDefaultWeight = (Fix64)1 / (Fix64)3;
        public static readonly Fix64 FPPiOver3 = Fix64.Pi / (Fix64)3;

        public FixKeyframe[] frames;
        
        public Fix64 Evaluate(Fix64 t)
        {
            if (frames == null)
                return Fix64.Zero;
            if (frames.Length == 0)
                return Fix64.Zero;

            Fix64 beginTime = frames[0].time;
            Fix64 endTime = frames[frames.Length - 1].time;

            if (t >= endTime)
            {
                return frames[frames.Length - 1].value;
            }
            else if (t <= beginTime)
            {
                return frames[0].value;
            }
            else
            {
                int prevIndex = 0, postIndex = 0;
                FindIndexForSampling(t, out prevIndex, out postIndex);
                ref FixKeyframe prevFrame = ref frames[prevIndex];
                ref FixKeyframe postFrame = ref frames[postIndex];
                if ((prevFrame.weightedMode & FixWeightedMode.Out) == FixWeightedMode.Out || (postFrame.weightedMode & FixWeightedMode.In) == FixWeightedMode.In)
                {
                    return InterpolateKeyframe(ref prevFrame, ref postFrame, t);
                }
                else
                {
                    int index;
                    Fix64 time;
                    Fix64 timeEnd;
                    Fix64 coeff0;
                    Fix64 coeff1;
                    Fix64 coeff2;
                    Fix64 coeff3;

                    index = prevIndex;
                    time = prevFrame.time;
                    timeEnd = postFrame.time;

                    Fix64 dx, length;
                    Fix64 dy;
                    Fix64 m1, m2, d1, d2;

                    dx = postFrame.time - prevFrame.time;
                    dx = FixMath.Max(dx, Fix64.EN4);
                    dy = postFrame.value - prevFrame.value;
                    length = 1.0F / (dx * dx);

                    m1 = prevFrame.outSlope;
                    m2 = postFrame.inSlope;
                    d1 = m1 * dx;
                    d2 = m2 * dx;

                    coeff0 = (d1 + d2 - dy - dy) * length / dx;
                    coeff1 = (dy + dy + dy - d1 - d1 - d2) * length;
                    coeff2 = m1;
                    coeff3 = prevFrame.value;

                    if (prevFrame.outSlope == Fix64.MaxValue || postFrame.inSlope == Fix64.MaxValue)
                    {
                        coeff0 = 0.0F;
                        coeff1 = 0.0F;
                        coeff2 = 0.0F;
                        coeff3= prevFrame.value;
                    }

                    t = t - time;
                    return (t * (t * (t * coeff0 + coeff1) + coeff2)) + coeff3;
                }
            }

            //return Fix64.Zero;
        }

        public void FindIndexForSampling(Fix64 t, out int prevIndex, out int postIndex)
        {
            int foundIndex = 0;
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].time <= t)
                {
                    foundIndex = i;
                }
                else
                {
                    break;
                }
            }

            prevIndex = foundIndex;
            postIndex = foundIndex + 1;
            if (postIndex >= frames.Length)
            {
                postIndex = frames.Length - 1;
            }
        }

        public Fix64 InterpolateKeyframe(ref FixKeyframe prevFrame, ref FixKeyframe postFrame, Fix64 t)
        {
            Fix64 output;

            if ((prevFrame.weightedMode & FixWeightedMode.Out) == FixWeightedMode.Out || (postFrame.weightedMode & FixWeightedMode.In) == FixWeightedMode.In)
                output = BezierInterpolate(ref prevFrame, ref postFrame, t);
            else
                output = HermiteInterpolate(ref prevFrame, ref postFrame, t);

            if (prevFrame.outSlope == Fix64.MaxValue || postFrame.inSlope == Fix64.MaxValue)
                output = prevFrame.value;

            return output;
        }

        #region Hermite stuff
        protected Fix64 HermiteInterpolate(ref FixKeyframe prevFrame, ref FixKeyframe postFrame, Fix64 t)
        {
            Fix64 dx = postFrame.time - prevFrame.time;
            Fix64 m1;
            Fix64 m2;
            if (dx != Fix64.Zero)
            {
                t = (t - prevFrame.time) / dx;
                m1 = prevFrame.outSlope * dx;
                m2 = postFrame.inSlope * dx;
            }
            else
            {
                t = Fix64.Zero;
                m1 = Fix64.Zero;
                m2 = Fix64.Zero;
            }

            return HermiteInterpolate(t, prevFrame.value, m1, m2, postFrame.value);
        }
        // 使用两点三次Hermite多项式计算时间t的坐标。p0对应y0, p1对应y1, m0 是y0的导数，m1 是y1的导数
        protected Fix64 HermiteInterpolate(Fix64 t, Fix64 p0, Fix64 m0, Fix64 m1, Fix64 p1)
        {
            Fix64 t2 = t * t;
            Fix64 t3 = t2 * t;

            Fix64 a = 2 * t3 - 3 * t2 + Fix64.One;  // y0 对应的系数
            Fix64 b = t3 - 2 * t2 + t;              // y0 导数对应的系数
            Fix64 c = t3 - t2;                      // y1 导数对应的系数
            Fix64 d = -2 * t3 + 3 * t2;             // y1 对应的系数

            return a * p0 + b * m0 + c * m1 + d * p1;
        }
        #endregion

        #region Bezier stuff
        protected Fix64 BezierInterpolate(ref FixKeyframe prevFrame, ref FixKeyframe postFrame, Fix64 t)
        {
            Fix64 prevOutWeight = (prevFrame.weightedMode & FixWeightedMode.Out) == FixWeightedMode.Out ? prevFrame.outWeight : FPCurveDefaultWeight;
            Fix64 postInWeight = (postFrame.weightedMode & FixWeightedMode.In) == FixWeightedMode.In ? postFrame.inWeight : FPCurveDefaultWeight;

            Fix64 dx = postFrame.time - prevFrame.time;
            if (dx == Fix64.Zero)
                return prevFrame.value;
            // prevFrame.outSlope表示第一条线段的斜率，postFrame.inSlope表示最后一条线段的斜率，中间线段由新生成的两个顶点构成
            // (t - prevFrame.time) / dx 将时间t变换到(0,1)区间
            return BezierInterpolate((t - prevFrame.time) / dx, prevFrame.value, prevFrame.outSlope * dx, prevOutWeight, postFrame.value, postFrame.inSlope * dx, postInWeight);
        }
        // 将两个坐标点转换为4个点，然后使用贝塞尔多项式计算时间t对应的值
        protected Fix64 BezierInterpolate(Fix64 t, Fix64 v1, Fix64 m1, Fix64 w1, Fix64 v2, Fix64 m2, Fix64 w2)
        {
            Fix64 u = BezierExtractU(t, w1, Fix64.One - w2);    // 不明白为什么要使用如此复杂的算法来修正时间t
            return BezierInterpolate(u, v1, w1 * m1 + v1, v2 - w2 * m2, v2);
        }

        /*protected Fix64 FAST_CBRT_POSITIVE(Fix64 x)
        {
            //(float)exp(log(x) / 3);
        }

        protected Fix64 FAST_CBRT(Fix64 x)
        {
            //(float)(((x) < 0) ? -exp(log(-(x)) / 3) : exp(log(x) / 3));
        }*/

        protected Fix64 BezierExtractU(Fix64 t, Fix64 w1, Fix64 w2)
        {
            //System.Diagnostics.Debug.Assert(t >= Fix64.Zero && t <= Fix64.One);

            Fix64 a = 3 * w1 - 3 * w2 + Fix64.One;
            Fix64 b = -6 * w1 + 3 * w2;
            Fix64 c = 3 * w1;
            Fix64 d = -t;

            if (FixMath.Abs(a) > Fix64.Epsilon)
            {
                Fix64 p = -b / (3 * a);
                Fix64 p2 = p * p;
                Fix64 p3 = p2 * p;

                Fix64 q = p3 + (b * c - 3 * a * d) / (6 * a * a);
                Fix64 q2 = q * q;

                Fix64 r = c / (3 * a);
                Fix64 rmp2 = r - p2;

                Fix64 s = q2 + rmp2 * rmp2 * rmp2;

                if (s < 0)
                {
                    Fix64 ssi = FixMath.Sqrt(-s);
                    Fix64 r2 = FixMath.Sqrt(-s + q2);
                    Fix64 phi = FixMath.Atan2(ssi, q);

                    Fix64 r_3 = Fix64.Pow(r2, Fix64.One / 3);//FAST_CBRT_POSITIVE(r2);
                    Fix64 phi_3 = phi / 3;

                    // Extract cubic roots.
                    Fix64 u1 = 2 * r_3 * FixMath.Cos(phi_3) + p;
                    Fix64 u2 = 2 * r_3 * FixMath.Cos(phi_3 + 2 * FPPiOver3) + p;
                    Fix64 u3 = 2 * r_3 * FixMath.Cos(phi_3 - 2 * FPPiOver3) + p;

                    if (u1 >= Fix64.Zero && u1 <= Fix64.One)
                        return u1;
                    else if (u2 >= Fix64.Zero && u2 <= Fix64.One)
                        return u2;
                    else if (u3 >= Fix64.Zero && u3 <= Fix64.One)
                        return u3;

                    // Aiming at solving numerical imprecisions when u is outside [0,1].
                    return (t < Fix64.Half) ? Fix64.Zero : Fix64.One;
                }
                else
                {
                    Fix64 ss = FixMath.Sqrt(s);
                    Fix64 u = Fix64.Pow(q + ss, Fix64.One / 3) + Fix64.Pow(q - ss, Fix64.One / 3) + p;

                    if (u >= Fix64.Zero && u <= Fix64.One)
                        return u;

                    // Aiming at solving numerical imprecisions when u is outside [0,1].
                    return (t < Fix64.Half) ? Fix64.Zero : Fix64.One;
                }
            }
            if (FixMath.Abs(b) > Fix64.Epsilon)
            {
                Fix64 s = c * c - 4 * b * d;
                Fix64 ss = FixMath.Sqrt(s);

                Fix64 u1 = (-c - ss) / (2 * b);
                Fix64 u2 = (-c + ss) / (2 * b);

                if (u1 >= Fix64.Zero && u1 <= Fix64.One)
                    return u1;
                else if (u2 >= Fix64.Zero && u2 <= Fix64.One)
                    return u2;

                // Aiming at solving numerical imprecisions when u is outside [0,1].
                return (t < Fix64.Half) ? Fix64.Zero : Fix64.One;
            }
            if (FixMath.Abs(c) > Fix64.Epsilon)
            {
                return (-d / c);
            }

            return Fix64.Zero;
        }
        // 使用贝塞尔多项式计算时间t对应的坐标
        protected Fix64 BezierInterpolate(Fix64 t, Fix64 p0, Fix64 p1, Fix64 p2, Fix64 p3)
        {
            Fix64 t2 = t * t;
            Fix64 t3 = t2 * t;
            Fix64 omt = Fix64.One - t;
            Fix64 omt2 = omt * omt;
            Fix64 omt3 = omt2 * omt;

            return omt3 * p0 + 3 * t * omt2 * p1 + 3 * t2 * omt * p2 + t3 * p3;
        }
        #endregion
    }
}