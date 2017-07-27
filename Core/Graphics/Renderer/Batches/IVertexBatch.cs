using System;

using OpenGL;

using pEngine.Common.Memory;
using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public interface IVertexBatch : IDependency<BatchDescriptor>
    {

		/// <summary>
		/// Primitive openGL type.
		/// </summary>
		PrimitiveType Primitive { get; }

		/// <summary>
		/// Batch vertexs.
		/// </summary>
		ArrayHandler<GLVertex> Vertexs { get; }

		/// <summary>
		/// Batch indexes.
		/// </summary>
		ArrayHandler<uint> Indexes { get; }

		/// <summary>
		/// Batch invalidation type.
		/// </summary>
		BatchInvalidationType InvalidationType { get; }

    }

	[Flags]
	public enum BatchInvalidationType
	{
		/// <summary>
		/// No invalidation.
		/// </summary>
		None = 0,

		/// <summary>
		/// Vertexs values are changed.
		/// </summary>
		Vertexs = 1 << 0,

		/// <summary>
		/// Indexes are changed.
		/// </summary>
		Indexes = 1 << 1,

		/// <summary>
		/// All batch is changed.
		/// </summary>
		Both = Vertexs | Indexes
	}
}
