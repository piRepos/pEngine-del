using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Tessellator;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer.Textures;
using pEngine.Core.Graphics.Renderer.FrameBuffering;

namespace pEngine.Core.Graphics.Renderer
{
    struct FrameRenderInfo
    {
		/// <summary>
		/// Asset list to render.
		/// </summary>
		public IEnumerable<Asset> Assets;

		/// <summary>
		/// Pending vertex batches to load.
		/// </summary>
		public IEnumerable<BatchDescriptor> Batches;

		/// <summary>
		/// Pending textures to load.
		/// </summary>
		public IEnumerable<TextureDescriptor> Textures;

        /// <summary>
        /// Pending buffers to pre-load.
        /// </summary>
        public IEnumerable<FrameBufferDescriptor> Buffers;

		/// <summary>
		/// Vertex memory heap size.
		/// </summary>
		public int VertexMemorySize;

		/// <summary>
		/// Index memory heap size.
		/// </summary>
		public int IndexMemorySize;

		/// <summary>
		/// Frame identifier.
		/// </summary>
		public long FrameID;
    }
}
