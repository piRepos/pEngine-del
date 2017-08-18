using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
    struct FrameBufferShader : IShader
    {

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		public string VertexSource => Properties.Resources.FrameBufferShaderVert;

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		public string GeometrySource => "";

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		public string FragmentSource => Properties.Resources.FrameBufferShaderFrag;

		/// <summary>
		/// This property will store the texture slot
		/// which will be userd to store temporary the texture
		/// during the rendering.
		/// </summary>
		/// <remarks>
		/// The max value is 32.
		/// </remarks>
		public int TextureAttachment { get; set; }

		/// <summary>
		/// Calculate all stati uniform for rendering.
		/// </summary>
		public IEnumerable<IUniform> Uniforms => new IUniform[]
		{
			new TextureUniform("Texture", TextureAttachment)
		};

	}
}
