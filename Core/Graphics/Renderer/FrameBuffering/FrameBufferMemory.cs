using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.FrameBuffering
{
    public class FrameBufferMemory
	{
		private Renderer renderer;

		/// <summary>
		/// Makes a new instance of <see cref="FrameBufferMemory"/> class.
		/// </summary>
		public FrameBufferMemory(Renderer renderer)
		{
			this.renderer = renderer;

			loadedBuffers = new Dictionary<long, GLFrameBuffer>();
		}

		#region References

		public Dictionary<long, GLFrameBuffer> loadedBuffers;

		/// <summary>
		/// Gets a framebuffer from a resource id.
		/// </summary>
		/// <param name="id">Framebuffer id.</param>
		/// <returns>OpenGL framebuffer instance.</returns>
		public GLFrameBuffer GetBuffer(long id)
		{
			// TODO: handle exception
			return loadedBuffers[id];
		}

		#endregion

		#region Loader

		/// <summary>
		/// Loads a new framebuffer.
		/// </summary>
		/// <param name="descriptor">Framebuffer descriptor.</param>
		public void DispatchDescriptor(FrameBufferDescriptor descriptor)
		{
			switch (descriptor.State)
			{
				case Data.FrameDependency.DependencyState.NotLoaded:
					LoadBuffer(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Modified:
					UpdateBuffer(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Disposed:
					DisposeBuffer(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Loaded:
					return;
			}
		}

		#endregion

		#region Load

		private void LoadBuffer(FrameBufferDescriptor info)
		{
			if (!loadedBuffers.ContainsKey(info.DescriptorID))
			{
				GLFrameBuffer buffer = new GLFrameBuffer(info.Size);

				buffer = new GLFrameBuffer(info.Size)
				{
					Multisampled = info.Multisampled
				};

				var bufferTextue = renderer.Textures.ResolveTexture(info.TextureId);
				bufferTextue.Attach(buffer);

				loadedBuffers.Add(info.DescriptorID, buffer);
			}
		}

		#endregion

		#region Update

		private void UpdateBuffer(FrameBufferDescriptor info)
		{
			if (loadedBuffers.ContainsKey(info.DescriptorID))
			{
				var oldBuffer = loadedBuffers[info.DescriptorID];
				oldBuffer.Size = info.Size;

				var bufferTexture = renderer.Textures.ResolveTexture(info.TextureId);
				bufferTexture.Size = info.Size;
				bufferTexture.Attach(oldBuffer, true);
			}
		}

		#endregion

		#region Dispose

		private void DisposeBuffer(FrameBufferDescriptor info)
		{
			if (loadedBuffers.ContainsKey(info.DescriptorID))
			{
				var oldBuffer = loadedBuffers[info.DescriptorID];
				oldBuffer.Dispose();

				loadedBuffers.Remove(info.DescriptorID);
			}
		}

		#endregion
	}
}
