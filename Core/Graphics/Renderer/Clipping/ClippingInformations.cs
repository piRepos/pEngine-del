using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.Clipping
{
    public struct ClippingInformations
    {

		/// <summary>
		/// Buffer mask texture.
		/// </summary>
		public long MaskTexture { get; set; }

		/// <summary>
		/// Mask operation.
		/// </summary>
		public MaskOperation Operation { get; set; }

    }
}
