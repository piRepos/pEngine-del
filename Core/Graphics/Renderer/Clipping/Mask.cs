using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Containers;

namespace pEngine.Core.Graphics.Renderer.Clipping
{
    public struct Mask
    {
		/// <summary>
		/// Mask node to render.
		/// </summary>
		public LayerMask MaskNode { get; set; }

		/// <summary>
		/// Mask operation.
		/// </summary>
		public MaskOperation Operation { get; set; }

		/// <summary>
		/// If false this mask will not be considered.
		/// </summary>
		public bool Enabled { get; set; }
    }
}
