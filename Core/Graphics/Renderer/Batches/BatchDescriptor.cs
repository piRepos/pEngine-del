using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public struct BatchDescriptor : IDependencyDescriptor
    {

		/// <summary>
		/// Vertexs static array.
		/// </summary>
		public GLVertex[] Vertexs { get; set; }

		/// <summary>
		/// Indexes static array.
		/// </summary>
		public uint[] Indexes { get; set; }

		/// <summary>
		/// Vertex array start position.
		/// </summary>
		public long VertexOffset { get; set; }

		/// <summary>
		/// Index array start position.
		/// </summary>
		public long IndexOffset { get; set; }

		/// <summary>
		/// Descriptor id.
		/// </summary>
		public long DescriptorID { get; set; }

		/// <summary>
		/// Batch invalidation type.
		/// </summary>
		public BatchInvalidationType InvalidationType { get; set; }

	}
}
