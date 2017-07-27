using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
    public interface IShader
    {

		/// <summary>
		/// Vertex sharder source code.
		/// </summary>
		string VertexSource { get; }

		/// <summary>
		/// Geometry sharder source code.
		/// </summary>
		string GeometrySource { get; }

		/// <summary>
		/// Fragment sharder source code.
		/// </summary>
		string FragmentSource { get; }

		/// <summary>
		/// Calculate all stati uniform for rendering.
		/// </summary>
		IEnumerable<IUniform> Uniforms { get; }

    }
}
