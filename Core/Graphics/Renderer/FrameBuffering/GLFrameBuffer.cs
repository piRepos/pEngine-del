using System;

using pEngine.Core.Graphics.Renderer.Textures;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.FrameBuffering
{
	public class GLFrameBuffer : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Size"></param>
		public GLFrameBuffer(Vector2i size)
		{
			Size = size;
			Handler = Gl.GenFramebuffer();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="GLFrameBuffer"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="GLFrameBuffer"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="GLFrameBuffer"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="GLFrameBuffer"/> so
		/// the garbage collector can reclaim the memory that the <see cref="GLFrameBuffer"/> was occupying.</remarks>
		public void Dispose()
		{
			Gl.DeleteBuffers(Handler);
		}

		#region Properties

		/// <summary>
		/// Opengl handler.
		/// </summary>
		public virtual uint Handler { get; set; }

		/// <summary>
		/// Set this buffer multisampled.
		/// </summary>
		public virtual bool Multisampled { get; set; }

		/// <summary>
		/// Framebuffer size.
		/// </summary>
		public Vector2i Size { get; set; }

		#endregion

		#region Binding

		/// <summary>
		/// Open framebuffer for drawing.
		/// </summary>
		public virtual void Begin()
		{
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handler);
		}

		/// <summary>
		/// Close framebuffer and finalize it.
		/// </summary>
		public virtual void End()
		{
			Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

		#endregion

		#region Attachment

		public void Attach(GLTexture texture)
		{
			Begin();
			{
				texture.PreAlloc(Size);

				Gl.FramebufferTexture2D
				(
					FramebufferTarget.DrawFramebuffer, 
					FramebufferAttachment.ColorAttachment0, 
					TextureTarget.Texture2d, 
					texture.Handler, 0
				);

				StatusCheck();
			}
			End();
		}

		private void StatusCheck()
		{
			// TODO: Handle errors.

			switch (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
			{
				case FramebufferStatus.FramebufferComplete:
					return;
				case FramebufferStatus.FramebufferIncompleteAttachment:
				case FramebufferStatus.FramebufferIncompleteDrawBuffer:
				case FramebufferStatus.FramebufferIncompleteLayerTargets:
				case FramebufferStatus.FramebufferIncompleteMissingAttachment:
				case FramebufferStatus.FramebufferIncompleteMultisample:
				case FramebufferStatus.FramebufferIncompleteReadBuffer:
				case FramebufferStatus.FramebufferUndefined:
				case FramebufferStatus.FramebufferUnsupported:
					throw new Exception("Frame buffer error.");
			}
		}

		#endregion

		/// <summary>
		/// Gets if the frame buffers are supported.
		/// </summary>
		public bool IsSupported => Gl.MAJOR_VERSION >= 3;
	}
}
