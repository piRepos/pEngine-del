using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Common.Math
{
    /// <summary>
    /// Contains static math utils.
    /// </summary>
    static class MathHelpers
    {

		/// <summary>
		/// Interpolate the specified A, B values with a specified precision.
		/// </summary>
		/// <param name="A">A.</param>
		/// <param name="B">B.</param>
		/// <param name="Prec">Precision.</param>
		static public double Interpolate(double A, double B, double Prec)
		{
			return (A + ((B - A) * Prec));
		}

		/// <summary>
		/// Interpolate the specified A, B values with a specified precision.
		/// </summary>
		/// <param name="A">A.</param>
		/// <param name="B">B.</param>
		/// <param name="Prec">Precision.</param>
		static public float Interpolate(float A, float B, float Prec)
		{
			return (A + ((B - A) * Prec));
		}

        /// <summary>
        /// Gets the unsigned module of a number.
        /// </summary>
        /// <param name="X">Number.</param>
        /// <param name="M">Module.</param>
        /// <returns>The module.</returns>
        static public int Mod(int X, int M)
        {
            int R = X % M;
            return R < 0 ? R + M : R;
        }

		/// <summary>
		/// Returns the next power of two that is larger than the specified number.
		/// </summary>
		/// <param name="n">The specified number.</param>
		/// <returns>The next power of two.</returns>
		public static long NextPowerOfTwo(long n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException("n", "Must be positive.");
			}
			return (long)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
		}

		/// <summary>
		/// Returns the next power of two that is larger than the specified number.
		/// </summary>
		/// <param name="n">The specified number.</param>
		/// <returns>The next power of two.</returns>
		public static int NextPowerOfTwo(int n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException("n", "Must be positive.");
			}
			return (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
		}

		/// <summary>
		/// Returns the next power of two that is larger than the specified number.
		/// </summary>
		/// <param name="n">The specified number.</param>
		/// <returns>The next power of two.</returns>
		public static float NextPowerOfTwo(float n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException("n", "Must be positive.");
			}
			return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
		}

		/// <summary>
		/// Returns the next power of two that is larger than the specified number.
		/// </summary>
		/// <param name="n">The specified number.</param>
		/// <returns>The next power of two.</returns>
		public static double NextPowerOfTwo(double n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException("n", "Must be positive.");
			}
			return System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
		}

		/// <summary>Calculates the factorial of a given natural number.
		/// </summary>
		/// <param name="n">The number.</param>
		/// <returns>n!</returns>
		public static long Factorial(int n)
		{
			long result = 1;

			for (; n > 1; n--)
			{
				result *= n;
			}

			return result;
		}

		/// <summary>
		/// Calculates the binomial coefficient <paramref name="n"/> above <paramref name="k"/>.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="k">The k.</param>
		/// <returns>n! / (k! * (n - k)!)</returns>
		public static long BinomialCoefficient(int n, int k)
		{
			return Factorial(n) / (Factorial(k) * Factorial(n - k));
		}

		/// <summary>
		/// Returns an approximation of the inverse square root of left number.
		/// </summary>
		/// <param name="x">A number.</param>
		/// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
		/// <remarks>
		/// This is an improved implementation of the the method known as Carmack's inverse square root
		/// which is found in the Quake III source code. This implementation comes from
		/// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
		/// http://www.beyond3d.com/content/articles/8/
		/// </remarks>
		public static float InverseSqrtFast(float x)
		{
			unsafe
			{
				float xhalf = 0.5f * x;
				int i = *(int*)&x;            // Read bits as integer.
				i = 0x5f375a86 - (i >> 1);    // Make an initial guess for Newton-Raphson approximation
				x = *(float*)&i;                // Convert bits back to float
				x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
				return x;
			}
		}

		/// <summary>
		/// Returns an approximation of the inverse square root of left number.
		/// </summary>
		/// <param name="x">A number.</param>
		/// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
		/// <remarks>
		/// This is an improved implementation of the the method known as Carmack's inverse square root
		/// which is found in the Quake III source code. This implementation comes from
		/// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
		/// http://www.beyond3d.com/content/articles/8/. The 64 bit implementation uses a different magic
		/// demon number found at https://en.wikipedia.org/wiki/Fast_inverse_square_root#History_and_investigation
		/// </remarks>
		public static double InverseSqrtFast(double x)
		{
			unsafe
			{
				double xhalf = 0.5 * x;
				long i = *(long*)&x;              // Read bits as long integer.
				i = 0x5fe6eb50c7b537a9 - (i >> 1);    // what the fuck?
				x = *(double*)&i;               // Convert bits back to double
				x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
				return x;
			}
		}

		/// <summary>
		/// Calculate the maximum common divisor.
		/// </summary>
		/// <param name="numbers">Source numbers.</param>
		/// <returns>The maximum common divisor.</returns>
		public static int GCD(int[] numbers)
		{
			return numbers.Aggregate(GCD);
		}

		/// <summary>
		/// Calculate the maximum common divisor.
		/// </summary>
		/// <param name="a">First number.</param>
		/// <param name="b">Second number.</param>
		/// <returns>The maximum common divisor.</returns>
		public static int GCD(int a, int b)
		{
			return b == 0 ? a : GCD(b, a % b);
		}

}
}
