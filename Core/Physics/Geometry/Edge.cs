using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Physics.Geometry
{
    public struct Edge : IEquatable<Edge>
    {
		/// <summary>
		/// Makes a new instance of <see cref="Edge"/>.
		/// </summary>
		/// <param name="start">Start vertex.</param>
		/// <param name="end">End vertex.</param>
		public Edge(uint start, uint end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Start point.
		/// </summary>
		public uint Start;

		/// <summary>
		/// End point.
		/// </summary>
		public uint End;

		/// <summary>
		/// String rappresentation of this <see cref="Edge"/>.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return $@"{Start} -> {End}";
		}

		#region Equals

		public bool Equals(Edge obj)
		{
			return this == obj;
		}

		public override bool Equals(Object obj)
		{
			return obj is Edge && this == (Edge)obj;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode()
				^ End.GetHashCode();
		}

		public static bool operator ==(Edge x, Edge y)
		{
			return x.Start == y.Start
				&& x.End == y.End;
		}

		public static bool operator !=(Edge x, Edge y)
		{
			return !(x == y);
		}

		#endregion
	}
}
