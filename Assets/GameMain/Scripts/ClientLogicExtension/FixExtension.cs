/*
* 文件名：FixExtenion
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 19:05:43
* 修改记录：
*/

using LogicShared.TrueSync.Math;
using LogicShared.TrueSync.Physics;
using UnityEngine;

namespace GameMain
{
    public static class FixExtension
    {
        public static FixAABB ToFixAABB(this FixAABB aabb, Bounds bounds)
        {
            var newAABB = new FixAABB()
            {
                Min = new FixVector(bounds.min.x, bounds.min.y, bounds.min.z),
                Max = new FixVector(bounds.max.x, bounds.max.y, bounds.max.z)
            };
            return newAABB;
        }

        public static Bounds ToBounds(this FixAABB aabb)
        {
            return new Bounds((aabb.Min.ToVector() + aabb.Max.ToVector()) / 2f,aabb.Max.ToVector() - aabb.Min.ToVector());
        }

        public static Vector3 ToVector(this FixVector vector)
        {
            return new Vector3(vector.x.AsFloat(), vector.y.AsFloat(), vector.z.AsFloat());
        }
        
        public static Vector3 ToVector(this FixVector2 vector)
        {
            return new Vector3(vector.x.AsFloat(), vector.y.AsFloat());
        }
        
        public static Vector2 ToVector2(this FixVector vector)
        {
            return new Vector2(vector.x.AsFloat(), vector.y.AsFloat());
        }
        
        public static Vector2 ToVector2(this FixVector2 vector)
        {
            return new Vector2(vector.x.AsFloat(), vector.y.AsFloat());
        }


        public static Quaternion ToQuaternion(this FixQuaternion quaternion)
        {
            return new Quaternion(quaternion.x.AsFloat(), quaternion.y.AsFloat(), quaternion.z.AsFloat(),
                quaternion.w.AsFloat());
        }
        
        public static FixQuaternion ToFixQuaternion(this Quaternion quaternion)
        {
            return new FixQuaternion(quaternion.x, quaternion.y, quaternion.z,
                quaternion.w);
        }

        public static FixVector ToFixVector(this Vector3 vector)
        {
            return new FixVector(vector.x, vector.y, vector.z);
        }
        
        public static FixMatrix ToFixMatrix(this Matrix4x4 matrix)
        {
            return new FixMatrix
            {
                M11 = matrix.m00,
                M12 = matrix.m01,
                M13 = matrix.m02,
                M21 = matrix.m10,
                M22 = matrix.m11,
                M23 = matrix.m12,
                M31 = matrix.m20,
                M32 = matrix.m21,
                M33 = matrix.m22,
            };
        }
        
        public static Matrix4x4 ToMatrix4X4(this FixMatrix fpMatrix)
        {
            return new Matrix4x4
            {
                m00 = (float)fpMatrix.M11,
                m01 = (float)fpMatrix.M12,
                m02 = (float)fpMatrix.M13,
                m10 = (float)fpMatrix.M21,
                m11 = (float)fpMatrix.M22,
                m12 = (float)fpMatrix.M23,
                m20 = (float)fpMatrix.M31,
                m21 = (float)fpMatrix.M32,
                m22 = (float)fpMatrix.M33,
                m33 = 1f,
            };
        }
        
        public static void CopyFromUnityKeyframe(this FixKeyframe fpKeyframe,UnityEngine.Keyframe target)
        {
            fpKeyframe.weightedMode = (FixWeightedMode)target.weightedMode;
            fpKeyframe.time = target.time;
            fpKeyframe.value = target.value;
            fpKeyframe.inSlope = target.inTangent;
            fpKeyframe.outSlope = target.outTangent;
            fpKeyframe.inWeight = target.inWeight;
            fpKeyframe.outWeight = target.outWeight;

            if (target.inTangent == UnityEngine.Mathf.Infinity)
                fpKeyframe.inSlope = Fix64.MaxValue;
            if (target.outTangent == UnityEngine.Mathf.Infinity)
                fpKeyframe.outSlope = Fix64.MaxValue;
        }

        public static void CopyToUnityKeyframe(this FixKeyframe fpKeyframe,ref UnityEngine.Keyframe target)
        {
            target.weightedMode = (UnityEngine.WeightedMode)fpKeyframe.weightedMode;
            target.time = fpKeyframe.time.AsFloat();
            target.value = fpKeyframe.value.AsFloat();
            target.inTangent = fpKeyframe.inSlope.AsFloat();
            target.outTangent = fpKeyframe.outSlope.AsFloat();
            target.inWeight = fpKeyframe.inWeight.AsFloat();
            target.outWeight = fpKeyframe.outWeight.AsFloat();

            if (fpKeyframe.inSlope == Fix64.MaxValue)
                target.inTangent = UnityEngine.Mathf.Infinity;
            if(fpKeyframe.outSlope == Fix64.MaxValue)
                target.outTangent = UnityEngine.Mathf.Infinity;
        }
        
        public static readonly UnityEngine.AnimationCurve defaultCurve = UnityEngine.AnimationCurve.Linear(0, 0, 1, 0);
        
        public static void CopyFromUnityCurve(this FixCurve fpCurve , UnityEngine.AnimationCurve target)
        {
            if(target.Equals(defaultCurve))
            {
                fpCurve.frames = null;
                return;
            }

            if(fpCurve.frames == null || (fpCurve.frames != null && fpCurve.frames.Length != target.length))
                fpCurve.frames = new FixKeyframe[target.length];
            
            for(int i=0; i<target.length;i++)
            {
                fpCurve.frames[i].CopyFromUnityKeyframe(target.keys[i]);
            }
        }

        public static void CopyToUnityCurve(this FixCurve fpCurve ,ref UnityEngine.AnimationCurve target)
        {
            if (fpCurve.frames == null || (fpCurve.frames != null && fpCurve.frames.Length == 0))
            {
                target.keys = defaultCurve.keys;
                return;
            }

            if (target == null || (fpCurve.frames != null && fpCurve.frames.Length != target.length))
                fpCurve.frames = new FixKeyframe[target.length];

            while(target.length > fpCurve.frames.Length)
            {
                target.RemoveKey(target.length - 1);
            }
            while(target.length < fpCurve.frames.Length)
            {
                target.AddKey(new UnityEngine.Keyframe());
            }
            for (int i = 0; i < target.length; i++)
            {
                fpCurve.frames[i].CopyToUnityKeyframe(ref target.keys[i]);
            }
        }
        
        public static FixPolyline ToFixPolyline(this FixPolyline fpPolyline,AnimationCurve curve)
        {
            if (curve == null)
                return null;
            FixPolyline pline = new FixPolyline();
            pline.keys = new FixKeypoint[curve.length];
            for (int i = 0; i < curve.length; i++)
            {
                pline.keys[i] = new FixKeypoint() { time = curve.keys[i].time, value = curve.keys[i].value };
            }
            return pline;
        }
#if UNITY_EDITOR
        public static AnimationCurve ToCurve(this FixPolyline fpPolyline,FixPolyline line)
        {
            if (line == null)
                return null;
            if (line.keys != null && line.keys.Length != 0)
            {
                var kfs = new Keyframe[line.keys.Length];
                for (int i = 0; i < line.keys.Length; i++)
                {
                    var fixKeypoint = line.keys[i];
                    kfs[i] = new Keyframe((float)fixKeypoint.time, (float)fixKeypoint.value);
                }
                var curve = new AnimationCurve(kfs);
                for (int i = 0; i < kfs.Length; i++)
                {
                    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Linear);
                    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Linear);
                }
                return curve;
            }
            return null;
        }
#endif
    }

}