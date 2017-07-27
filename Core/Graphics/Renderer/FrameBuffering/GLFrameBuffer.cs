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

			Texture = new GLTexture();
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
			if (Target.HasFlag(FramebufferBindMode.DrawBuffer))
			{
				if (BoundMode.HasFlag(FramebufferBindMode.DrawBuffer))
					return;

				Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, Handler);
			}

			if (Target.HasFlag(FramebufferBindMode.ReadBuffer))
			{
				if (BoundMode.HasFlag(FramebufferBindMode.ReadBuffer))
					throw new InvalidOperationException("Buffer already bound.");

				Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, Handler);
			}

			BoundMode |= Target;
		}

		/// <summary>
		/// Close framebuffer and finalize it.
		/// </summary>
		public virtual void End(FramebufferBindMode Target)
		{
			if (Target.HasFlag(FramebufferBindMode.DrawBuffer))
			{
				if (!BoundMode.HasFlag(FramebufferBindMode.DrawBuffer))
					throw new InvalidOperationException("Buffer already unbound.");

				Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			}

			if (Target.HasFlag(FramebufferBindMode.ReadBuffer))
			{
				if (!BoundMode.HasFlag(FramebufferBindMode.ReadBuffer))
					throw new InvalidOperationException("Buffer already unbound.");

				Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
			}

			// TODO: Remove flag operator
			BoundMode = BoundMode & Target;
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
			if (!BoundMode.HasFlag(FramebufferBindMode.DrawBuffer))
				throw new InvalidOperationException("This framebuffer is not bound for drawing.");

			Color4 C = Color4.Black; C.Ab = 0;
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

		/// <summary>
		/// Video buffer texture.
		/// </summary>
		public GLTexture Texture { get; }
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
