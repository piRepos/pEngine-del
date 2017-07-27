using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public class BatchesManager
    {

		/// <summary>
		/// Makes a new instance of <see cref="BatchesManager"/>.
		/// </summary>
		public BatchesManager()
		{
			Verticies = new VertexBuffer();
			Indicies = new IndexBuffer();
		}

		#region Modules

		/// <summary>
		/// Batches manager opengl handler.
		/// </summary>
		public uint Handler { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public VertexBuffer Verticies { get; }

		/// <summary>
		/// 
		/// </summary>
		public IndexBuffer Indicies { get; }

		#endregion

		#region Batches

		/// <summary>
		/// Creates a new <see cref="VertexBatch"/>.
		/// </summary>
		/// <returns>The <see cref="VertexBatch"/>.</returns>
		public VertexBatch GetBatch()
		{
			return new VertexBatch(this);
		}

		#endregion

		#region Management

		/// <summary>
		/// Initialize all OpenGL stuff.
		/// </summary>
		public void Initialize()
		{
			Handler = Gl.GenVertexArray();

			Verticies.Initialize(Handler);
			Indicies.Initialize(Handler);
		}

		/// <summary>
		/// 
		/// </summary>
		public void UploadBatches()
		{
			Gl.BindVertexArray(Handler);

			Verticies.UploadAllHeap();
			Indicies.UploadAllHeap();

			Gl.BindVertexArray(0);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Bind()
		{
			Gl.BindVertexArray(Handler);
			Verticies.Bind();
			Indicies.Bind();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Unbind()
		{
			Indicies.Unbind();
			Verticies.Unbind();
			Gl.BindVertexArray(0);
		}

		#endregion

	}
}
