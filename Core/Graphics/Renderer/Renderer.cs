using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

using pEngine.Common.Math;
using pEngine.Common;

using pEngine.Core.Graphics.Shading;

using pEngine.Core.Graphics.Renderer.Shading;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Renderer.Textures;
using pEngine.Core.Graphics.Renderer.FrameBuffering;

namespace pEngine.Core.Graphics.Renderer
{
	using TextureChannel = Int64;

    public class Renderer
    {
        private pEngine gameHost;
        private QuadVertexBatch screenQuad;

        /// <summary>
        /// Creates a new instance of <see cref="Renderer"/>.
        /// </summary>
        public Renderer(pEngine host)
        {
            gameHost = host;
            currentShader = null;

            Vertexs = new GLVertexMemory();
            Textures = new GLTextureMemory();
            Buffers = new FrameBufferMemory(this);
        }

        /// <summary>
        /// Initialize all OpenGL related resources.
        /// </summary>
        public void Initialize()
        {
            Vertexs.Initialize();
            Textures.Initialize();

            currentTextures = new TextureChannel[32];

            screenQuad = gameHost.Batches.DefaultQuad;
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
        public FrameBufferMemory Buffers { get; }

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

            ScissorSetting(null, true);

            foreach (Asset a in assets)
            {
                DrawAsset(a);
            }

            Vertexs.Unbind();

            if (currentShader != null)
                currentShader.Unbind();

            if (currentFrameBuffer != null)
                currentFrameBuffer.End();
        }

        /// <summary>
        /// Draws a single asset.
        /// </summary>
        /// <param name="a">Asset to draw.</param>
        public void DrawAsset(Asset a)
        {
            FrameBufferSetting(a.TargetID);

            ShaderSetting(a.Shader, a.Transformation);

            TexturesSetting(a.Textures);

            ScissorSetting(a.ScissorArea);

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

        #endregion

        #region Frame buffering

        /// <summary>
        /// Current binded framebuffer.
        /// </summary>
        GLFrameBuffer currentFrameBuffer;

        /// <summary>
        /// Frame buffer state and binding setting.
        /// </summary>
        /// <param name="targetID">Frame buffer id.</param>
        public void FrameBufferSetting(long targetID)
        {
            // - Setting frame buffer
            if (targetID < 0)
            {
                if (currentFrameBuffer != null)
                    currentFrameBuffer.End();

                currentFrameBuffer = null;
            }
            else
            {
                GLFrameBuffer currBuffer = Buffers.GetBuffer(targetID);
                if (currBuffer != currentFrameBuffer)
                {
                    if (currentFrameBuffer != null)
                        currentFrameBuffer.End();

                    currentFrameBuffer = currBuffer;

                    currentFrameBuffer.Begin();
                    currentFrameBuffer.Clear();

                }
            }
        }

        #endregion

        #region Texture binding

        /// <summary>
        /// Current binded texture.
        /// </summary>
        TextureChannel[] currentTextures;

        /// <summary>
        /// Bind all specified textures.
        /// </summary>
        /// <param name="textures">An array of slot - texture id.</param>
        public void TexturesSetting(KeyValuePair<int, long>[] textures)
        {
            // - Texture binding
            if (textures != null)
            {
                foreach (var tex in textures)
                {
                    if (currentTextures[tex.Key] == tex.Value)
                        continue;

                    currentTextures[tex.Key] = tex.Value;
                    GLTexture tx = Textures.ResolveTexture(tex.Value);

                    tx.Bind(tex.Key);
                }
            }
        }

        #endregion

        #region Scissor clipping

        /// <summary>
        /// Cache for the current clipping area.
        /// </summary>
        Rect? currentScissor;

        /// <summary>
        /// Sets a scissor clipping region, if null scissor is the framebuffer.
        /// </summary>
        /// <param name="area">Region to clip.</param>
        public void ScissorSetting(Rect? area, bool force = false)
        {
            int x, y, w, h;

            if (!force && currentScissor == area)
                return;

            currentScissor = area;

            x = 0;
            y = 0;
            w = gameHost.Window.BufferSize.Width;
            h = gameHost.Window.BufferSize.Height;

            if (area != null)
            {
                Rect a = area.Value;
                y = gameHost.Window.BufferSize.Height - (a.Position.Y + a.Size.Height);
                x = a.Position.X;
                w = a.Size.Width;
                h = a.Size.Height;
            }

            Gl.Scissor(x, y, w, h);
        }

        #endregion

        #region Shading

        /// <summary>
        /// Current binded shader.
        /// </summary>
        GLShader currentShader;

        /// <summary>
        /// Sets a shader, and initialize it with the state uniforms.
        /// </summary>
        /// <param name="shader">Shader state to use.</param>
        /// <param name="transformation">Object transformation</param>
        public void ShaderSetting(IShader shader, Common.Math.Matrix transformation)
        {
            // - Gets render shader
            GLShader s = gameHost.Shaders.GetGLShader(shader.GetType());

            // - Bind render shader if not binded
            if (currentShader != s)
            {
                (currentShader = s).Bind();

                // - Bind vertex memory with shader pointers
                Vertexs.Bind(s.VertexAttrPointer, s.TexCoordAttrPointer, s.ColorAttrPointer);
            }

            // - Apply shader uniforms
            foreach (IUniform uniform in shader.Uniforms)
                uniform.Apply(s.Program);

            // - Set asset model view
            s.SetModelView(transformation);
        }

        #endregion

        #region Stencil clipping

        public void StencilClippingSetting()
        {
            
        }

        #endregion
    }
}
