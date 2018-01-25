using System;
using System.Collections.Generic;

using pEngine.Utils.Math;

using OpenGL;

namespace pEngine.Graphics.Renderer
{
	using Matrix = Utils.Math.Matrix;
	using TextureChannel = KeyValuePair<int, long>;

	public struct Asset
	{ 

		/// <summary>
		/// Elements to draw (Verticies and indexes).
		/// </summary>
		//public DrawElement[] Elements;

		/// <summary>
		/// Binded textures for each channel.
		/// </summary>
		public TextureChannel[] Textures;

		/// <summary>
		/// Asset transformation.
		/// </summary>
		public Matrix Transformation;

		/// <summary>
		/// Frame buffer target id.
		/// </summary>
		public long TargetID;

		/// <summary>
		/// Draw shader parameters.
		/// </summary>
		//public IShader Shader;

		#region Clipping informations

		/// <summary>
		/// Scissor clipping area, null if no scissor clipping.
		/// </summary>
		public Rect? ScissorArea;

		/// <summary>
		/// Target stencil mask layer.
		/// </summary>
		public uint? StencilLayer;

        /// <summary>
        /// The stencil masks.
        /// </summary>
        //public (uint, MaskOperation)[] StencilTargets;

		#endregion

		#region Blending function

		/// <summary>
		/// Asset alpha blending search factor.
		/// </summary>
		public BlendingFactor AlphaBlendingSrc;

		/// <summary>
		/// Asset alpha blending destination factor.
		/// </summary>
		public BlendingFactor AlphaBlendingDst;

		/// <summary>
		/// Asset color blending search factor.
		/// </summary>
		public BlendingFactor ColorBlendingSrc;

		/// <summary>
		/// Asset color blending destination factor.
		/// </summary>
		public BlendingFactor ColorBlendingDst;

		#endregion

	}
}
