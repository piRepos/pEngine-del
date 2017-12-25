using System;

namespace pEngine.Utils.Math
{
	using Math = System.Math;

	/// <summary>
	/// 2D Angle which can be reappresented in
	/// radians or in degrees
	/// </summary>
	public struct Angle
	{
		/// <summary>
		/// Angle in radians
		/// </summary>
		public double Radians
		{ get { return radians % (2 * Math.PI); } set { radians = value; } }

		/// <summary>
		/// Angle in degrees
		/// </summary>
		public double Degrees
		{
			get { return radians * (180.0 / Math.PI); }
			set { radians = (value % 360) * (Math.PI / 180.0); }
		}

		// -------------------------------------

		private double radians;

		/// <summary>
		/// Make a new angle with <c>degrees</c> initialization
		/// </summary>
		/// <param name="Degrees">Angle in degrees</param>
		public Angle(double Degrees)
		{
			radians = 0;
			this.Degrees = Degrees;
		}

		/// <summary>
		/// Check if an other object is the same angle
		/// </summary>
		/// <param name="obj">Object to check</param>
		/// <returns>True if are equals</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Angle)) return false;

			Angle Obj = (Angle)(obj);

			return radians == Obj.Radians;
		}

		/// <summary>
		/// Returns an hash code
		/// </summary>
		/// <returns>Hash</returns>
		public override int GetHashCode()
		{
			return (int)radians ^ (int)radians * 1000;
		}


		// OPERATORS -----------------------------------

		#region Sum
		public static Angle operator +(Angle A1, Angle A2)
		{
			return new Angle(A1.Degrees + A2.Degrees);
		}
		#endregion

		#region Diff
		public static Angle operator -(Angle A1, Angle A2)
		{
			return new Angle(A1.Degrees - A2.Degrees);
		}
		#endregion

		#region Mul
		public static Angle operator *(Angle A1, double A2)
		{
			return new Angle(A1.Degrees * A2);
		}
		#endregion

		#region Compare
		public static bool operator ==(Angle A1, Angle A2)
		{
			return A1.Degrees == A2.Degrees;
		}

		public static bool operator !=(Angle A1, Angle A2)
		{
			return A1.Degrees != A2.Degrees;
		}

		public static bool operator <(Angle A1, Angle A2)
		{
			return A1.Degrees < A2.Degrees;
		}

		public static bool operator >(Angle A1, Angle A2)
		{
			return A1.Degrees > A2.Degrees;
		}

		public static bool operator <=(Angle A1, Angle A2)
		{
			return A1.Degrees <= A2.Degrees;
		}

		public static bool operator >=(Angle A1, Angle A2)
		{
			return A1.Degrees >= A2.Degrees;
		}
		#endregion
	}
}
