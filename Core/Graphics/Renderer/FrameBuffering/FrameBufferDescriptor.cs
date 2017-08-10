using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;

using pEngine.Core.Graphics.Renderer.Textures;
using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Graphics.Renderer.FrameBuffering
{
    public struct FrameBufferDescriptor : IDependencyDescriptor
    {
		/// <summary>
		/// Frame buffer id.
		/// </summary>
		public long DescriptorID { get; set; }

		/// <summary>
		/// If true this buffer needs to be refreshed.
		/// </summary>
		public bool Invalidated { get; set; }

		/// <summary>
		/// Enables multisampling on this buffer.
		/// </summary>
		public bool Multisampled { get; set; }

		/// <summary>
		/// Frame buffer target texture.
		/// </summary>
		public long TextureId { get; set; }

		/// <summary>
		/// Frame buffer type.
		/// </summary>
		public FrameBufferType Type { get; set; }

		/// <summary>
		/// Frame buffer area size.
		/// </summary>
		public Vector2i Size { get; set; }
    }

	public enum FrameBufferType
	{
		/// <summary>
		/// This buffer will be invalidated on each frame.
		/// </summary>
		Dynamic = 1,

		/// <summary>
		/// This buffer handle invalidation only for graphics changes.
		/// </summary>
		Static = 2,

		/// <summary>
		/// Default label.
		/// </summary>
		None = 0
	}
}
