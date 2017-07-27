using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer.Batches
{
    public struct DrawElement
    {
		/// <summary>
		/// Offset in the index array for drawing.
		/// </summary>
		public int ElementOffset;

		/// <summary>
		/// Number of indicies.
		/// </summary>
		public int ElementSize;

		/// <summary>
		/// Drawing primitive.
		/// </summary>
		public PrimitiveType Primitive;
	}
}
