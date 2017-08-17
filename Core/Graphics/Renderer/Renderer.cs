using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

using pEngine.Common.Math;

using pEngine.Core.Graphics.Renderer.Shading;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer.Textures;
using pEngine.Core.Graphics.Renderer.FrameBuffering;

namespace pEngine.Core.Graphics.Renderer
{
	using TextureChannel = Int64;

	public class Renderer
    {
		private pEngine gameHost;

		/// <summary>
		/// Creates a new instance of <see cref="Renderer"/>.
		/// </summary>
		public Renderer(pEngine host)
		{
			gameHost = host;
			currentShader = null;

			Vertexs = new GLVertexMemory();
			Textures = new GLTextureMemory();
			Buffers = new FrameBufferBatch(this);
		}

		/// <summary>
		/// Initialize all OpenGL related resources.
		/// </summary>
		public void Initialize()
		{
			Vertexs.Initialize();
			Textures.Initialize();

			currentTextures = new TextureChannel[32];
		}

		#region Modules

		/// <summary>
		/// OpenGL vertex memory.
		/// </summary>
		public GLVertexMemory Vertexs { get; }

		/// <summary>
		/// OpenGL texture memory.
		/// </summary>
		public GLTextureMemory Textures { get; }

		/// <summary>
		/// 
		/// </summary>
		public FrameBufferBatch Buffers { get; }

		#endregion

		#region State

		/// <summary>
		/// Current binded shader.
		/// </summary>
		GLShader currentShader;

		/// <summary>
		/// 
		/// </summary>
		GLFrameBuffer currentFrameBuffer;

		/// <summary>
		/// Current binded texture.
		/// </summary>
		TextureChannel[] currentTextures;

		#endregion

		#region Rendering

		/// <summary>
		/// Takes all frame assets and draw it
		/// on the backbuffer.
		/// </summary>
		/// <param name="assets">Assets to draw.</param>
		public void DrawFrame(IEnumerable<Asset> assets)
		{
			for (int i = 0; i < currentTextures.Length; i++)
				currentTextures[i] = -1;

			currentShader = null;

            foreach (Asset a in assets)
            {
				// - Setting frame buffer
				if (a.TargetID < 0)
				{
					if (currentFrameBuffer != null)
						currentFrameBuffer.End(FramebufferBindMode.Buffer);

					currentFrameBuffer = null;
				}
				else
				{
					GLFrameBuffer currBuffer = Buffers.GetBuffer(a.TargetID);
					if (currBuffer != currentFrameBuffer)
					{
						if (currentFrameBuffer != null)
							currentFrameBuffer.End(FramebufferBindMode.Buffer);

						currentFrameBuffer = currBuffer;

						currentFrameBuffer.Begin(FramebufferBindMode.Buffer);
						currentFrameBuffer.Clear();

					}
				}

				// - Gets render shader
				GLShader s = gameHost.Shaders.GetGLShader(a.Shader.GetType());

                // - Bind render shader if not binded
                if (currentShader != s)
                {
                    (currentShader = s).Bind();

                    // - Bind vertex memory with shader pointers
                    Vertexs.Bind(s.VertexAttrPointer, s.TexCoordAttrPointer, s.ColorAttrPointer);
                }

                // - Apply shader uniforms
                foreach (IUniform uniform in a.Shader.Uniforms)
                    uniform.Apply(s.Program);

                // - Set asset model view
                s.SetModelView(a.Transformation);

                // - Texture binding
                if (a.Textures != null)
                {
                    foreach (var tex in a.Textures)
                    {
                        if (currentTextures[tex.Key] == tex.Value)
                            continue;

                        currentTextures[tex.Key] = tex.Value;
                        GLTexture tx = Textures.ResolveTexture(tex.Value);

                        tx.Bind(tex.Key);
                    }
                }

                // - Set blending function ---------------------------------------------------------------------------------------------
                Gl.BlendFuncSeparate(a.ColorBlendingSrc, a.ColorBlendingDst, a.AlphaBlendingSrc, a.AlphaBlendingDst);
                // ---------------------------------------------------------------------------------------------------------------------

                foreach (DrawElement e in a.Elements)
                {
                    // - Draw asset ----------------------------------------------------------------------------------------------------
                    Gl.DrawElements(e.Primitive, e.ElementSize, DrawElementsType.UnsignedInt, (IntPtr)(e.ElementOffset * sizeof(uint)));
                    // -----------------------------------------------------------------------------------------------------------------
                }
            }

			Vertexs.Unbind();

			if (currentShader != null)
				currentShader.Unbind();

			if (currentFrameBuffer != null)
				currentFrameBuffer.End(FramebufferBindMode.Buffer);
		}

		#endregion
	}
}
