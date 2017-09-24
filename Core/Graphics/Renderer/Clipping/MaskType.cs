using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.Clipping
{
	public enum MaskType
	{
		/// <summary>
		/// It will be used a shader with alpha multiplication.
		/// </summary>
		ShaderMask = 0,

		/// <summary>
		/// It will be used a stencil buffer (with binary alpha).
		/// </summary>
		StencilMask = 1
	}
}
