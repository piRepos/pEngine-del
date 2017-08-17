using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using OpenGL;

using pEngine.Common.Math;
using pEngine.Common.Memory;

using pEngine.Core.Graphics.Renderer.FrameBuffering;

namespace pEngine.Core.Graphics.Renderer.Textures
{
	/// <summary>
	/// Makes a 2D texture
	/// </summary>
	public class GLTexture
	{

		/// <summary>
		/// Makes a new <see cref="GLTexture"/>.
		/// </summary>
		/// <param name="Handler">From another texture.</param>
		public GLTexture(uint handler)
			: base()
		{
			Handler = handler;
		}

		/// <summary>
		/// Makes a new <see cref="GLTexture"/>.
		/// </summary>
		public GLTexture()
			: base()
		{
			Handler = Gl.GenTexture();
		}

		/// <summary>
		/// Texture handler.
		/// </summary>
		public uint Handler { get; }

		#region Binding
		

		/// <summary>
		/// Bind the current texture.
		/// </summary>
		public void Bind(int Attachment = -1)
		{
            if (Attachment >= 0)
			    Gl.ActiveTexture(TextureUnit.Texture0 + Attachment);
            
			Gl.BindTexture(TextureTarget.Texture2d, Handler);

			// Set draw texture filters
			Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)HorizontalWarp);
			Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)VerticalWarp);
			Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)MagnificationFilter);

			// Filter for shrink scale
			if (MipmapLevel > 1)
			{
				Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
			}
			else Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)MinificationFilter);
		}

		/// <summary>
		/// Unbind this texture if is bound.
		/// </summary>
		public void Unbind(int Attachment = -1)
		{
            if (Attachment >= 0)
			    Gl.ActiveTexture(TextureUnit.Texture0 + Attachment);
            
			Gl.BindTexture(TextureTarget.Texture2d, 0);
		}

		#endregion

		#region Filters

		/// <summary>
		/// Shrink filter.
		/// </summary>
		public TextureMinFilter MinificationFilter { get; set; } = TextureMinFilter.Linear;

		/// <summary>
		/// Grow filter.
		/// </summary>
		public TextureMagFilter MagnificationFilter { get; set; } = TextureMagFilter.Linear;

		/// <summary>
		/// Texture border warping.
		/// </summary>
		public TextureWrapMode HorizontalWarp { get; set; } = TextureWrapMode.ClampToEdge;

		/// <summary>
		/// Texture border warping.
		/// </summary>
		public TextureWrapMode VerticalWarp { get; set; } = TextureWrapMode.ClampToEdge;

		#endregion

		#region Pixels

		/// <summary>
		/// Texture size.
		/// </summary>
		public Vector2i Size { get; private set; }

		/// <summary>
		/// Number of mipmaps (default 0).
		/// </summary>
		public uint MipmapLevel { get; set; }

		/// <summary>
		/// Returns RGBA texture pixels.
		/// </summary>
		/// <returns>Texture pixels</returns>
		public PixelBuffer GetPixels()
		{
			PixelBuffer Buff = new PixelBuffer(Size);

			Bind();
			{
				Gl.GetTexImage(TextureTarget.Texture2d, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Buff.Pixels);
			}
			Unbind();

			return Buff;
		}

		/// <summary>
		/// Send this texture to video memory.
		/// </summary>
		public void Upload(params PixelBuffer[] Image)
		{
			if (Image.Length < 1)
				throw new ArgumentException("Empty parameter not allowed.", nameof(Image));

			Size = Image[0].BufferSize;

			// Bind texture
			Bind();
			{
				// OpenGL VRAM upload all mipmaps
				for (int mipmap = 0; mipmap < Image.Length; mipmap++)
				{
					Gl.TexImage2D(
						TextureTarget.Texture2d,
						mipmap,
						InternalFormat.Rgba,
						Image[mipmap].BufferSize.Width,
						Image[mipmap].BufferSize.Height, 0,
						PixelFormat.Rgba,
						PixelType.UnsignedByte,
						Image[mipmap].Pixels
					);

					if (mipmap > 0)
					{
						Gl.TexParameter(TextureTarget.Texture2d, (TextureParameterName)GetTextureParameter.TextureMaxLevel, mipmap - 1);
						Gl.TexParameter(TextureTarget.Texture2d, (TextureParameterName)GetTextureParameter.TextureBaseLevel, mipmap - 1);
					}
				}

				if (MipmapLevel > 1)
				{
					Gl.TexParameter(TextureTarget.Texture2d, (TextureParameterName)GetTextureParameter.TextureMaxLevel, MipmapLevel - 1);
					Gl.TexParameter(TextureTarget.Texture2d, (TextureParameterName)GetTextureParameter.TextureBaseLevel, 0);
				}
			}
			// Unbind texture
			Unbind();
		}

		/// <summary>
		/// Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			// Bind texture
			Bind();
			{
                try
                {
                    Gl.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest);
                }
                catch (Exception) { }

				Gl.GenerateMipmap(TextureTarget.Texture2d);
			}
			// Unbind texture
			Unbind();
		}

		// TODO: Framebuffer in texture class

		
		/// <summary>
		/// Attach this texture to a framebuffer.
		/// </summary>
		/// <param name="FrameBuffer">Framebuffer to attach.</param>
		public void Attach(GLFrameBuffer frameBuffer)
		{
			frameBuffer.Begin(FramebufferBindMode.Buffer);

			Gl.BindTexture(TextureTarget.Texture2d, Handler);

			Size = frameBuffer.Size;

			// Depends On GL_ARB_texture_non_power_of_two
			Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, Size.Width, Size.Height,
				0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

			Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


			Gl.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, Handler, 0);
			
			if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferStatus.FramebufferComplete)
				throw new Exception("Frame buffer is not complete.");

			frameBuffer.End(FramebufferBindMode.Buffer);
		}
		

		#endregion
	}
}
