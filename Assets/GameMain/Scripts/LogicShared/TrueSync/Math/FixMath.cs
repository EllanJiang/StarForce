using System;
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

namespace LogicShared.TrueSync.Math {

    /// <summary>
    /// Contains common math operations.
    /// </summary>
    public sealed class FixMath {

        /// <summary>
        /// PI constant.
        /// </summary>
        public static Fix64 Pi = Fix64.Pi;

        /**
        *  @brief PI over 2 constant.
        **/
        public static Fix64 PiOver2 = Fix64.PiOver2;

        /// <summary>
        /// A small value often used to decide if numeric 
        /// results are zero.
        /// </summary>
		public static Fix64 Epsilon = Fix64.Epsilon;

        /**
        *  @brief Degree to radians constant.
        **/
        public static Fix64 Deg2Rad = Fix64.Deg2Rad;

        /**
        *  @brief Radians to degree constant.
        **/
        public static Fix64 Rad2Deg = Fix64.Rad2Deg;


        /**
         * @brief Fix64 infinity.
         * */
        public static Fix64 Infinity = Fix64.MaxValue;

        /// <summary>
        /// Gets the square root.
        /// </summary>
        /// <param name="number">The number to get the square root from.</param>
        /// <returns></returns>
        #region public static Fix64 Sqrt(Fix64 number)
        public static Fix64 Sqrt(Fix64 number) {
            return Fix64.Sqrt(number);
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static Fix64 Max(Fix64 val1, Fix64 val2)
        public static Fix64 Max(Fix64 val1, Fix64 val2) {
            return (val1 > val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the minimum number of two values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <returns>Returns the smallest value.</returns>
        #region public static Fix64 Min(Fix64 val1, Fix64 val2)
        public static Fix64 Min(Fix64 val1, Fix64 val2) {
            return (val1 < val2) ? val1 : val2;
        }
        #endregion

        /// <summary>
        /// Gets the maximum number of three values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>Returns the largest value.</returns>
        #region public static Fix64 Max(Fix64 val1, Fix64 val2,Fix64 val3)
        public static Fix64 Max(Fix64 val1, Fix64 val2, Fix64 val3) {
            Fix64 max12 = (val1 > val2) ? val1 : val2;
            return (max12 > val3) ? max12 : val3;
        }
        #endregion

        /// <summary>
        /// Returns a number which is within [min,max]
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        #region public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
        public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max) {
            if (value < min)
            {
                value = min;
                return value;
            }
            if (value > max)
            {
                value = max;
            }
            return value;
        }
        #endregion

        /// <summary>
        /// Returns a number which is within [Fix64.Zero, Fix64.One]
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        public static Fix64 Clamp01(Fix64 value)
        {
            if (value < Fix64.Zero)
                return Fix64.Zero;

            if (value > Fix64.One)
                return Fix64.One;

            return value;
        }

        /// <summary>
        /// Changes every sign of the matrix entry to '+'
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="result">The absolute matrix.</param>
        #region public static void Absolute(ref JMatrix matrix,out JMatrix result)
        public static void Absolute(ref FixMatrix matrix, out FixMatrix result) {
            result.M11 = Fix64.Abs(matrix.M11);
            result.M12 = Fix64.Abs(matrix.M12);
            result.M13 = Fix64.Abs(matrix.M13);
            result.M21 = Fix64.Abs(matrix.M21);
            result.M22 = Fix64.Abs(matrix.M22);
            result.M23 = Fix64.Abs(matrix.M23);
            result.M31 = Fix64.Abs(matrix.M31);
            result.M32 = Fix64.Abs(matrix.M32);
            result.M33 = Fix64.Abs(matrix.M33);
        }
        #endregion

        /// <summary>
        /// Returns the sine of value.
        /// </summary>
        public static Fix64 Sin(Fix64 value) {
            return Fix64.Sin(value);
        }

        /// <summary>
        /// Returns the cosine of value.
        /// </summary>
        public static Fix64 Cos(Fix64 value) {
            return Fix64.Cos(value);
        }

        /// <summary>
        /// Returns the tan of value.
        /// </summary>
        public static Fix64 Tan(Fix64 value) {
            return Fix64.Tan(value);
        }

        /// <summary>
        /// Returns the arc sine of value.
        /// </summary>
        public static Fix64 Asin(Fix64 value) {
            return Fix64.Asin(value);
        }

        /// <summary>
        /// Returns the arc cosine of value.
        /// </summary>
        public static Fix64 Acos(Fix64 value) {
            return Fix64.Acos(value);
        }

        /// <summary>
        /// Returns the arc tan of value.
        /// </summary>
        public static Fix64 Atan(Fix64 value) {
            return Fix64.Atan(value);
        }

        /// <summary>
        /// Returns the arc tan of coordinates x-y.
        /// </summary>
        public static Fix64 Atan2(Fix64 y, Fix64 x) {
            return Fix64.Atan2(y, x);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fix64 Floor(Fix64 value) {
            return Fix64.Floor(value);
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static Fix64 Ceiling(Fix64 value) {
            return value;
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static Fix64 Round(Fix64 value) {
            return Fix64.Round(value);
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(Fix64 value) {
            return Fix64.Sign(value);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static Fix64 Abs(Fix64 value) {
            return Fix64.Abs(value);                
        }

        public static Fix64 Barycentric(Fix64 value1, Fix64 value2, Fix64 value3, Fix64 amount1, Fix64 amount2) {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        public static Fix64 CatmullRom(Fix64 value1, Fix64 value2, Fix64 value3, Fix64 value4, Fix64 amount) {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using FPs not to lose precission
            Fix64 amountSquared = amount * amount;
            Fix64 amountCubed = amountSquared * amount;
            return (Fix64)(0.5 * (2.0 * value2 +
                                 (value3 - value1) * amount +
                                 (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                 (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        public static Fix64 Distance(Fix64 value1, Fix64 value2) {
            return Fix64.Abs(value1 - value2);
        }

        public static Fix64 Hermite(Fix64 value1, Fix64 tangent1, Fix64 value2, Fix64 tangent2, Fix64 amount) {
            // All transformed to Fix64 not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            Fix64 v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            Fix64 sCubed = s * s * s;
            Fix64 sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                         (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                         t1 * s +
                         v1;
            return (Fix64)result;
        }

        public static Fix64 Lerp(Fix64 value1, Fix64 value2, Fix64 amount) {
            return value1 + (value2 - value1) * Clamp01(amount);
        }

        public static Fix64 InverseLerp(Fix64 value1, Fix64 value2, Fix64 amount) {
            if (value1 != value2)
                return Clamp01((amount - value1) / (value2 - value1));
            return Fix64.Zero;
        }

        public static Fix64 SmoothStep(Fix64 value1, Fix64 value2, Fix64 amount) {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            Fix64 result = Clamp(amount, 0f, 1f);
            result = Hermite(value1, 0f, value2, 0f, result);
            return result;
        }


        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// </summary>
        internal static Fix64 Pow2(Fix64 x)
        {
            if (x.RawValue == 0)
            {
                return Fix64.One;
            }

            // Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
            bool neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == Fix64.One)
            {
                return neg ? Fix64.One / (Fix64)2 : (Fix64)2;
            }
            if (x >= Fix64.Log2Max)
            {
                return neg ? Fix64.One / Fix64.MaxValue : Fix64.MaxValue;
            }
            if (x <= Fix64.Log2Min)
            {
                return neg ? Fix64.MaxValue : Fix64.Zero;
            }

            /* The algorithm is based on the power series for exp(x):
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             * 
             * From term n, we get term n+1 by multiplying with x/n.
             * When the sum term drops to zero, we can stop summing.
             */

            int integerPart = (int)Floor(x);
            // Take fractional part of exponent
            x = Fix64.FromRaw(x.RawValue & 0x00000000FFFFFFFF);

            var result = Fix64.One;
            var term = Fix64.One;
            int i = 1;
            while (term.RawValue != 0)
            {
                term = Fix64.FastMul(Fix64.FastMul(x, term), Fix64.Ln2) / (Fix64)i;
                result += term;
                i++;
            }

            result = Fix64.FromRaw(result.RawValue << integerPart);
            if (neg)
            {
                result = Fix64.One / result;
            }

            return result;
        }

        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        internal static Fix64 Log2(Fix64 x)
        {
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
            }

            // This implementation is based on Clay. S. Turner's fast binary logarithm
            // algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
            //     Processing Mag., pp. 124,140, Sep. 2010.)

            long b = 1U << (Fix64.FRACTIONAL_PLACES - 1);
            long y = 0;

            long rawX = x.RawValue;
            while (rawX < Fix64.ONE)
            {
                rawX <<= 1;
                y -= Fix64.ONE;
            }

            while (rawX >= (Fix64.ONE << 1))
            {
                rawX >>= 1;
                y += Fix64.ONE;
            }

            var z = Fix64.FromRaw(rawX);

            for (int i = 0; i < Fix64.FRACTIONAL_PLACES; i++)
            {
                z = Fix64.FastMul(z, z);
                if (z.RawValue >= (Fix64.ONE << 1))
                {
                    z = Fix64.FromRaw(z.RawValue >> 1);
                    y += b;
                }
                b >>= 1;
            }

            return Fix64.FromRaw(y);
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Ln(Fix64 x)
        {
            return Fix64.FastMul(Log2(x), Fix64.Ln2);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero, with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static Fix64 Pow(Fix64 b, Fix64 exp)
        {
            if (b == Fix64.One)
            {
                return Fix64.One;
            }

            if (exp.RawValue == 0)
            {
                return Fix64.One;
            }

            if (b.RawValue == 0)
            {
                if (exp.RawValue < 0)
                {
                    //throw new DivideByZeroException();
                    return Fix64.MaxValue;
                }
                return Fix64.Zero;
            }

            Fix64 log2 = Log2(b);
            return Pow2(exp * log2);
        }

        public static Fix64 MoveTowards(Fix64 current, Fix64 target, Fix64 maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
                return target;
            return (current + (Sign(target - current)) * maxDelta);
        }

        public static Fix64 Repeat(Fix64 t, Fix64 length)
        {
            return (t - (Floor(t / length) * length));
        }

        public static Fix64 DeltaAngle(Fix64 current, Fix64 target)
        {
            Fix64 num = Repeat(target - current, (Fix64)360f);
            if (num > (Fix64)180f)
            {
                num -= (Fix64)360f;
            }
            return num;
        }

        public static Fix64 MoveTowardsAngle(Fix64 current, Fix64 target, float maxDelta)
        {
            target = current + DeltaAngle(current, target);
            return MoveTowards(current, target, maxDelta);
        }

        public static Fix64 SmoothDamp(Fix64 current, Fix64 target, ref Fix64 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed)
        {
            Fix64 deltaTime = Fix64.EN2;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static Fix64 SmoothDamp(Fix64 current, Fix64 target, ref Fix64 currentVelocity, Fix64 smoothTime)
        {
            Fix64 deltaTime = Fix64.EN2;
            Fix64 positiveInfinity = -Fix64.MaxValue;
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
        }

        public static Fix64 SmoothDamp(Fix64 current, Fix64 target, ref Fix64 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed, Fix64 deltaTime)
        {
            smoothTime = Max(Fix64.EN4, smoothTime);
            Fix64 num = (Fix64)2f / smoothTime;
            Fix64 num2 = num * deltaTime;
            Fix64 num3 = Fix64.One / (((Fix64.One + num2) + (((Fix64)0.48f * num2) * num2)) + ((((Fix64)0.235f * num2) * num2) * num2));
            Fix64 num4 = current - target;
            Fix64 num5 = target;
            Fix64 max = maxSpeed * smoothTime;
            num4 = Clamp(num4, -max, max);
            target = current - num4;
            Fix64 num7 = (currentVelocity + (num * num4)) * deltaTime;
            currentVelocity = (currentVelocity - (num * num7)) * num3;
            Fix64 num8 = target + ((num4 + num7) * num3);
            if (((num5 - current) > Fix64.Zero) == (num8 > num5))
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }
    }
}
