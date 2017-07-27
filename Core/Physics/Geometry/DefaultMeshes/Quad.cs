using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;
using pEngine.Common;

namespace pEngine.Core.Physics.Geometry
{
    public class Quad : Shape
    {
		/// <summary>
		/// Creates a new instance of <see cref="Quad"/>.
		/// </summary>
		/// <param name="p1">Top-left corner point.</param>
		/// <param name="p2">Top-right corner point.</param>
		/// <param name="p3">Bot-left corner point.</param>
		/// <param name="p4">Bot-right corner point.</param>
		public Quad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) 
			: base ("Quad")
		{
			Points.Add(new Point { Position = p1 });
			Points.Add(new Point { Position = p2 });
			Points.Add(new Point { Position = p3 });
			Points.Add(new Point { Position = p4 });

			Edges.Add(new Edge(0, 1));
			Edges.Add(new Edge(1, 2));
			Edges.Add(new Edge(2, 3));
			Edges.Add(new Edge(3, 0));

			Faces.Add(new Face(new uint[]{ 0, 1, 2, 3}));
		}

		/// <summary>
		/// Creates a new instance of <see cref="Quad"/>.
		/// </summary>
		public Quad()
			: this(new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1))
		{
		}

		#region Positions

		/// <summary>
		/// First quad point.
		/// </summary>
		public Vector2 P1
		{
			get { return Points[0].Position; }
			set { Point p = Points[0]; p.Position = value; Points[0] = p; }
		}

		/// <summary>
		/// Second quad point.
		/// </summary>
		public Vector2 P2
		{
			get { return Points[1].Position; }
			set { Point p = Points[1]; p.Position = value; Points[1] = p; }
		}

		/// <summary>
		/// Third quad point.
		/// </summary>
		public Vector2 P3
		{
			get { return Points[2].Position; }
			set { Point p = Points[2]; p.Position = value; Points[2] = p; }
		}

		/// <summary>
		/// Fourth quad point.
		/// </summary>
		public Vector2 P4
		{
			get { return Points[3].Position; }
			set { Point p = Points[3]; p.Position = value; Points[3] = p; }
		}

		#endregion

	
	}
}
