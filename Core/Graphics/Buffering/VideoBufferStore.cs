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
        /// <param name="target">Target.</param>
        public VideoBuffer GetVideoBuffer(ITexture target)
        {
            VideoBuffer newVideoBuffer = new VideoBuffer(target);

            base.AddDependency(newVideoBuffer);

            return newVideoBuffer;
        }
    }
}
