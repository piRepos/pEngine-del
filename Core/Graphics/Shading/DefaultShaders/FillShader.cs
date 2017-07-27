using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{ 
    public struct FillShader : IShader
    {

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		public string VertexSource => Properties.Resources.FillShaderVert;

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		public string GeometrySource => "";

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		public string FragmentSource => Properties.Resources.FillShaderFrag;

		/// <summary>
		/// Calculate all stati uniform for rendering.
		/// </summary>
		public IEnumerable<IUniform> Uniforms => new IUniform[] { };

	}
}
