using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
    public struct SpriteShader : IShader
	{

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		public string VertexSource => Properties.Resources.SpriteShaderVert;

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		public string GeometrySource => "";

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		public string FragmentSource => Properties.Resources.SpriteShaderFrag;

		/// <summary>
		/// Texture opacity.
		/// </summary>
		/// <remarks>
		/// The value's range goes from 0 to 1;
		/// default value is 1.
		/// </remarks>
		public float Opacity { get; set; }

		/// <summary>
		/// Texture image brightness.
		/// </summary>
		/// <remarks>
		/// The value's range goes from -1 to 1;
		/// default value is 0.
		/// </remarks>
		public float Brightness { get; set; }

		/// <summary>
		/// Texture contrast.
		/// </summary>
		/// <remarks>
		/// The value's range goes from 0 to infinity;
		/// default value is 1.
		/// </remarks>
		public float Contrast { get; set; }

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
			new Float1Uniform("Opacity", Opacity),
			new Float1Uniform("Brightness", Brightness),
			new Float1Uniform("Contrast", Contrast),
			new TextureUniform("Texture", TextureAttachment)
		};

	}
}
