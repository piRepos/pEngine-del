using System;

using pEngine.Core.Graphics.Renderer.Textures;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.FrameBuffering
{
	public class GLFrameBuffer
	{
		public GLFrameBuffer(Vector2i Size)
		{
			this.Size = Size;

			Handler = Gl.GenFramebuffer();
		}

		#region Handler

		/// <summary>
		/// Opengl handler.
		/// </summary>
		public virtual uint Handler { get; set; }

		/// <summary>
		/// Set this buffer multisampled.
		/// </summary>
		public virtual bool Multisampled { get; set; }

		#endregion

		#region Binding

		/// <summary>
		/// Current mode bound.
		/// </summary>
		public FramebufferBindMode BoundMode { get; private set; }

		/// <summary>
		/// Open framebuffer for drawing.
		/// </summary>
		public virtual void Begin(FramebufferBindMode Target)
		{
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handler);
		}

		/// <summary>
		/// Close framebuffer and finalize it.
		/// </summary>
		public virtual void End(FramebufferBindMode Target)
		{
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		#endregion

		/// <summary>
		/// Reset this framebuffer.
		/// </summary>
		public virtual void ResetBuffer()
		{

		}

		/// <summary>
		/// Clear framebuffer content.
		/// </summary>
		public virtual void Clear()
		{
			Color4 C = Color4.Black; C.Ab = 0;
			Gl.Viewport(0, 0, Size.Width, Size.Height);
			Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			Gl.ClearBuffer(OpenGL.Buffer.Color, 0, new float[] { C.Rf, C.Gf, C.Bf, C.Af });
		}

		/// <summary>
		/// Gets if the frame buffers are supported.
		/// </summary>
		public bool IsSupported => Gl.MAJOR_VERSION >= 3;

		/// <summary>
		/// Framebuffer size.
		/// </summary>
		public Vector2i Size { get; set; }
	}

	/// <summary>
	/// Begin mode.
	/// </summary>
	[Flags]
	public enum FramebufferBindMode
	{
		/// <summary>
		/// Open buffer in read mode.
		/// </summary>
		ReadBuffer = 1,

		/// <summary>
		/// Open buffer in draw mode.
		/// </summary>
		DrawBuffer = 2,

		/// <summary>
		/// Open buffer in standard mode.
		/// </summary>
		Buffer = ReadBuffer | DrawBuffer,

		/// <summary>
		/// Buffer not bound.
		/// </summary>
		None = 0
	}
}
