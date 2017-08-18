using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public class GLVertexMemory
    {

		/// <summary>
		/// Makes a new instance of <see cref="GLVertexMemory"/>.
		/// </summary>
		public GLVertexMemory()
		{

		}

		/// <summary>
		/// OpenGL array handler.
		/// </summary>
		public uint ArrayHandler { get; private set; }

		/// <summary>
		/// Vertex bugger for vertexs storage.
		/// </summary>
		public uint VertexHandler { get; private set; }

		/// <summary>
		/// Index buffer for indexes storage.
		/// </summary>
		public uint IndexHandler { get; private set; }

		/// <summary>
		/// Vertex array size in VRAM.
		/// </summary>
		public int VertexArraySize { get; private set; }

		/// <summary>
		/// Index array size in VRAM.
		/// </summary>
		public int IndexArraySize { get; private set; }

		/// <summary>
		/// Initialize opengl handlers.
		/// </summary>
		public void Initialize()
		{
			// Vertex array generation
			ArrayHandler = Gl.GenVertexArray();

			// Buffers for store vertexs and indexes
			VertexHandler = Gl.GenBuffer();
			IndexHandler = Gl.GenBuffer();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vertexs"></param>
		/// <param name="indexes"></param>
		public void ResizeMemory(int vertexs, int indexes)
		{
			if (vertexs != VertexArraySize)
			{
				Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexHandler);

				IntPtr old = IntPtr.Zero;

				if (VertexArraySize > 0)
				{
					old = Marshal.AllocHGlobal(vertexs * (int)GLVertex.Stride);
					Gl.GetBufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (uint)VertexArraySize * GLVertex.Stride, old);
				}

				Gl.BufferData(BufferTarget.ArrayBuffer, (uint)vertexs * GLVertex.Stride, old, BufferUsage.StaticDraw);

				if (VertexArraySize > 0)
					Marshal.FreeHGlobal(old);

				VertexArraySize = vertexs;
			}

			if (indexes != IndexArraySize)
			{
				Gl.BindBuffer(BufferTarget.ElementArrayBuffer, IndexHandler);

				IntPtr old = IntPtr.Zero;

				if (VertexArraySize > 0)
				{
					old = Marshal.AllocHGlobal(indexes * sizeof(uint));
					Gl.GetBufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, (uint)IndexArraySize * sizeof(uint), old);
				}

				Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)indexes * sizeof(uint), old, BufferUsage.StaticDraw);

				if (VertexArraySize > 0)
					Marshal.FreeHGlobal(old);

				IndexArraySize = indexes;
			}
		}

		/// <summary>
		/// Upload/update a vertex batch in the VRAM.
		/// </summary>
		public void LoadBatch(BatchDescriptor batch)
		{
			switch (batch.State)
			{
				case Data.FrameDependency.DependencyState.NotLoaded:
				case Data.FrameDependency.DependencyState.Modified:

					if (batch.InvalidationType.HasFlag(BatchInvalidationType.Vertexs))
					{
						Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexHandler);

						Gl.BufferSubData(
							BufferTarget.ArrayBuffer,
							new IntPtr(batch.VertexOffset * GLVertex.Stride),
							(uint)batch.Vertexs.Length * GLVertex.Stride,
							batch.Vertexs
							);

						Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
					}

					if (batch.InvalidationType.HasFlag(BatchInvalidationType.Indexes))
					{
						Gl.BindBuffer(BufferTarget.ElementArrayBuffer, IndexHandler);

						Gl.BufferSubData(
							BufferTarget.ElementArrayBuffer,
							new IntPtr(batch.IndexOffset * sizeof(uint)),
							(uint)batch.Indexes.Length * sizeof(uint),
							batch.Indexes
							);

						Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
					}

					break;

				case Data.FrameDependency.DependencyState.Disposed:
				case Data.FrameDependency.DependencyState.Loaded:
					break;
			}
			
		}

		/// <summary>
		/// Bind this instance.
		/// </summary>
		public void Bind(int posIndex = 0, int txCoordIndex = 1, int colorIndex = 2)
		{
			Gl.BindVertexArray(ArrayHandler);
			Gl.BindBuffer(BufferTarget.ArrayBuffer, VertexHandler);
			Gl.BindBuffer(BufferTarget.ElementArrayBuffer, IndexHandler);

			if (posIndex >= 0)
			{
				Gl.EnableVertexAttribArray((uint)posIndex);
				Gl.VertexAttribPointer((uint)posIndex, 2, VertexAttribType.Float, false, (int)GLVertex.Stride, IntPtr.Zero);
			}

			if (txCoordIndex >= 0)
			{
				Gl.EnableVertexAttribArray((uint)txCoordIndex);
				Gl.VertexAttribPointer((uint)txCoordIndex, 2, VertexAttribType.Float, false, (int)GLVertex.Stride, new IntPtr(2 * sizeof(float)));
			}

			if (colorIndex >= 0)
			{
				Gl.EnableVertexAttribArray((uint)colorIndex);
				Gl.VertexAttribPointer((uint)colorIndex, 4, VertexAttribType.Float, false, (int)GLVertex.Stride, new IntPtr(4 * sizeof(float)));
			}
		}

		/// <summary>
		/// Unbind this instance.
		/// </summary>
		public void Unbind()
		{
			Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
			Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			Gl.BindVertexArray(0);
		}
	}
}
