/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

using System;

namespace LogicShared.TrueSync.Math
{

    /// <summary>
    /// A Quaternion representing an orientation.
    /// </summary>
    [Serializable]
    public struct FixQuaternion
    {

        /// <summary>The X component of the quaternion.</summary>
        public Fix64 x;
        /// <summary>The Y component of the quaternion.</summary>
        public Fix64 y;
        /// <summary>The Z component of the quaternion.</summary>
        public Fix64 z;
        /// <summary>The W component of the quaternion.</summary>
        public Fix64 w;

        public static readonly FixQuaternion identity;

        static FixQuaternion() {
            identity = new FixQuaternion(0, 0, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the JQuaternion structure.
        /// </summary>
        /// <param name="x">The X component of the quaternion.</param>
        /// <param name="y">The Y component of the quaternion.</param>
        /// <param name="z">The Z component of the quaternion.</param>
        /// <param name="w">The W component of the quaternion.</param>
        public FixQuaternion(Fix64 x, Fix64 y, Fix64 z, Fix64 w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Set(Fix64 new_x, Fix64 new_y, Fix64 new_z, Fix64 new_w) {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }

        public void SetFromToRotation(FixVector fromDirection, FixVector toDirection) {
            FixQuaternion targetRotation = FixQuaternion.FromToRotation(fromDirection, toDirection);
            this.Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
        }

        public FixVector eulerAngles {
            get {
                FixVector result = new FixVector();

                Fix64 ysqr = y * y;
                Fix64 t0 = -2.0f * (ysqr + z * z) + 1.0f;
                Fix64 t1 = +2.0f * (x * y - w * z);
                Fix64 t2 = -2.0f * (x * z + w * y);
                Fix64 t3 = +2.0f * (y * z - w * x);
                Fix64 t4 = -2.0f * (x * x + ysqr) + 1.0f;

                t2 = t2 > 1.0f ? 1.0f : t2;
                t2 = t2 < -1.0f ? -1.0f : t2;

                result.x = Fix64.Atan2(t3, t4) * Fix64.Rad2Deg;
                result.y = Fix64.Asin(t2) * Fix64.Rad2Deg;
                result.z = Fix64.Atan2(t1, t0) * Fix64.Rad2Deg;

                return result * -1;
            }
        }

        public static Fix64 Angle(FixQuaternion a, FixQuaternion b) {
            FixQuaternion aInv = FixQuaternion.Inverse(a);
            FixQuaternion f = b * aInv;

            Fix64 angle = Fix64.Acos(f.w) * 2 * Fix64.Rad2Deg;

            if (angle > 180) {
                angle = 360 - angle;
            }

            return angle;
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixQuaternion Add(FixQuaternion quaternion1, FixQuaternion quaternion2)
        {
            FixQuaternion result;
            FixQuaternion.Add(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        public static FixQuaternion LookRotation(FixVector forward) {
            return CreateFromMatrix(FixMatrix.LookAt(forward, FixVector.up));
        }

        public static FixQuaternion LookRotation(FixVector forward, FixVector upwards) {
            return CreateFromMatrix(FixMatrix.LookAt(forward, upwards));
        }

        public static FixQuaternion Slerp(FixQuaternion from, FixQuaternion to, Fix64 t) {
            t = FixMath.Clamp(t, 0, 1);

            Fix64 dot = Dot(from, to);

            if (dot < 0.0f) {
                to = Multiply(to, -1);
                dot = -dot;
            }

            Fix64 halfTheta = Fix64.Acos(dot);

            return Multiply(Multiply(from, Fix64.Sin((1 - t) * halfTheta)) + Multiply(to, Fix64.Sin(t * halfTheta)), 1 / Fix64.Sin(halfTheta));
        }

        public static FixQuaternion RotateTowards(FixQuaternion from, FixQuaternion to, Fix64 maxDegreesDelta) {
            Fix64 dot = Dot(from, to);

            if (dot < 0.0f) {
                to = Multiply(to, -1);
                dot = -dot;
            }

            Fix64 halfTheta = Fix64.Acos(dot);
            Fix64 theta = halfTheta * 2;

            maxDegreesDelta *= Fix64.Deg2Rad;

            if (maxDegreesDelta >= theta) {
                return to;
            }

            maxDegreesDelta /= theta;

            return Multiply(Multiply(from, Fix64.Sin((1 - maxDegreesDelta) * halfTheta)) + Multiply(to, Fix64.Sin(maxDegreesDelta * halfTheta)), 1 / Fix64.Sin(halfTheta));
        }

        public static FixQuaternion Euler(Fix64 x, Fix64 y, Fix64 z) {
            x *= Fix64.Deg2Rad;
            y *= Fix64.Deg2Rad;
            z *= Fix64.Deg2Rad;

            FixQuaternion rotation;
            FixQuaternion.CreateFromYawPitchRoll(y, x, z, out rotation);

            return rotation;
        }

        public static FixQuaternion Euler(FixVector eulerAngles) {
            return Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        }

        public static FixQuaternion AngleAxis(Fix64 angle, FixVector axis) {
            axis = axis * Fix64.Deg2Rad;
            axis.Normalize();

            Fix64 halfAngle = angle * Fix64.Deg2Rad * Fix64.Half;

            FixQuaternion rotation;
            Fix64 sin = Fix64.Sin(halfAngle);

            rotation.x = axis.x * sin;
            rotation.y = axis.y * sin;
            rotation.z = axis.z * sin;
            rotation.w = Fix64.Cos(halfAngle);

            return rotation;
        }

        public static void CreateFromYawPitchRoll(Fix64 yaw, Fix64 pitch, Fix64 roll, out FixQuaternion result)
        {
            Fix64 num9 = roll * Fix64.Half;
            Fix64 num6 = Fix64.Sin(num9);
            Fix64 num5 = Fix64.Cos(num9);
            Fix64 num8 = pitch * Fix64.Half;
            Fix64 num4 = Fix64.Sin(num8);
            Fix64 num3 = Fix64.Cos(num8);
            Fix64 num7 = yaw * Fix64.Half;
            Fix64 num2 = Fix64.Sin(num7);
            Fix64 num = Fix64.Cos(num7);
            result.x = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.w = ((num * num3) * num5) + ((num2 * num4) * num6);
        }

        /// <summary>
        /// Quaternions are added.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The sum of both quaternions.</param>
        public static void Add(ref FixQuaternion quaternion1, ref FixQuaternion quaternion2, out FixQuaternion result)
        {
            result.x = quaternion1.x + quaternion2.x;
            result.y = quaternion1.y + quaternion2.y;
            result.z = quaternion1.z + quaternion2.z;
            result.w = quaternion1.w + quaternion2.w;
        }
        #endregion

        public static FixQuaternion Conjugate(FixQuaternion value)
        {
            FixQuaternion quaternion;
            quaternion.x = -value.x;
            quaternion.y = -value.y;
            quaternion.z = -value.z;
            quaternion.w = value.w;
            return quaternion;
        }

        public static Fix64 Dot(FixQuaternion a, FixQuaternion b) {
            return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static FixQuaternion Inverse(FixQuaternion rotation) {
            Fix64 invNorm = Fix64.One / ((rotation.x * rotation.x) + (rotation.y * rotation.y) + (rotation.z * rotation.z) + (rotation.w * rotation.w));
            return FixQuaternion.Multiply(FixQuaternion.Conjugate(rotation), invNorm);
        }

        public static FixQuaternion FromToRotation(FixVector fromVector, FixVector toVector) {
            FixVector w = FixVector.Cross(fromVector, toVector);
            FixQuaternion q = new FixQuaternion(w.x, w.y, w.z, FixVector.Dot(fromVector, toVector));
            q.w += Fix64.Sqrt(fromVector.sqrMagnitude * toVector.sqrMagnitude);
            q.Normalize();

            return q;
        }

        public static FixQuaternion Lerp(FixQuaternion a, FixQuaternion b, Fix64 t) {
            t = FixMath.Clamp(t, Fix64.Zero, Fix64.One);

            return LerpUnclamped(a, b, t);
        }

        public static FixQuaternion LerpUnclamped(FixQuaternion a, FixQuaternion b, Fix64 t) {
            FixQuaternion result = FixQuaternion.Multiply(a, (1 - t)) + FixQuaternion.Multiply(b, t);
            result.Normalize();

            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixQuaternion Subtract(FixQuaternion quaternion1, FixQuaternion quaternion2)
        {
            FixQuaternion result;
            FixQuaternion.Subtract(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        /// <summary>
        /// Quaternions are subtracted.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The difference of both quaternions.</param>
        public static void Subtract(ref FixQuaternion quaternion1, ref FixQuaternion quaternion2, out FixQuaternion result)
        {
            result.x = quaternion1.x - quaternion2.x;
            result.y = quaternion1.y - quaternion2.y;
            result.z = quaternion1.z - quaternion2.z;
            result.w = quaternion1.w - quaternion2.w;
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)
        public static FixQuaternion Multiply(FixQuaternion quaternion1, FixQuaternion quaternion2)
        {
            FixQuaternion result;
            FixQuaternion.Multiply(ref quaternion1, ref quaternion2, out result);
            return result;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The product of both quaternions.</param>
        public static void Multiply(ref FixQuaternion quaternion1, ref FixQuaternion quaternion2, out FixQuaternion result)
        {
            Fix64 x = quaternion1.x;
            Fix64 y = quaternion1.y;
            Fix64 z = quaternion1.z;
            Fix64 w = quaternion1.w;
            Fix64 num4 = quaternion2.x;
            Fix64 num3 = quaternion2.y;
            Fix64 num2 = quaternion2.z;
            Fix64 num = quaternion2.w;
            Fix64 num12 = (y * num2) - (z * num3);
            Fix64 num11 = (z * num4) - (x * num2);
            Fix64 num10 = (x * num3) - (y * num4);
            Fix64 num9 = ((x * num4) + (y * num3)) + (z * num2);
            result.x = ((x * num) + (num4 * w)) + num12;
            result.y = ((y * num) + (num3 * w)) + num11;
            result.z = ((z * num) + (num2 * w)) + num10;
            result.w = (w * num) - num9;
        }
        #endregion

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <returns>The scaled quaternion.</returns>
        #region public static JQuaternion Multiply(JQuaternion quaternion1, Fix64 scaleFactor)
        public static FixQuaternion Multiply(FixQuaternion quaternion1, Fix64 scaleFactor)
        {
            FixQuaternion result;
            FixQuaternion.Multiply(ref quaternion1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Scale a quaternion
        /// </summary>
        /// <param name="quaternion1">The quaternion to scale.</param>
        /// <param name="scaleFactor">Scale factor.</param>
        /// <param name="result">The scaled quaternion.</param>
        public static void Multiply(ref FixQuaternion quaternion1, Fix64 scaleFactor, out FixQuaternion result)
        {
            result.x = quaternion1.x * scaleFactor;
            result.y = quaternion1.y * scaleFactor;
            result.z = quaternion1.z * scaleFactor;
            result.w = quaternion1.w * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Sets the length of the quaternion to one.
        /// </summary>
        #region public void Normalize()
        public void Normalize()
        {
            Fix64 num2 = (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z)) + (this.w * this.w);
            Fix64 num = 1 / (Fix64.Sqrt(num2));
            this.x *= num;
            this.y *= num;
            this.z *= num;
            this.w *= num;
        }
        #endregion

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <returns>JQuaternion representing an orientation.</returns>
        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)
        public static FixQuaternion CreateFromMatrix(FixMatrix matrix)
        {
            FixQuaternion result;
            FixQuaternion.CreateFromMatrix(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <param name="result">JQuaternion representing an orientation.</param>
        public static void CreateFromMatrix(ref FixMatrix matrix, out FixQuaternion result)
        {
            Fix64 num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > Fix64.Zero)
            {
                Fix64 num = Fix64.Sqrt((num8 + Fix64.One));
                result.w = num * Fix64.Half;
                num = Fix64.Half / num;
                result.x = (matrix.M23 - matrix.M32) * num;
                result.y = (matrix.M31 - matrix.M13) * num;
                result.z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                Fix64 num7 = Fix64.Sqrt((((Fix64.One + matrix.M11) - matrix.M22) - matrix.M33));
                Fix64 num4 = Fix64.Half / num7;
                result.x = Fix64.Half * num7;
                result.y = (matrix.M12 + matrix.M21) * num4;
                result.z = (matrix.M13 + matrix.M31) * num4;
                result.w = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                Fix64 num6 = Fix64.Sqrt((((Fix64.One + matrix.M22) - matrix.M11) - matrix.M33));
                Fix64 num3 = Fix64.Half / num6;
                result.x = (matrix.M21 + matrix.M12) * num3;
                result.y = Fix64.Half * num6;
                result.z = (matrix.M32 + matrix.M23) * num3;
                result.w = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                Fix64 num5 = Fix64.Sqrt((((Fix64.One + matrix.M33) - matrix.M11) - matrix.M22));
                Fix64 num2 = Fix64.Half / num5;
                result.x = (matrix.M31 + matrix.M13) * num2;
                result.y = (matrix.M32 + matrix.M23) * num2;
                result.z = Fix64.Half * num5;
                result.w = (matrix.M12 - matrix.M21) * num2;
            }
        }
        #endregion

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static Fix64 operator *(JQuaternion value1, JQuaternion value2)
        public static FixQuaternion operator *(FixQuaternion value1, FixQuaternion value2)
        {
            FixQuaternion result;
            FixQuaternion.Multiply(ref value1, ref value2,out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Add two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The sum of both quaternions.</returns>
        #region public static Fix64 operator +(JQuaternion value1, JQuaternion value2)
        public static FixQuaternion operator +(FixQuaternion value1, FixQuaternion value2)
        {
            FixQuaternion result;
            FixQuaternion.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtract two quaternions.
        /// </summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The difference of both quaternions.</returns>
        #region public static Fix64 operator -(JQuaternion value1, JQuaternion value2)
        public static FixQuaternion operator -(FixQuaternion value1, FixQuaternion value2)
        {
            FixQuaternion result;
            FixQuaternion.Subtract(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /**
         *  @brief Rotates a {@link TSVector} by the {@link TSQuanternion}.
         **/
        public static FixVector operator *(FixQuaternion quat, FixVector vec) {
            Fix64 num = quat.x * 2f;
            Fix64 num2 = quat.y * 2f;
            Fix64 num3 = quat.z * 2f;
            Fix64 num4 = quat.x * num;
            Fix64 num5 = quat.y * num2;
            Fix64 num6 = quat.z * num3;
            Fix64 num7 = quat.x * num2;
            Fix64 num8 = quat.x * num3;
            Fix64 num9 = quat.y * num3;
            Fix64 num10 = quat.w * num;
            Fix64 num11 = quat.w * num2;
            Fix64 num12 = quat.w * num3;

            FixVector result;
            result.x = (1f - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
            result.y = (num7 + num12) * vec.x + (1f - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
            result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (1f - (num4 + num5)) * vec.z;

            return result;
        }

        public override string ToString() {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }

    }
}
