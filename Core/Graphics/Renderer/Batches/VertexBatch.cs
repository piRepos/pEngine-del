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
    public abstract class VertexBatch : IVertexBatch
    {

		/// <summary>
		/// Makes a new instance of <see cref="VertexBatch"/>
		/// </summary>
		/// <param name="vertexs">Vertex heap.</param>
		/// <param name="indexes">Index heap.</param>
		/// <param name="vPrimitive">Number of vertexs for a single primitive.</param>
		/// <param name="iPrimitive">Number of indexes for primitive.</param>
		/// <param name="primitives">Number of preallocated primitives.</param>
		internal VertexBatch(DistributedArray<GLVertex> vertexs, DistributedArray<uint> indexes, int vPrimitive, int iPrimitive, int primitives)
		{
			Vertexs = vertexs.Alloc(vPrimitive * primitives);
			Indexes = indexes.Alloc(iPrimitive * primitives);

			VertexsForPrimitive = vPrimitive;
			IndexesForPrimitive = iPrimitive;

			vertexManager = vertexs;
			indexManager = indexes;

			PrimitivesCount = primitives;

			vertexs.Defragged += OnDefrag;

			BuildIndexes();

			InvalidationType |= BatchInvalidationType.Both;
			State = DependencyState.NotLoaded;
		}

		/// <summary>
		/// Makes a new instance of <see cref="VertexBatch"/>
		/// </summary>
		/// <param name="vertexs">Vertex heap.</param>
		/// <param name="indexes">Index heap.</param>
		internal VertexBatch(DistributedArray<GLVertex> vertexs, DistributedArray<uint> indexes)
		{
			VertexsForPrimitive = 1;
			IndexesForPrimitive = 1;

			vertexManager = vertexs;
			indexManager = indexes;

			PrimitivesCount = 0;

			vertexs.Defragged += OnDefrag;

			BuildIndexes();

			InvalidationType |= BatchInvalidationType.Both;
			State = DependencyState.NotLoaded;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="VertexBatch"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="VertexBatch"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="VertexBatch"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="VertexBatch"/> so
		/// the garbage collector can reclaim the memory that the <see cref="VertexBatch"/> was occupying.</remarks>
		public void Dispose()
		{
			Vertexs.Dispose();
			Indexes.Dispose();

			State = DependencyState.Disposed;
		}

		DistributedArray<GLVertex> vertexManager;
		DistributedArray<uint> indexManager;

		/// <summary>
		/// Batch invalidation type.
		/// </summary>
		public BatchInvalidationType InvalidationType { get; set; }

		/// <summary>
		/// Primitive openGL type.
		/// </summary>
		public PrimitiveType Primitive { get; protected set; }

		/// <summary>
		/// Primitives count.
		/// </summary>
		public int PrimitivesCount { get; protected set; }

		/// <summary>
		/// Number of vertexs for primitive.
		/// </summary>
		public int VertexsForPrimitive { get; protected set; }

		/// <summary>
		/// Number of indexes for primitive.
		/// </summary>
		public int IndexesForPrimitive { get; protected set; }

		/// <summary>
		/// Batch vertexs.
		/// </summary>
		public ArrayHandler<GLVertex> Vertexs { get; private set; }

		/// <summary>
		/// Batch indexes.
		/// </summary>
		public ArrayHandler<uint> Indexes { get; private set; }

		#region Allocation

		/// <summary>
		/// Resize this vertex batch.
		/// </summary>
		/// <param name="vertexCount">New vertex count.</param>
		/// <param name="indexCount">New index count.</param>
		public void Resize(int vertexCount, int indexCount)
		{
			bool changed = false;

			if (vertexCount != Vertexs.Size)
			{
				GLVertex[] tmp = new GLVertex[Vertexs.Size];

				Array.Copy(vertexManager.Memory, Vertexs.Offset, tmp, 0, Vertexs.Size);
				
				Vertexs.Dispose();
				Vertexs = vertexManager.Alloc(vertexCount);

				Array.Copy(tmp, 0, vertexManager.Memory, Vertexs.Offset, Math.Min(tmp.Length, vertexCount));

				InvalidationType |= BatchInvalidationType.Vertexs;

				changed = true;
			}

			if (indexCount != Indexes.Size)
			{
				uint[] tmp = new uint[Indexes.Size];

				Array.Copy(indexManager.Memory, Indexes.Offset, tmp, 0, Indexes.Size);

				Indexes.Dispose();
				Indexes = indexManager.Alloc(indexCount);
				
				Array.Copy(tmp, 0, indexManager.Memory, Indexes.Offset, Math.Min(tmp.Length, indexCount));

				InvalidationType |= BatchInvalidationType.Indexes;

				changed = true;
			}

			if (changed)
			{
				BuildIndexes();
				InvalidateDependency();
			}
		}

		/// <summary>
		/// Resize this vertex batch.
		/// </summary>
		/// <param name="primitives">Number of primitives.</param>
		public void Resize(int primitives)
		{
			PrimitivesCount = primitives;

			Resize(primitives * VertexsForPrimitive, primitives * IndexesForPrimitive);
		}

		#endregion

		#region Indexes management

		/// <summary>
		/// Virtual method that crerates indexes array.
		/// </summary>
		public virtual void BuildIndexes()
		{
			InvalidationType |= BatchInvalidationType.Indexes;
			InvalidateDependency();
		}

		private void OnDefrag(object sender, EventArgs e)
		{
			BuildIndexes();

			InvalidationType |= BatchInvalidationType.Both;
			InvalidateDependency();
		}

		#endregion

		#region Dependency

		/// <summary>
		/// Sets the dependency modified.
		/// </summary>
		public void InvalidateDependency()
		{
			if (State == DependencyState.Loaded)
				State = DependencyState.Modified;
		}

		/// <summary>
		/// Actual dependency load state.
		/// </summary>
		public DependencyState State { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		public BatchDescriptor GetDescriptor()
		{
			var d = new BatchDescriptor
			{
				Vertexs = new GLVertex[Vertexs.Size],
				Indexes = new uint[Indexes.Size],
				VertexOffset = Vertexs.Offset,
				IndexOffset = Indexes.Offset,
				InvalidationType = InvalidationType,
				DescriptorID = DependencyID
			};

			Array.Copy(Vertexs.MemoryRef.Memory, Vertexs.Offset, d.Vertexs, 0, Vertexs.Size);
			Array.Copy(Indexes.MemoryRef.Memory, Indexes.Offset, d.Indexes, 0, Indexes.Size);

			return d;
		}

		#endregion

	}
}
