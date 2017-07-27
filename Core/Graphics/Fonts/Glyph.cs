using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;

using pEngine.Core.Graphics.Textures;

using pEngine.Core.Data.Files;
using pEngine.Core.Data;

using SharpFont;

namespace pEngine.Core.Graphics.Fonts
{
    public class Glyph : DependencyTexture
    {
		/// <summary>
		/// Creates a new instance of <see cref="Glyph"/>.
		/// </summary>
		/// <param name="fontFace">Glyph's font.</param>
		public Glyph()
		{
			Drawable = true;
		}

		/*
		 * Glyph metrics:
		 * --------------
		 *
		 *                       xmin                     xmax
		 *                        |                         |
		 *                        |<-------- width -------->|
		 *                        |                         |
		 *              |         +-------------------------+----------------- ymax
		 *              |         |    ggggggggg   ggggg    |     ^        ^
		 *              |         |   g:::::::::ggg::::g    |     |        |
		 *              |         |  g:::::::::::::::::g    |     |        |
		 *              |         | g::::::ggggg::::::gg    |     |        |
		 *              |         | g:::::g     g:::::g     |     |        |
		 *    offset_x -|-------->| g:::::g     g:::::g     |  offset_y    |
		 *              |         | g:::::g     g:::::g     |     |        |
		 *              |         | g::::::g    g:::::g     |     |        |
		 *              |         | g:::::::ggggg:::::g     |     |        |
		 *              |         |  g::::::::::::::::g     |     |      height
		 *              |         |   gg::::::::::::::g     |     |        |
		 *  baseline ---*---------|---- gggggggg::::::g-----*--------      |
		 *            / |         |             g:::::g     |              |
		 *     origin   |         | gggggg      g:::::g     |              |
		 *              |         | g:::::gg   gg:::::g     |              |
		 *              |         |  g::::::ggg:::::::g     |              |
		 *              |         |   gg:::::::::::::g      |              |
		 *              |         |     ggg::::::ggg        |              |
		 *              |         |         gggggg          |              v
		 *              |         +-------------------------+----------------- ymin
		 *              |                                   |
		 *              |------------- advance_x ---------->|
		 */

		/// <summary>
		/// Glyph index.
		/// </summary>
		public uint Index { get; internal set; }

		/// <summary>
		/// Orizontal X cursor advance.
		/// </summary>
		public double HoriAdvance { get; internal set; }

		/// <summary>
		/// Orizontal X padding.
		/// </summary>
		public double HoriBearingX { get; internal set; }

		/// <summary>
		/// Orizontal Y padding.
		/// </summary>
		public double HoriBearingY { get; internal set; }

		/// <summary>
		/// Vertical X cursor advance.
		/// </summary>
		public double VertAdvance { get; internal set; }

		/// <summary>
		/// Vertical X padding.
		/// </summary>
		public double VertBearingX { get; internal set; }

		/// <summary>
		/// Vertical Y padding.
		/// </summary>
		public double VertBearingY { get; internal set; }

		/// <summary>
		/// Control characters can't be drwan but needs metrics.
		/// </summary>
		public bool Drawable { get; internal set; }

	}
}
