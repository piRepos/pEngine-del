using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.FrameBuffering
{
    class FrameBufferBatch
    {
		private Renderer renderer;

		/// <summary>
		/// Makes a new instance of <see cref="FrameBufferBatch"/> class.
		/// </summary>
		public FrameBufferBatch(Renderer renderer)
		{
			this.renderer = renderer;
		}

		#region References

		public Dictionary<long, GLFrameBuffer> bufferStore;

		/// <summary>
		/// Gets a framebuffer from a resource id.
		/// </summary>
		/// <param name="id">Framebuffer id.</param>
		/// <returns>OpenGL framebuffer instance.</returns>
		public GLFrameBuffer GetBuffer(long id)
		{
			// TODO: handle exception
			return bufferStore[id];
		}

		#endregion

		#region Loader

		/// <summary>
		/// Loads a new framebuffer.
		/// </summary>
		/// <param name="descriptor">Framebuffer descriptor.</param>
		public void LoadBuffer(FrameBufferDescriptor descriptor)
		{
			GLFrameBuffer buffer;

			if (bufferStore.ContainsKey(descriptor.DescriptorID))
				buffer = bufferStore[descriptor.DescriptorID];
			else
			{
				buffer = new GLFrameBuffer(descriptor.Size);
				bufferStore.Add(descriptor.DescriptorID, buffer);
			}

			buffer.Size = descriptor.Size;
			buffer.Multisampled = descriptor.Multisampled;
			
			var bufferTextue = renderer.Textures.ResolveTexture(descriptor.TextureId);
			bufferTextue.Attach(buffer);
		}

		#endregion
	}
}
