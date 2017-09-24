using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
	public struct MaskAttachment
	{
		/// <summary>
		/// 
		/// </summary>
		public MaskOperation Operation { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int TextureAttachment { get; set; }
	}

	public struct MaskShader : IShader
	{

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		public string VertexSource => Properties.Resources.MaskShaderVert;

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		public string GeometrySource => "";

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		public string FragmentSource => Properties.Resources.MaskShaderFrag;

		/// <summary>
		/// This property will store the texture slot
		/// which will be used to store temporary the texture
		/// during the rendering.
		/// </summary>
		/// <remarks>
		/// The max value is 32.
		/// </remarks>
		public int SourceTextureAttachment { get; set; }

		/// <summary>
		/// This property will store a list of texture slot
		/// which will be used to clip the mask on the source texture.
		/// </summary>
		/// <remarks>
		/// The max value is 32 and the max number of elements is 31.
		/// </remarks>
		public IEnumerable<MaskAttachment> MaskTextureAttachments { get; set; }

		private int[] AllTextures => (new int[] { SourceTextureAttachment }).Union(MaskTextureAttachments.Select(x => x.TextureAttachment)).ToArray();
		private int[] AllOperations => (MaskTextureAttachments.Select(x => (int)x.Operation)).ToArray();

		/// <summary>
		/// Calculate all stati uniform for rendering.
		/// </summary>
		public IEnumerable<IUniform> Uniforms => new IUniform[]
		{
			new TextureUniforms("Textures", AllTextures),
			new Int1Uniform("TextureCount", AllTextures.Length - 1),
			new IntUniforms("Operations", AllOperations)
		};

	}
}
