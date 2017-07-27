using System;

using pEngine.Core.Graphics.Textures;
using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Graphics.Renderer.FrameBuffering;


namespace pEngine.Core.Graphics.Buffering
{
    public class VideoBuffer : IDependency<FrameBufferDescriptor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> class.
        /// </summary>
        /// <param name="size">Buffer size.</param>
        public VideoBuffer(ITexture targetTexture)
        {
            TargetTexture = targetTexture;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> so the garbage collector can reclaim the memory
        /// that the <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> was occupying.</remarks>
        public void Dispose()
        {
            
        }

        /// <summary>
        /// Gets or sets the buffer size in pixels.
        /// </summary>
        public Vector2i Size => TargetTexture.Size;

        /// <summary>
        /// Gets or sets the framebuffer type.
        /// </summary>
        /// <value>
        /// Can be static or dynamic; Use static for improve
        /// performance on low framerate objects.
        /// </value>
        public FrameBufferType Type { get; set; }

        /// <summary>
        /// Gets or sets the target texture.
        /// </summary>
        public ITexture TargetTexture { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:pEngine.Core.Graphics.Buffering.VideoBuffer"/> is invalidated.
        /// </summary>
        public bool VideoInvalidated { get; private set; }

        /// <summary>
        /// Force the target items to redraw on this buffer.
        /// </summary>
        public void Invalidate()
        {
            VideoInvalidated = true;
        }

		#region Dependency

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		public FrameBufferDescriptor GetDescriptor()
        {
            return new FrameBufferDescriptor
            {
                Invalidated = VideoInvalidated,
                Size = Size,
                Type = Type,
                TextureId = TargetTexture.DependencyID,
                DescriptorID = DependencyID
            };
        }

		/// <summary>
		/// True if the resource is changed.
		/// </summary>
		public bool Invalidated { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

        #endregion
    }
}
