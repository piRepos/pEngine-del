using System;
using System.Collections.Generic;

using pEngine.Common.Math;

using pEngine.Core.Graphics.Renderer.FrameBuffering;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Shading;

using OpenGL;

namespace pEngine.Core.Graphics.Renderer
{
	using Matrix = Common.Math.Matrix;
	using TextureChannel = KeyValuePair<int, long>;

	public struct Asset
	{ 

		/// <summary>
		/// Elements to draw (Verticies and indexes).
		/// </summary>
		public DrawElement[] Elements;

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
		public IShader Shader;

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
