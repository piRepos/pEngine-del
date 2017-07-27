using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Timing.Base;

using pEngine.Core.Graphics.Renderer.Batches;

namespace pEngine.Core.Graphics.Renderer.Tessellator
{
    public class MeshCache
    {
		/// <summary>
		/// Create a new instance of <see cref="MeshCache"/>.
		/// </summary>
		public MeshCache()
		{
			cache = new Dictionary<long, VertexBatch>();
			expiring = new Dictionary<long, DateTime>();
		}

		#region Mesh lifetime

		/// <summary>
		/// 
		/// </summary>
		public static TimeSpan MeshLifetime { get; set; }

		/// <summary>
		/// 
		/// </summary>
		Dictionary<long, DateTime> expiring;

		/// <summary>
		/// Updates the mesh cache.
		/// </summary>
		/// <param name="clock">Draw thread clock.</param>
		public void Update(IFrameBasedClock clock)
		{
			foreach (var expires in expiring)
			{
				if (DateTime.Now - expires.Value > MeshLifetime)
				{
					cache.Remove(expires.Key);
				}
			}
		}

		#endregion

		#region Mesh management

		/// <summary>
		/// 
		/// </summary>
		Dictionary<long, VertexBatch> cache;

		/// <summary>
		/// Gets the vertexs of a mesh.
		/// </summary>
		/// <param name="mesh">Mesh key.</param>
		/// <returns>Vertex batch.</returns>
		public VertexBatch GetBatch(MeshDescriptor mesh)
		{
			if (!cache.ContainsKey(mesh.DescriptorID))
				return null;

			expiring[mesh.DescriptorID] = DateTime.Now;

			return cache[mesh.DescriptorID];
		}

		/// <summary>
		/// Insert a mesh in the cache.
		/// </summary>
		/// <param name="mesh">Mesh to register.</param>
		/// <param name="batch">Linked batch.</param>
		public void RegisterBatch(MeshDescriptor mesh, VertexBatch batch)
		{
			if (cache.ContainsKey(mesh.DescriptorID))
				return;

			cache[mesh.DescriptorID] = batch;

			expiring[mesh.DescriptorID] = DateTime.Now;
		}

		#endregion
	}
}
