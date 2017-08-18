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
			Size = targetTexture.Size;
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
			State = DependencyState.Disposed;
        }

        /// <summary>
        /// Gets or sets the buffer size in pixels.
        /// </summary>
        public Vector2i Size { get; set; }

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
				State = State,
                Size = Size,
                Type = Type,
                TextureId = TargetTexture.DependencyID,
                DescriptorID = DependencyID
            };
        }

		/// <summary>
		/// Sets the dependency modified.
		/// </summary>
		public void InvalidateDependency()
		{
			if (State == DependencyState.Loaded)
				State = DependencyState.Modified;
		}

		/// <summary>
		/// Actual dependency load state.
		/// </summary>
		public DependencyState State { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

        #endregion
    }
}
