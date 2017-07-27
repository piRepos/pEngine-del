using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Physics.Geometry;

using pEngine.Core.Graphics.Renderer.Batches;

using pEngine.Common.Diagnostic;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public class Tessellator
    {
		/// <summary>
		/// Creates a new instances of <see cref="Tessellator"/>.
		/// </summary>
		public Tessellator()
		{

			Cache = new MeshCache();

			MeshCache.MeshLifetime = new TimeSpan(0, 0, 10);
		}

		#region Modules
		

		MeshCache Cache { get; }

		#endregion

		public void LoadMeshes(MeshDescriptor mesh)
		{
			PerformanceCollector coll = new PerformanceCollector("Interpolator");

			Interpolator interpolator = new Interpolator(mesh, 2);

			using (coll.StartCollect("Interpolation"))
				interpolator.ComputeBezier();

			MeshDescriptor interpolated = interpolator.OutputMesh;

			VertexBatch batch = Cache.GetBatch(interpolated);
			

			if (interpolated.Invalidation.HasFlag(MeshInvalidation.Vertexs))
			{
				foreach (DrawablePoint p in interpolated.Points)
				{
					GLVertex v = new GLVertex
					{
						X = p.Position.X,
						Y = p.Position.Y,
						Tx = p.RasterPosition.X,
						Ty = p.RasterPosition.Y,
						A = p.FillColor.Af,
						R = p.FillColor.Rf,
						G = p.FillColor.Gf,
						B = p.FillColor.Bf
					};
					
				}
			}

			if (interpolated.Invalidation.HasFlag(MeshInvalidation.Edges))
			{
				int ie = 0;
				uint end = 0;

				foreach (Edge e in interpolated.Edges)
				{
					batch.Indexes[ie++] = e.Start;
					end = e.End;
				}

				batch.Indexes[ie] = end;
			}
		}

    }
}
