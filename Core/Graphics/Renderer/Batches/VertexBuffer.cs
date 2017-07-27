using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Memory;
using pEngine.Common.Timing.Base;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.Batches
{
	public class VertexSegment : ArrayHandler<GLVertex>
	{
		public VertexSegment(DistributedArray<GLVertex> parent, int offset, int size)
			: base(parent, offset, size)
		{

		}
	}

	public class VertexBuffer : DistributedArray<GLVertex>
    {
		/// <summary>
		/// Makes a new instance of <see cref="VertexManager"/>.
		/// </summary>
		public VertexBuffer()
			: base(10D)
		{

		}

		/// <summary>
		/// OpenGL vertex buffer object handler.
		/// </summary>
		public uint Handler { get; private set; }
		
		/// <summary>
		/// Initialize all OpenGL stuff.
		/// </summary>
		public void Initialize(uint glBuffer)
		{
			Handler = Gl.GenBuffer();
		}

		#region Degradation management

		double lastDefragElapsed = 0;

		/// <summary>
		/// Update memory state and manage space
		/// </summary>
		/// <param name="DeltaTime">Delta time</param>
		public void Update(IFrameBasedClock Clock)
		{
			lastDefragElapsed += Clock.ElapsedFrameTime / 1000D;

			if (lastDefragElapsed > 5D)
			{
				// Every 5 seconds check fragmentation
				if (FragmentationIndex >= 0.5D)
					Defrag();
			}
		}

		#endregion

		#region OpenGL

		/// <summary>
		/// Upload all vertex to video memory
		/// </summary>
		public void UploadAllHeap()
		{
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handler);
			Gl.BufferData(BufferTargetARB.ArrayBuffer, Count * GLVertex.Stride, ToArray(), BufferUsageARB.StaticDraw);
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		}

		/// <summary>
		/// Bind this instance.
		/// </summary>
		public void Bind()
		{
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handler);
		}

		/// <summary>
		/// Unbind this instance.
		/// </summary>
		public void Unbind()
		{
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		}

		#endregion

	}
}
