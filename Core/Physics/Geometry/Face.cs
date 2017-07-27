using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Physics.Geometry
{
    public struct Face
    {
		/// <summary>
		/// Makes a new instance of <see cref="Face"/>.
		/// </summary>
		/// <param name="edgeCount">Edges count.</param>
		public Face(int edgeCount)
		{
			Edges = new uint[edgeCount];
		}

		/// <summary>
		/// Makes a new instance of <see cref="Face"/>.
		/// </summary>
		/// <param name="edges">Edges to add.</param>
		public Face(IEnumerable<uint> edges)
		{
			Edges = edges.ToArray();
		}

		/// <summary>
		/// Edge indexes for this face.
		/// </summary>
		public uint[] Edges;

		/// <summary>
		/// String rappresentation of this <see cref="Face"/>.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			string ret = Edges.First().ToString();
			for (int i = 1; i < Edges.Length; ++i)
				ret += " -> " + Edges[i];
			return ret;
		}

	}
}
