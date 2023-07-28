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
    /// A vector structure.
    /// </summary>
    [Serializable]
    public struct FixVector
    {
        private static Fix64 ZeroEpsilonSq = FixMath.Epsilon;
        internal static FixVector InternalZero;
        internal static FixVector Arbitrary;
        
        public Fix64 x;
        
        public Fix64 y;
        
        public Fix64 z;
        
        #region Static readonly variables
        /// <summary>
        /// A vector with components (0,0,0);
        /// </summary>
        public static readonly FixVector zero;
        /// <summary>
        /// A vector with components (-1,0,0);
        /// </summary>
        public static readonly FixVector left;
        /// <summary>
        /// A vector with components (1,0,0);
        /// </summary>
        public static readonly FixVector right;
        /// <summary>
        /// A vector with components (0,1,0);
        /// </summary>
        public static readonly FixVector up;
        /// <summary>
        /// A vector with components (0,-1,0);
        /// </summary>
        public static readonly FixVector down;
        /// <summary>
        /// A vector with components (0,0,-1);
        /// </summary>
        public static readonly FixVector back;
        /// <summary>
        /// A vector with components (0,0,1);
        /// </summary>
        public static readonly FixVector forward;
        /// <summary>
        /// A vector with components (1,1,1);
        /// </summary>
        public static readonly FixVector one;
        /// <summary>
        /// A vector with components 
        /// (FP.MinValue,FP.MinValue,FP.MinValue);
        /// </summary>
        public static readonly FixVector MinValue;
        /// <summary>
        /// A vector with components 
        /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
        /// </summary>
        public static readonly FixVector MaxValue;
        #endregion

        #region Private static constructor
        static FixVector()
        {
            one = new FixVector(1, 1, 1);
            zero = new FixVector(0, 0, 0);
            left = new FixVector(-1, 0, 0);
            right = new FixVector(1, 0, 0);
            up = new FixVector(0, 1, 0);
            down = new FixVector(0, -1, 0);
            back = new FixVector(0, 0, -1);
            forward = new FixVector(0, 0, 1);
            MinValue = new FixVector(Fix64.MinValue);
            MaxValue = new FixVector(Fix64.MaxValue);
            Arbitrary = new FixVector(1, 1, 1);
            InternalZero = zero;
        }
        #endregion

        public static FixVector Abs(FixVector other) {
            return new FixVector(Fix64.Abs(other.x), Fix64.Abs(other.y), Fix64.Abs(other.z));
        }

        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        /// <returns>Returns the squared length of the vector.</returns>
        public Fix64 sqrMagnitude {
            get { 
                return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
            }
        }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        /// <returns>Returns the length of the vector.</returns>
        public Fix64 magnitude {
            get {
                Fix64 num = ((this.x * this.x) + (this.y * this.y)) + (this.z * this.z);
                return Fix64.Sqrt(num);
            }
        }

        public static FixVector ClampMagnitude(FixVector vector, Fix64 maxLength) {
            return Normalize(vector) * maxLength;
        }

        /// <summary>
        /// Gets a normalized version of the vector.
        /// </summary>
        /// <returns>Returns a normalized version of the vector.</returns>
        public FixVector normalized {
            get {
                FixVector result = new FixVector(this.x, this.y, this.z);
                result.Normalize();

                return result;
            }
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>

        public FixVector(int x,int y,int z)
		{
			this.x = (Fix64)x;
			this.y = (Fix64)y;
			this.z = (Fix64)z;
		}

		public FixVector(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public void Scale(FixVector other) {
            this.x = x * other.x;
            this.y = y * other.y;
            this.z = z * other.z;
        }

        /// <summary>
        /// Sets all vector component to specific values.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public void Set(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="xyz">All components of the vector are set to xyz</param>
        public FixVector(Fix64 xyz)
        {
            this.x = xyz;
            this.y = xyz;
            this.z = xyz;
        }

		public static FixVector Lerp(FixVector from, FixVector to, Fix64 percent) {
			return from + (to - from) * percent;
		}

        public static FixVector Slerp(FixVector start, FixVector end, Fix64 percent)
        {
            Fix64 dot = FixVector.Dot(start, end);
            FixMath.Clamp(dot, -1, 1);
            Fix64 theta = FixMath.Acos(dot) * percent;
            FixVector RelativeVec = end - start * dot;
            RelativeVec.Normalize();
            return ((start * FixMath.Cos(theta)) + (RelativeVec * FixMath.Sin(theta)));
        }

        public static FixVector MoveTowards(FixVector current, FixVector target, Fix64 maxDistanceDelta)
        {
            FixVector a = target - current;
            Fix64 magnitude = a.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }

        public static void TransformNormal(ref FixVector normal, ref FixMatrix matrix, out FixVector result)
        {
            result = new FixVector((normal.x * matrix.M11) + (normal.y * matrix.M21) + (normal.z * matrix.M31),
                                 (normal.x * matrix.M12) + (normal.y * matrix.M22) + (normal.z * matrix.M32),
                                 (normal.x * matrix.M13) + (normal.y * matrix.M23) + (normal.z * matrix.M33));
        }

        public static void TransformCoordinate(ref FixVector coordinate, ref FixMatrix transform, out FixVector result)
        {
            result = new FixVector
            {
                x = (coordinate.x * transform.M11) + (coordinate.y * transform.M21) + (coordinate.z * transform.M31),
                y = (coordinate.x * transform.M12) + (coordinate.y * transform.M22) + (coordinate.z * transform.M32),
                z = (coordinate.x * transform.M13) + (coordinate.y * transform.M23) + (coordinate.z * transform.M33)
            };
        }
        
        public static FixVector TransformCoordinate(FixVector coordinate, FixMatrix transform)
        {
            var result = new FixVector
            {
                x = (coordinate.x * transform.M11) + (coordinate.y * transform.M21) + (coordinate.z * transform.M31),
                y = (coordinate.x * transform.M12) + (coordinate.y * transform.M22) + (coordinate.z * transform.M32),
                z = (coordinate.x * transform.M13) + (coordinate.y * transform.M23) + (coordinate.z * transform.M33)
            };

            return result;
        }

        /// <summary>
        /// Builds a string from the JVector.
        /// </summary>
        /// <returns>A string containing all three components.</returns>
        #region public override string ToString()
        public override string ToString() {
            return string.Format("({0:f1}, {1:f1}, {2:f1})", x.AsFloat(), y.AsFloat(), z.AsFloat());
        }
        #endregion

        /// <summary>
        /// Tests if an object is equal to this vector.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>Returns true if they are euqal, otherwise false.</returns>
        #region public override bool Equals(object obj)
        public override bool Equals(object obj)
        {
            if (!(obj is FixVector)) return false;
            FixVector other = (FixVector)obj;

            return (((x == other.x) && (y == other.y)) && (z == other.z));
        }
        #endregion

        /// <summary>
        /// Multiplies each component of the vector by the same components of the provided vector.
        /// </summary>
        public static FixVector Scale(FixVector vecA, FixVector vecB) {
            FixVector result;
            result.x = vecA.x * vecB.x;
            result.y = vecA.y * vecB.y;
            result.z = vecA.z * vecB.z;

            return result;
        }

        /// <summary>
        /// Tests if two JVector are equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns true if both values are equal, otherwise false.</returns>
        #region public static bool operator ==(JVector value1, JVector value2)
        public static bool operator ==(FixVector value1, FixVector value2)
        {
            return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z));
        }
        #endregion

        /// <summary>
        /// Tests if two JVector are not equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns false if both values are equal, otherwise true.</returns>
        #region public static bool operator !=(JVector value1, JVector value2)
        public static bool operator !=(FixVector value1, FixVector value2)
        {
            if ((value1.x == value2.x) && (value1.y == value2.y))
            {
                return (value1.z != value2.z);
            }
            return true;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        #region public static JVector Min(JVector value1, JVector value2)

        public static FixVector Min(FixVector value1, FixVector value2)
        {
            FixVector result;
            FixVector.Min(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
        public static void Min(ref FixVector value1, ref FixVector value2, out FixVector result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        #region public static JVector Max(JVector value1, JVector value2)
        public static FixVector Max(FixVector value1, FixVector value2)
        {
            FixVector result;
            FixVector.Max(ref value1, ref value2, out result);
            return result;
        }
		
		public static Fix64 Distance(FixVector v1, FixVector v2) {
			return Fix64.Sqrt ((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
		}
        
        public static Fix64 DistanceSqr(FixVector v1, FixVector v2) {
            return (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z);
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
        public static void Max(ref FixVector value1, ref FixVector value2, out FixVector result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        /// Sets the length of the vector to zero.
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            x = Fix64.Zero;
            y = Fix64.Zero;
            z = Fix64.Zero;
        }
        #endregion

        /// <summary>
        /// Checks if the length of the vector is zero.
        /// </summary>
        /// <returns>Returns true if the vector is zero, otherwise false.</returns>
        #region public bool IsZero()
        public bool IsZero()
        {
            return (this.sqrMagnitude == Fix64.Zero);
        }

        /// <summary>
        /// Checks if the length of the vector is nearly zero.
        /// </summary>
        /// <returns>Returns true if the vector is nearly zero, otherwise false.</returns>
        public bool IsNearlyZero()
        {
            return (this.sqrMagnitude < ZeroEpsilonSq);
        }
        #endregion

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The transformed vector.</returns>
        #region public static JVector Transform(JVector position, JMatrix matrix)
        public static FixVector Transform(FixVector position, FixMatrix matrix)
        {
            FixVector result;
            FixVector.Transform(ref position, ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void Transform(ref FixVector position, ref FixMatrix matrix, out FixVector result)
        {
            Fix64 num0 = ((position.x * matrix.M11) + (position.y * matrix.M21)) + (position.z * matrix.M31);
            Fix64 num1 = ((position.x * matrix.M12) + (position.y * matrix.M22)) + (position.z * matrix.M32);
            Fix64 num2 = ((position.x * matrix.M13) + (position.y * matrix.M23)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Transforms a vector by the transposed of the given Matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void TransposedTransform(ref FixVector position, ref FixMatrix matrix, out FixVector result)
        {
            Fix64 num0 = ((position.x * matrix.M11) + (position.y * matrix.M12)) + (position.z * matrix.M13);
            Fix64 num1 = ((position.x * matrix.M21) + (position.y * matrix.M22)) + (position.z * matrix.M23);
            Fix64 num2 = ((position.x * matrix.M31) + (position.y * matrix.M32)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        #region public static FP Dot(JVector vector1, JVector vector2)
        public static Fix64 Dot(FixVector vector1, FixVector vector2)
        {
            return FixVector.Dot(ref vector1, ref vector2);
        }


        /// <summary>
        /// Calculates the dot product of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static Fix64 Dot(ref FixVector vector1, ref FixVector vector2)
        {
            return ((vector1.x * vector2.x) + (vector1.y * vector2.y)) + (vector1.z * vector2.z);
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static void Add(JVector value1, JVector value2)
        public static FixVector Add(FixVector value1, FixVector value2)
        {
            FixVector result;
            FixVector.Add(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// Adds to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The sum of both vectors.</param>
        public static void Add(ref FixVector value1, ref FixVector value2, out FixVector result)
        {
            Fix64 num0 = value1.x + value2.x;
            Fix64 num1 = value1.y + value2.y;
            Fix64 num2 = value1.z + value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static FixVector Divide(FixVector value1, Fix64 scaleFactor) {
            FixVector result;
            FixVector.Divide(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the scaled vector.</param>
        public static void Divide(ref FixVector value1, Fix64 scaleFactor, out FixVector result) {
            result.x = value1.x / scaleFactor;
            result.y = value1.y / scaleFactor;
            result.z = value1.z / scaleFactor;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        public static FixVector Subtract(FixVector value1, FixVector value2)
        {
            FixVector result;
            FixVector.Subtract(ref value1, ref value2, out result);
            return result;
        }

        /// <summary>
        /// Subtracts to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The difference of both vectors.</param>
        public static void Subtract(ref FixVector value1, ref FixVector value2, out FixVector result)
        {
            Fix64 num0 = value1.x - value2.x;
            Fix64 num1 = value1.y - value2.y;
            Fix64 num2 = value1.z - value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        #region public static JVector Cross(JVector vector1, JVector vector2)
        public static FixVector Cross(FixVector vector1, FixVector vector2)
        {
            FixVector result;
            FixVector.Cross(ref vector1, ref vector2, out result);
            return result;
        }

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of both vectors.</param>
        public static void Cross(ref FixVector vector1, ref FixVector vector2, out FixVector result)
        {
            Fix64 num3 = (vector1.y * vector2.z) - (vector1.z * vector2.y);
            Fix64 num2 = (vector1.z * vector2.x) - (vector1.x * vector2.z);
            Fix64 num = (vector1.x * vector2.y) - (vector1.y * vector2.x);
            result.x = num3;
            result.y = num2;
            result.z = num;
        }
        #endregion

        /// <summary>
        /// Gets the hashcode of the vector.
        /// </summary>
        /// <returns>Returns the hashcode of the vector.</returns>
        #region public override int GetHashCode()
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Inverses the direction of the vector.
        /// </summary>
        #region public static JVector Negate(JVector value)
        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <returns>The negated vector.</returns>
        public static FixVector Negate(FixVector value)
        {
            FixVector result;
            FixVector.Negate(ref value,out result);
            return result;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <param name="result">The negated vector.</param>
        public static void Negate(ref FixVector value, out FixVector result)
        {
            Fix64 num0 = -value.x;
            Fix64 num1 = -value.y;
            Fix64 num2 = -value.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        #region public static JVector Normalize(JVector value)
        public static FixVector Normalize(FixVector value)
        {
            FixVector result;
            FixVector.Normalize(ref value, out result);
            return result;
        }

        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void Normalize()
        {
            Fix64 num2 = ((this.x * this.x) + (this.y * this.y)) + (this.z * this.z);
            Fix64 num = Fix64.One / Fix64.Sqrt(num2);
            this.x *= num;
            this.y *= num;
            this.z *= num;
        }

        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="result">A normalized vector.</param>
        public static void Normalize(ref FixVector value, out FixVector result)
        {
            Fix64 num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z);
            Fix64 num = Fix64.One / Fix64.Sqrt(num2);
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }
        #endregion

        #region public static void Swap(ref JVector vector1, ref JVector vector2)

        /// <summary>
        /// Swaps the components of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector to swap with the second.</param>
        /// <param name="vector2">The second vector to swap with the first.</param>
        public static void Swap(ref FixVector vector1, ref FixVector vector2)
        {
            Fix64 temp;

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;
        }
        #endregion

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        #region public static JVector Multiply(JVector value1, FP scaleFactor)
        public static FixVector Multiply(FixVector value1, Fix64 scaleFactor)
        {
            FixVector result;
            FixVector.Multiply(ref value1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the multiplied vector.</param>
        public static void Multiply(ref FixVector value1, Fix64 scaleFactor, out FixVector result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }
        #endregion

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the cross product of both.</returns>
        #region public static JVector operator %(JVector value1, JVector value2)
        public static FixVector operator %(FixVector value1, FixVector value2)
        {
            FixVector result; FixVector.Cross(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the dot product of both.</returns>
        #region public static FP operator *(JVector value1, JVector value2)
        public static Fix64 operator *(FixVector value1, FixVector value2)
        {
            return FixVector.Dot(ref value1, ref value2);
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value1">The vector to scale.</param>
        /// <param name="value2">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(JVector value1, FP value2)
        public static FixVector operator *(FixVector value1, Fix64 value2)
        {
            FixVector result;
            FixVector.Multiply(ref value1, value2,out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Multiplies a vector by a scale factor.
        /// </summary>
        /// <param name="value2">The vector to scale.</param>
        /// <param name="value1">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        #region public static JVector operator *(FP value1, JVector value2)
        public static FixVector operator *(Fix64 value1, FixVector value2)
        {
            FixVector result;
            FixVector.Multiply(ref value2, value1, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector operator -(JVector value1, JVector value2)
        public static FixVector operator -(FixVector value1, FixVector value2)
        {
            FixVector result; FixVector.Subtract(ref value1, ref value2, out result);
            return result;
        }

        public static FixVector operator -(FixVector value)
        {
            value.x = -value.x;
            value.y = -value.y;
            value.z = -value.z;
            return value;
        }
        
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static JVector operator +(JVector value1, JVector value2)
        public static FixVector operator +(FixVector value1, FixVector value2)
        {
            FixVector result; FixVector.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Divides a vector by a factor.
        /// </summary>
        /// <param name="value1">The vector to divide.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the scaled vector.</returns>
        public static FixVector operator /(FixVector value1, Fix64 value2) {
            FixVector result;
            FixVector.Divide(ref value1, value2, out result);
            return result;
        }

        public static Fix64 Angle(FixVector a, FixVector b) {
            return Fix64.Acos(a.normalized * b.normalized) * Fix64.Rad2Deg;
        }

        public FixVector2 ToFixVector2() {
            return new FixVector2(this.x, this.z);
        }

        /// <summary>
        /// 投影
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="onNormal"></param>
        /// <returns></returns>
        public static FixVector Project(FixVector vector, FixVector onNormal)
        {
            var num1 = Dot(onNormal, onNormal);
            if (num1 < Fix64.Epsilon)
            {
                return zero;
            }
            
            var num2 = Dot(vector, onNormal);
            return new FixVector(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
        }
    }

}