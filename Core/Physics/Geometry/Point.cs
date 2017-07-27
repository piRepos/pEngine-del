using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;
using pEngine.Common;

namespace pEngine.Core.Physics.Geometry
{
    public struct Point : IPoint, IEquatable<Point>
    {
		/// <summary>
		/// Makes a new instance of <see cref="Point"/>.
		/// </summary>
		/// <param name="pos">Point position.</param>
		/// <param name="anchor">True if this point is a curve anchor.</param>
		public Point(Vector2 pos, bool anchor = false)
		{
			Position = pos;
			Interpolated = anchor;
		}

		/// <summary>
		/// Point position.
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// If true this is an anchor point for bezier curves.
		/// </summary>
		public bool Interpolated { get; set; }

		/// <summary>
		/// String rappresentation of this <see cref="Point"/>.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			string i = Interpolated ? "interpolated" : "no interpolation";
			return $@"({Position}) : {i}";
		}

		#region Equals

		public bool Equals(Point obj)
		{
			return this == obj;
		}

		public override bool Equals(Object obj)
		{
			return obj is Point && this == (Point)obj;
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode()
				^ Interpolated.GetHashCode();
		}

		public static bool operator ==(Point x, Point y)
		{
			return x.Position == y.Position
				&& x.Interpolated == y.Interpolated;
		}

		public static bool operator !=(Point x, Point y)
		{
			return !(x == y);
		}

		#endregion
	}
}
