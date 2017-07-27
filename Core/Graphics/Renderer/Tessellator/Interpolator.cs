using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Physics.Geometry;
using pEngine.Common.Math;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public class Interpolator
    {
		/// <summary>
		/// Makes a new instance of <see cref="Interpolator"/>.
		/// </summary>
		public Interpolator(MeshDescriptor inputMesh, uint accuracy)
		{
			InputMesh = inputMesh;
			Accuracy = accuracy;

			interpolationCache = new Dictionary<Edge[], uint[]>();
		}

		/// <summary>
		/// Number of vertex per interpolation.
		/// </summary>
		public uint Accuracy { get; }

		/// <summary>
		/// 
		/// </summary>
		public MeshDescriptor InputMesh { get; }

		/// <summary>
		/// 
		/// </summary>
		public MeshDescriptor OutputMesh { get; private set; }


		private Dictionary<Edge[], uint[]> interpolationCache;

		public void ComputeBezier()
		{
			List<DrawablePoint> Points = new List<DrawablePoint>(InputMesh.Points);
			List<Edge> Edges = new List<Edge>();
			List<Face> Faces = new List<Face>();

			List<Edge> interpolationEdges = new List<Edge>();
			List<Edge> interpolatedEdges = new List<Edge>();

			List<uint> interpolationEdgeCache = new List<uint>();

			// TODO: Foreach face
			for (int f = 0; f < InputMesh.Faces.Length; ++f)
			{
				List<Edge> inputEdges = new List<Edge>(GetOrederdEdges(InputMesh.Faces[f].Edges));
				List<uint> edgeIndexes = new List<uint>();

				var prevPoint = InputMesh.Edges[0].Start;

				for (int e = 0; e < inputEdges.Count; ++e)
				{
					Edge toAdd; int indx;

					var edge = inputEdges[e];
					var start = InputMesh.Points[(int)edge.Start];
					var end = InputMesh.Points[(int)edge.End];

					if (e > 0 && !end.Interpolated)
					{
						// - Add non interpolated edge
						toAdd = new Edge(prevPoint, edge.Start);
						indx = Edges.FindIndex(x => x == toAdd);
						if (indx < 0)
						{
							edgeIndexes.Add((uint)Edges.Count);
							Edges.Add(toAdd);
						}
						else edgeIndexes.Add((uint)indx);
						prevPoint = edge.Start;
					}
					else if (!end.Interpolated)
					{
						// - Add first edge
						indx = Edges.FindIndex(x => x == edge);
						if (indx < 0)
						{
							edgeIndexes.Add((uint)Edges.Count);
							Edges.Add(edge);
						}
						else edgeIndexes.Add((uint)indx);
						prevPoint = edge.End;
					}

					if (end.Interpolated)
					{
						interpolationEdges.Clear();
						interpolatedEdges.Clear();
						interpolationEdgeCache.Clear();

						do interpolationEdges.Add(InputMesh.Edges[e++]);
						while (e < InputMesh.Edges.Length && InputMesh.Points[(int)InputMesh.Edges[e].Start].Interpolated);
						e--;

						// - Check if the interpolation is cached
						if (interpolationCache.ContainsKey(interpolationEdges.ToArray()))
						{
							edgeIndexes.AddRange(interpolationCache[interpolationEdges.ToArray()]);
							prevPoint = interpolationEdges.Last().End;

							continue;
						}

						List<DrawablePoint> interpolationPoints = new List<DrawablePoint>(GetOrederdPoints(interpolationEdges));

						// - Compute interpolated points
						foreach (DrawablePoint p in CalculateBezier(interpolationPoints, Accuracy))
						{
							// - Cache this edge
							interpolationEdgeCache.Add((uint)Edges.Count);

							// - Add edge
							edgeIndexes.Add((uint)Edges.Count);
							Edges.Add(new Edge(prevPoint, (uint)Points.Count));

							// - Update prev point and add the new point
							prevPoint = (uint)Points.Count;
							Points.Add(p);
						}

						// - Store this interpolation to the cache
						interpolationCache[interpolationEdges.ToArray()] = interpolationEdgeCache.ToArray();

						// - Insert last edge
						edgeIndexes.Add((uint)Edges.Count);
						Edges.Add(new Edge(prevPoint, interpolationEdges.Last().End));
						prevPoint = interpolationEdges.Last().End;

					}
				}

				Faces.Add(new Face(edgeIndexes));
			}

			OutputMesh = new MeshDescriptor
			{
				DescriptorID = InputMesh.DescriptorID,
				Name = InputMesh.Name,
				Points = Points.ToArray(),
				Edges = Edges.ToArray(),
				Faces = Faces.ToArray()
			};
		}

		private IEnumerable<Edge> GetOrederdEdges(IEnumerable<uint> edges)
		{
			foreach (uint e in edges)
			{
				yield return InputMesh.Edges[(int)e];
			}
		}

		private IEnumerable<DrawablePoint> GetOrederdPoints(List<Edge> edges)
		{
			yield return InputMesh.Points[(int)edges[0].Start];

			foreach (Edge e in edges)
			{
				yield return InputMesh.Points[(int)e.End];
			}
		}

		private IEnumerable<Edge> GetLinkedPoints(uint startPoint, uint endPoint)
		{
			for (uint i = startPoint; i < endPoint - 1; ++i)
				yield return new Edge(i, i + 1);
		}

		private IEnumerable<DrawablePoint> CalculateBezier(List<DrawablePoint> anchors, uint accuracy)
		{
			// If there are 2 vertex i can't make a Bezier
			if (anchors.Count <= 2)
				yield break;

			for (float i = 1F / accuracy; i < 1F; i += 1F / accuracy)
			{
				List<DrawablePoint> temp = new List<DrawablePoint>();
				for (int j = 1; j < anchors.Count; ++j)
					temp.Add(InterpolatePoints(anchors[j - 1], anchors[j], i));

				while (temp.Count > 1)
				{
					List<DrawablePoint> temp2 = new List<DrawablePoint>();

					for (int j = 1; j < temp.Count; ++j)
						temp2.Add(InterpolatePoints(temp[j - 1], temp[j], i));

					temp = temp2;
				}

				yield return temp[0];
			}
		}

		private DrawablePoint InterpolatePoints(DrawablePoint p1, DrawablePoint p2, float prec)
		{
			Vector2 pp0 = p1.Position;
			Vector2 pp1 = p2.Position;
			Vector2 t0 = p1.RasterPosition;
			Vector2 t1 = p2.RasterPosition;
			float fr = (p1.FillColor.Rf * prec) + (p2.FillColor.Rf * (1 - prec));
			float fg = (p1.FillColor.Gf * prec) + (p2.FillColor.Gf * (1 - prec));
			float fb = (p1.FillColor.Bf * prec) + (p2.FillColor.Bf * (1 - prec));
			float fa = (p1.FillColor.Af * prec) + (p2.FillColor.Af * (1 - prec));
			float sr = (p1.StrokeColor.Rf * prec) + (p2.StrokeColor.Rf * (1 - prec));
			float sg = (p1.StrokeColor.Gf * prec) + (p2.StrokeColor.Gf * (1 - prec));
			float sb = (p1.StrokeColor.Bf * prec) + (p2.StrokeColor.Bf * (1 - prec));
			float sa = (p1.StrokeColor.Af * prec) + (p2.StrokeColor.Af * (1 - prec));

			DrawablePoint np = new DrawablePoint
			{
				Position = new Vector2(InterpolateFloat(pp0.X, pp1.X, prec), InterpolateFloat(pp0.Y, pp1.Y, prec)),
				RasterPosition = new Vector2(InterpolateFloat(t0.X, t1.X, prec), InterpolateFloat(t0.Y, t1.Y, prec)),
				FillColor = new Color4(fr, fg, fb, fa),
				StrokeColor = new Color4(sr, sg, sb, sa),
				Interpolated = false,
				Thickness = (p1.Thickness * prec) + (p2.Thickness * (1 - prec))
			};

			return np;
		}

		private float InterpolateFloat(float A, float B, float Prec)
		{
			return (A + ((B - A) * Prec));
		}

	}
}
