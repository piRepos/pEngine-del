using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Textures;

using pEngine.Core.Data.Files;

namespace pEngine.Core.Graphics.Fonts
{
	using CharsetRange = KeyValuePair<int, int>;

	/// <summary>
	/// Generic font.
	/// </summary>
	public interface IFont : ITexture
	{

		/// <summary>
		/// Size of glyphs in the font atlas.
		/// </summary>
		int LoadSize { get; }

		/// <summary>
		/// True if this font is loaded with outline.
		/// </summary>
		bool Outlined { get; }

		/// <summary>
		/// Internal font file.
		/// </summary>
		TrueTypeFont FontFile { get; }

		/// <summary>
		/// Character ranges.
		/// </summary>
		List<CharsetRange> Charsets { get; set; }

		/// <summary>
		/// Gets the maximum ascender of all glyph.
		/// </summary>
		int Ascender { get; }

		/// <summary>
		/// Gets the maximum descender of all glyph.
		/// </summary>
		int Descender { get; }

		/// <summary>
		/// Default glyph for not loaded characters.
		/// </summary>
		Glyph NullGlyph { get; }

		/// <summary>
		/// Index accessor.
		/// </summary>
		/// <param name="c">Character to get.</param>
		/// <returns>A glyph.</returns>
		Glyph this[char c] { get; }

	}
}
