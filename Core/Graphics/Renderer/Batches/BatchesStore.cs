using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;

using pEngine.Common.Memory;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public class BatchesStore : FrameDependencyManager<IVertexBatch, BatchDescriptor>, IDisposable
    {

		/// <summary>
		/// Makes a new <see cref="BatchesStore"/>.
		/// </summary>
		/// <param name="host">Game host.</param>
		public BatchesStore(pEngine host)
			: base(host)
		{
			VertexHeap = new DistributedArray<GLVertex>();
			IndexHeap = new DistributedArray<uint>();

			DefaultQuad = GetBatch<QuadVertexBatch>(1);


			DefaultQuad.Vertexs[0] = new GLVertex
			{
				X = 0,
				Y = 0,
				R = 1,
				G = 1,
				B = 1,
				A = 1,
				Tx = 0,
				Ty = 1
			};

			DefaultQuad.Vertexs[1] = new GLVertex
			{
				X = 1,
				Y = 0,
				R = 1,
				G = 1,
				B = 1,
				A = 1,
				Tx = 1,
				Ty = 1
			};

			DefaultQuad.Vertexs[2] = new GLVertex
			{
				X = 1,
				Y = 1,
				R = 1,
				G = 1,
				B = 1,
				A = 1,
				Tx = 1,
				Ty = 0
			};

			DefaultQuad.Vertexs[3] = new GLVertex
			{
				X = 0,
				Y = 1,
				R = 1,
				G = 1,
				B = 1,
				A = 1,
				Tx = 0,
				Ty = 0
			};
		}

		/// <summary>
		/// Vertex heap that manage all vertexs in the physics thread.
		/// </summary>
		DistributedArray<GLVertex> VertexHeap;

		/// <summary>
		/// Index heap that manage all uint indicies in the physics thread.
		/// </summary>
		DistributedArray<uint> IndexHeap;

		/// <summary>
		/// Vertex heap size.
		/// </summary>
		public uint VertexHeapSize => VertexHeap.Count;

		/// <summary>
		/// Index heap size.
		/// </summary>
		public uint IndexHeapSize => IndexHeap.Count;

		/// <summary>
		/// Quad for generic use.
		/// </summary>
		public QuadVertexBatch DefaultQuad { get; private set; }

		/// <summary>
		/// Releases all resource used by the <see cref="BatchesStore"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="BatchesStore"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="BatchesStore"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="BatchesStore"/> so
		/// the garbage collector can reclaim the memory that the <see cref="BatchesStore"/> was occupying.</remarks>
		public override void Dispose()
		{

		}

		/// <summary>
		/// Creates a new instance of <see cref="VertexBatch"/>.
		/// </summary>
		/// <typeparam name="Type">Type of Vertex batch (Default vertexBatch)</typeparam>
		/// <param name="primitives">Number of primitives.</param>
		/// <returns>A vertex batch.</returns>
		public Type GetBatch<Type>(int primitives) 
			where Type : VertexBatch
		{
			var param = new object[] { VertexHeap, IndexHeap, primitives };
			Type b = Activator.CreateInstance(typeof(Type), param) as Type;

			AddDependency(b);

			defragCounter++;

			return b;
		}

		#region Heap management

		long defragCounter = 0;

		public override IEnumerable<BatchDescriptor> GetDependencyDescriptors()
		{

			if (defragCounter > 10)
			{
				bool defragged = false;

				if (VertexHeap.FragmentationIndex > 0.5)
				{
					VertexHeap.Defrag();
					defragged = true;
				}

				if (IndexHeap.FragmentationIndex > 0.5)
				{
					IndexHeap.Defrag();
					defragged = true;
				}

				if (defragged)
				{
					foreach (var dep in Dependencies)
						dep.InvalidateDependency();
				}
			}

			return base.GetDependencyDescriptors();
		}

		#endregion

		#region Dependency management

		protected override void OnDependencyChange(IVertexBatch dependency)
		{
		}

		protected override void OnDependencyDispose(IVertexBatch dependency)
		{
		}

		protected override void OnDependencyLoad(IVertexBatch dependency)
		{
		}

		#endregion
	}
}
