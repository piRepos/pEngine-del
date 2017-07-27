using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
	public struct TextShader : IShader
	{

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		public string VertexSource => Properties.Resources.TextShaderVert;

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		public string GeometrySource => "";

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		public string FragmentSource => Properties.Resources.TextShaderFrag;

		/// <summary>
		/// Outline color property.
		/// </summary>
		/// <remarks>
		/// Default value is black.
		/// </remarks>
		public Color4 OutlineColor { get; set; }

		/// <summary>
		/// Index of the byte used for masking the outline glyph shape.
		/// </summary>
		/// <remarks>
		/// Default value is 0 (RED).
		/// </remarks>
		public int OutlineMaskByte { get; set; }

		/// <summary>
		/// Index of the byte used for masking the content glyph shape.
		/// </summary>
		/// <remarks>
		/// Default value is 1 (GREEN).
		/// </remarks>
		public float ContentMaskByte { get; set; }

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
			new Float4Uniform("OutlineColor", OutlineColor),
			new Float1Uniform("OutlineMaskBit", OutlineMaskByte),
			new Float1Uniform("ContentMaskBit", ContentMaskByte),
			new TextureUniform("Texture", TextureAttachment)
		};

	}
}
