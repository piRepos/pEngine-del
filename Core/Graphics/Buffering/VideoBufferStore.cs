using System;

using pEngine.Core.Graphics.Textures;
using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Graphics.Renderer.FrameBuffering;

namespace pEngine.Core.Graphics.Buffering
{
    public class VideoBufferStore : FrameDependencyManager<VideoBuffer, FrameBufferDescriptor>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:pEngine.Core.Graphics.Buffering.VideoBufferStore"/> class.
        /// </summary>
        public VideoBufferStore(pEngine host)
            : base(host)
        {
        }

        /// <summary>
        /// Gets a new video buffer from a texture.
        /// </summary>
        /// <returns>The video buffer.</returns>
        /// <param name="target">Target texture.</param>
        public VideoBuffer GetVideoBuffer(ITexture target)
        {
            VideoBuffer newVideoBuffer = new VideoBuffer(target);

            base.AddDependency(newVideoBuffer);

            return newVideoBuffer;
        }

		/// <summary>
		/// Gets a new video buffer.
		/// </summary>
		/// <returns>The video buffer.</returns>
		public VideoBuffer GetVideoBuffer(Vector2i size)
		{
			VideoBuffer newVideoBuffer = new VideoBuffer(host.Textures.GetTexture(size));

			base.AddDependency(newVideoBuffer);

			return newVideoBuffer;
		}

		protected override void OnDependencyChange(VideoBuffer dependency)
		{

		}

		protected override void OnDependencyDispose(VideoBuffer dependency)
		{

		}

		protected override void OnDependencyLoad(VideoBuffer dependency)
		{

		}
	}
}
