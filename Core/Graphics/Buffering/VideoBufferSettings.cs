using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.FrameBuffering;
using pEngine.Core.Graphics.Textures;

namespace pEngine.Core.Graphics.Buffering
{
    public struct VideoBufferSettings
    {
		public VideoBufferSettings(bool enabled = false)
		{
			Enabled = enabled;
			Draw = true;
			Bypass = false;
			VideoBuffer = null;
			lastInvalidationFrame = -1;
		}

		/// <summary>
		/// Sets buffering for this object enabled or disabled.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// If false buffer will be filled but not drawed in the scene.
		/// </summary>
		public bool Draw { get; set; }

		/// <summary>
		/// If true the buffered objects will be drawed in each frame.
		/// </summary>
		public bool Bypass { get; set; }

		/// <summary>
		/// Object video buffer (if the property FrameBuffered is false this property is null).
		/// </summary>
		public VideoBuffer VideoBuffer { get; set; }

		/// <summary>
		/// Object texture (if the property FrameBuffered is false this property is null).
		/// </summary>
		public ITexture Texture => VideoBuffer?.TargetTexture;

		#region Invalidation

		long lastInvalidationFrame;

		/// <summary>
		/// Invalidate this frame buffer.
		/// </summary>
		/// <param name="frame">Current frame.</param>
		public void Invalidate(long frame)
		{
			lastInvalidationFrame = frame;
		}

		/// <summary>
		/// Checks if this buffer needs to be redrawed.
		/// </summary>
		/// <param name="frame">Current frame.</param>
		/// <returns>True if needs redraw.</returns>
		public bool NeedsRedraw(long frame)
		{
			return lastInvalidationFrame >= frame || !Enabled || Bypass;
		}

		#endregion

	}
}
