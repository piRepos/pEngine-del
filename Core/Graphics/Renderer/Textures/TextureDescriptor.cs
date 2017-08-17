using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Memory;

using pEngine.Core.Data.FrameDependency;

namespace pEngine.Core.Graphics.Renderer.Textures
{
    public struct TextureDescriptor : IDependencyDescriptor
    {
		/// <summary>
		/// Texture data.
		/// </summary>
		public PixelBuffer[] Buffer { get; set; }

		/// <summary>
		/// Mipmaps generation delegated to GPU.
		/// </summary>
		public bool GenerateMipmaps { get; set; }

		/// <summary>
		/// Current texture mipmap level.
		/// </summary>
		public uint Mipmaps { get; set; }

		/// <summary>
		/// Actual dependency load state.
		/// </summary>
		public DependencyState State { get; set; }

		/// <summary>
		/// Identifier for this instance of descriptor.
		/// </summary>
		public long DescriptorID { get; set; }
	}
}
