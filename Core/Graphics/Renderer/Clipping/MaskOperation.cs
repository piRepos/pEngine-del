using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.Clipping
{
	public enum MaskOperation
	{
		/// <summary>
		/// This mask will not considered.
		/// </summary>
		None = 0,

		/// <summary>
		/// This mask will be added to the other masks.
		/// </summary>
		Add = 1,

		/// <summary>
		/// This mask will be subtracted from the other masks
		/// </summary>
		Sub = 2
	}
}
