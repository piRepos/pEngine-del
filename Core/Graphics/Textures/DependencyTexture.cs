using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.DataModel;
using pEngine.Common.Math;

using pEngine.Core.Data;

namespace pEngine.Core.Graphics.Textures
{
    public class DependencyTexture : Resource, ITexture, IMovable
    {
		/// <summary>
		/// Makes a new <see cref="DependencyTexture"/>.
		/// </summary>
		/// <param name="parent">Parent atlas.</param>
		/// <param name="size">Texture size.</param>
		public DependencyTexture(StandaloneTexture parent, Vector2i size)
		{
			Size = size;
			Reference = parent;
			IsPartial = true;
		}

		/// <summary>
		/// Makes a new <see cref="DependencyTexture"/>.
		/// </summary>
		public DependencyTexture()
		{
			IsPartial = true;
		}

		/// <summary>
		/// Parent texture.
		/// </summary>
		public StandaloneTexture Reference { get; internal set; }

		/// <summary>
		/// Texture relative size (value from 0 to 1).
		/// </summary>
		public Vector2 RasterSize { get; internal set; }

		/// <summary>
		/// Texture relative position (value from 0 to 1).
		/// </summary>
		public Vector2 RasterPosition { get; internal set; }

		/// <summary>
		/// Real texture size in pixels.
		/// </summary>
		public Vector2i Size { get; internal set; }

		/// <summary>
		/// Real texture position if ther's a parent.
		/// </summary>
		public Vector2i Position { get; set; }

		/// <summary>
		/// Top left corner texture position.
		/// </summary>
		public Vector2 TopLeft => RasterPosition + new Vector2(0, RasterSize.Height);

		/// <summary>
		/// Top right corner texture position.
		/// </summary>
		public Vector2 TopRight => RasterPosition + new Vector2(RasterSize.Width, RasterSize.Height);

		/// <summary>
		/// Bottom right corner texture position.
		/// </summary>
		public Vector2 BotRight => RasterPosition + new Vector2(RasterSize.Width, 0);

		/// <summary>
		/// Bottom left corner texture position.
		/// </summary>
		public Vector2 BotLeft => RasterPosition + new Vector2(0, 0);

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public virtual long DependencyID => Reference.DependencyID;

		/// <summary>
		/// Used space in VRAM.
		/// </summary>
		public override uint UsedSpace => (uint)(Size.Width * Size.Height * 4 * sizeof(byte));

		internal override void OnLoad()
		{
			return;
		}
	}
}
