using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

using pEngine.Common.Memory;
using pEngine.Common.Math;
using pEngine.Common;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public class QuadVertexBatch : VertexBatch
    {
		/// <summary>
		/// Creates a new instance of <see cref="QuadVertexBatch"/> class.
		/// </summary>
		/// <param name="vertexs">Vertex array heap.</param>
		/// <param name="indexes">Index array heap.</param>
		/// <param name="count">Number of primitives.</param>
		public QuadVertexBatch(DistributedArray<GLVertex> vertexs, DistributedArray<uint> indexes, int count)
			: base(vertexs, indexes, 4, 6, count)
		{
			Primitive = PrimitiveType.Triangles;
		}

		public override void BuildIndexes()
		{
			for (uint i = 0; i < PrimitivesCount; ++i)
			{
				Indexes.MemoryRef.Memory[Indexes.Offset + 0 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 0;
				Indexes.MemoryRef.Memory[Indexes.Offset + 1 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 1;
				Indexes.MemoryRef.Memory[Indexes.Offset + 2 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 2;

				Indexes.MemoryRef.Memory[Indexes.Offset + 3 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 0;
				Indexes.MemoryRef.Memory[Indexes.Offset + 4 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 3;
				Indexes.MemoryRef.Memory[Indexes.Offset + 5 + (i * 6)] = (uint)Vertexs.Offset + (i * 4) + 2;
			}

			base.BuildIndexes();
		}

	}
}
