﻿using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using pEngine.Common.Math;

namespace pEngine
{
	/// <summary>Represents a 2D vector using two single-precision floating-point numbers.</summary>
	/// <remarks>
	/// The Vector2 structure is suitable for interoperation with unmanaged code requiring two consecutive floats.
	/// </remarks>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 : IEquatable<Vector2>
	{
		/// <summary>
		/// The X component of the Vector2.
		/// </summary>
		public float X;

		/// <summary>
		/// The Y component of the Vector2.
		/// </summary>
		public float Y;

		/// <summary>
		/// Defines a unit-length Vector2 that points towards the X-axis.
		/// </summary>
		public static readonly Vector2 UnitX = new Vector2(1, 0);

		/// <summary>
		/// Defines a unit-length Vector2 that points towards the Y-axis.
		/// </summary>
		public static readonly Vector2 UnitY = new Vector2(0, 1);

		/// <summary>
		/// Defines a zero-length Vector2.
		/// </summary>
		public static readonly Vector2 Zero = new Vector2(0, 0);

		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector2 One = new Vector2(1, 1);

		/// <summary>
		/// Defines the size of the Vector2 struct in bytes.
		/// </summary>
		public const int SizeInBytes = sizeof(float) * 2;

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector2(float value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
		/// Constructs a new Vector2.
		/// </summary>
		/// <param name="x">The x coordinate of the net Vector2.</param>
		/// <param name="y">The y coordinate of the net Vector2.</param>
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Gets or sets the value at the index of the Vector.
		/// </summary>
		public float this[int index]
		{
			get
			{
				if (index == 0)
					return X;
				else if (index == 1)
					return Y;
				throw new IndexOutOfRangeException("You tried to access this vector at index: " + index);
			}
			set
			{
				if (index == 0)
					X = value;
				else if (index == 1)
					Y = value;
				throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		public float Width
		{
			get
			{
				return X;
			}
			set
			{
				X = value;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public float Height
		{
			get
			{
				return Y;
			}
			set
			{
				Y = value;
			}
		}

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public float Length
		{
			get { return (float)System.Math.Sqrt(X * X + Y * Y); }
		}

		/// <summary>
		/// Gets an approximation of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property uses an approximation of the square root function to calculate vector magnitude, with
		/// an upper error bound of 0.001.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthSquared"/>
		public float LengthFast
		{
			get { return 1.0f / MathHelpers.InverseSqrtFast(X * X + Y * Y); }
		}

		/// <summary>
		/// Gets the square of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property avoids the costly square root operation required by the Length property. This makes it more suitable
		/// for comparisons.
		/// </remarks>
		/// <see cref="Length"/>
		/// <seealso cref="LengthFast"/>
		public float LengthSquared
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		/// Gets the perpendicular vector on the right side of this vector.
		/// </summary>
		public Vector2 PerpendicularRight
		{
			get { return new Vector2(Y, -X); }
		}

		/// <summary>
		/// Gets the perpendicular vector on the left side of this vector.
		/// </summary>
		public Vector2 PerpendicularLeft
		{
			get { return new Vector2(-Y, X); }
		}

		/// <summary>
		/// Scales the Vector2 to unit length.
		/// </summary>
		public void Normalize()
		{
			float scale = 1.0f / this.Length;
			X *= scale;
			Y *= scale;
		}

		/// <summary>
		/// Scales the Vector2 to approximately unit length.
		/// </summary>
		public void NormalizeFast()
		{
			float scale = MathHelpers.InverseSqrtFast(X * X + Y * Y);
			X *= scale;
			Y *= scale;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <returns>Result of operation.</returns>
		public static Vector2 Add(Vector2 a, Vector2 b)
		{
			Add(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add(ref Vector2 a, ref Vector2 b, out Vector2 result)
		{
			result = new Vector2(a.X + b.X, a.Y + b.Y);
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>Result of subtraction</returns>
		public static Vector2 Subtract(Vector2 a, Vector2 b)
		{
			Subtract(ref a, ref b, out a);
			return a;
		}

		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract(ref Vector2 a, ref Vector2 b, out Vector2 result)
		{
			result = new Vector2(a.X - b.X, a.Y - b.Y);
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2 Multiply(Vector2 vector, float scale)
		{
			Multiply(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2 vector, float scale, out Vector2 result)
		{
			result = new Vector2(vector.X * scale, vector.Y * scale);
		}

		/// <summary>
		/// Multiplies a vector by the components a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2 Multiply(Vector2 vector, Vector2 scale)
		{
			Multiply(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2 vector, ref Vector2 scale, out Vector2 result)
		{
			result = new Vector2(vector.X * scale.X, vector.Y * scale.Y);
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2 Divide(Vector2 vector, float scale)
		{
			Divide(ref vector, scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2 vector, float scale, out Vector2 result)
		{
			Multiply(ref vector, 1 / scale, out result);
		}

		/// <summary>
		/// Divides a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of the operation.</returns>
		public static Vector2 Divide(Vector2 vector, Vector2 scale)
		{
			Divide(ref vector, ref scale, out vector);
			return vector;
		}

		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2 vector, ref Vector2 scale, out Vector2 result)
		{
			result = new Vector2(vector.X / scale.X, vector.Y / scale.Y);
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector2 ComponentMin(Vector2 a, Vector2 b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void ComponentMin(ref Vector2 a, ref Vector2 b, out Vector2 result)
		{
			result = new Vector2();
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector2 ComponentMax(Vector2 a, Vector2 b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			return a;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void ComponentMax(ref Vector2 a, ref Vector2 b, out Vector2 result)
		{
			result = new Vector2();
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Returns the Vector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3</returns>
		public static Vector2 Min(Vector2 left, Vector2 right)
		{
			return left.LengthSquared < right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Returns the Vector3 with the minimum magnitude
		/// </summary>
		/// <param name="left">Left operand</param>
		/// <param name="right">Right operand</param>
		/// <returns>The minimum Vector3</returns>
		public static Vector2 Max(Vector2 left, Vector2 right)
		{
			return left.LengthSquared >= right.LengthSquared ? left : right;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <returns>The clamped vector</returns>
		public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
		{
			vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
			return vec;
		}

		/// <summary>
		/// Clamp a vector to the given minimum and maximum vectors
		/// </summary>
		/// <param name="vec">Input vector</param>
		/// <param name="min">Minimum vector</param>
		/// <param name="max">Maximum vector</param>
		/// <param name="result">The clamped vector</param>
		public static void Clamp(ref Vector2 vec, ref Vector2 min, ref Vector2 max, out Vector2 result)
		{
			result = new Vector2();
			result.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
			result.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
		}

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static Vector2 Normalize(Vector2 vec)
		{
			float scale = 1.0f / vec.Length;
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void Normalize(ref Vector2 vec, out Vector2 result)
		{
			result = new Vector2();
			float scale = 1.0f / vec.Length;
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
		}

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <returns>The normalized vector</returns>
		public static Vector2 NormalizeFast(Vector2 vec)
		{
			float scale = MathHelpers.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y);
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Scale a vector to approximately unit length
		/// </summary>
		/// <param name="vec">The input vector</param>
		/// <param name="result">The normalized vector</param>
		public static void NormalizeFast(ref Vector2 vec, out Vector2 result)
		{
			result = new Vector2();
			float scale = MathHelpers.InverseSqrtFast(vec.X * vec.X + vec.Y * vec.Y);
			result.X = vec.X * scale;
			result.Y = vec.Y * scale;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static float Dot(Vector2 left, Vector2 right)
		{
			return left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot(ref Vector2 left, ref Vector2 right, out float result)
		{
			result = left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		/// Calculate the perpendicular dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The perpendicular dot product of the two inputs</returns>
		public static float PerpDot(Vector2 left, Vector2 right)
		{
			return left.X * right.Y - left.Y * right.X;
		}

		/// <summary>
		/// Calculate the perpendicular dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The perpendicular dot product of the two inputs</param>
		public static void PerpDot(ref Vector2 left, ref Vector2 right, out float result)
		{
			result = left.X * right.Y - left.Y * right.X;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
		public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
		{
			a.X = blend * (b.X - a.X) + a.X;
			a.Y = blend * (b.Y - a.Y) + a.Y;
			return a;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
		public static void Lerp(ref Vector2 a, ref Vector2 b, float blend, out Vector2 result)
		{
			result = new Vector2();
			result.X = blend * (b.X - a.X) + a.X;
			result.Y = blend * (b.Y - a.Y) + a.Y;
		}

		/// <summary>
		/// Interpolate 3 Vectors using Barycentric coordinates
		/// </summary>
		/// <param name="a">First input Vector</param>
		/// <param name="b">Second input Vector</param>
		/// <param name="c">Third input Vector</param>
		/// <param name="u">First Barycentric Coordinate</param>
		/// <param name="v">Second Barycentric Coordinate</param>
		/// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
		public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
		{
			return a + u * (b - a) + v * (c - a);
		}

		/// <summary>Interpolate 3 Vectors using Barycentric coordinates</summary>
		/// <param name="a">First input Vector.</param>
		/// <param name="b">Second input Vector.</param>
		/// <param name="c">Third input Vector.</param>
		/// <param name="u">First Barycentric Coordinate.</param>
		/// <param name="v">Second Barycentric Coordinate.</param>
		/// <param name="result">Output Vector. a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</param>
		public static void BaryCentric(ref Vector2 a, ref Vector2 b, ref Vector2 c, float u, float v, out Vector2 result)
		{
			result = a; // copy

			Vector2 temp = b; // copy
			Subtract(ref temp, ref a, out temp);
			Multiply(ref temp, u, out temp);
			Add(ref result, ref temp, out result);

			temp = c; // copy
			Subtract(ref temp, ref a, out temp);
			Multiply(ref temp, v, out temp);
			Add(ref result, ref temp, out result);
		}

		/// <summary>
		/// Gets or sets an Vector2 with the Y and X components of this instance.
		/// </summary>
		[XmlIgnore]
		public Vector2 Yx
		{
			get { return new Vector2(Y, X); }
			set
			{
				Y = value.X;
				X = value.Y;
			}
		}

		/// <summary>
		/// Adds the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of addition.</returns>
		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			left.X += right.X;
			left.Y += right.Y;
			return left;
		}

		/// <summary>
		/// Subtracts the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of subtraction.</returns>
		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			left.X -= right.X;
			left.Y -= right.Y;
			return left;
		}

		/// <summary>
		/// Negates the specified instance.
		/// </summary>
		/// <param name="vec">Operand.</param>
		/// <returns>Result of negation.</returns>
		public static Vector2 operator -(Vector2 vec)
		{
			vec.X = -vec.X;
			vec.Y = -vec.Y;
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by an another vector.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(Vector2 vec1, Vector2 vec2)
		{
			vec1.X *= vec2.X;
			vec1.Y *= vec2.Y;
			return vec1;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(Vector2 vec, int scale)
		{
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(int scale, Vector2 vec)
		{
			vec.X *= scale;
			vec.Y *= scale;
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(Vector2 vec, float scale)
		{
			vec.X = (int)(vec.X * scale);
			vec.Y = (int)(vec.Y * scale);
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(float scale, Vector2 vec)
		{
			vec.X = (int)(vec.X * scale);
			vec.Y = (int)(vec.Y * scale);
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(Vector2 vec, double scale)
		{
			vec.X = (int)(vec.X * scale);
			vec.Y = (int)(vec.Y * scale);
			return vec;
		}

		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator *(double scale, Vector2 vec)
		{
			vec.X = (int)(vec.X * scale);
			vec.Y = (int)(vec.Y * scale);
			return vec;
		}

		/// <summary>
		/// Divides the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand</param>
		/// <param name="scale">Right operand</param>
		/// <returns>Result of the division.</returns>
		public static Vector2 operator /(Vector2 vec, float scale)
		{
			float mult = 1.0f / scale;
			vec.X *= mult;
			vec.Y *= mult;
			return vec;
		}

		/// <summary>
		/// Divide the specified instance by a vector.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2 operator /(Vector2 vec1, Vector2 vec2)
		{
			return Divide(vec1, vec2);
		}

		/// <summary>
		/// Compares the specified instances for equality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are equal; false otherwise.</returns>
		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares the specified instances for inequality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are not equal; false otherwise.</returns>
		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !left.Equals(right);
		}

		internal static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		/// <summary>
		/// Returns a System.String that represents the current Vector2.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0}{2} {1}", X, Y, listSeparator);
		}

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Vector2))
				return false;

			return this.Equals((Vector2)obj);
		}

		/// <summary>Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector2 other)
		{
			return
				X == other.X &&
				Y == other.Y;
		}
	}
}