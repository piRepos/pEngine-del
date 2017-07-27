using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Textures;

using pEngine.Core.Data.Files;
using pEngine.Core.Data;

using SharpFont;

namespace pEngine.Core.Graphics.Fonts
{
	using CharsetRange = KeyValuePair<int, int>;

    public class StaticFont : StaticTextureAtlas<int, Glyph>, IFont
    {

		/// <summary>
		/// Makes a new instance of <see cref="StaticFont"/>.
		/// </summary>
		/// <param name="fontFile">Font source file.</param>
		/// <param name="loadSize">Size of glyphs in the font atlas.</param>
		/// <param name="outline">Load outlined glyphs.</param>
		public StaticFont(TrueTypeFont fontFile, int loadSize, bool outline)
			: base()
		{
			Dependencies.Add(fontFile);

			FontFile = fontFile;
			LoadSize = loadSize;
			Outlined = outline;

			Charsets = new List<CharsetRange>
			{
				Charset.ControlCharacters,
				Charset.BasicLatin,
				Charset.Latin1Supplement
			};
		}

		/// <summary>
		/// Makes a new instance of <see cref="StaticFont"/>.
		/// </summary>
		/// <param name="fontFile">Font source file path.</param>
		/// <param name="loadSize">Size of glyphs in the font atlas.</param>
		/// <param name="outline">Load outlined glyphs.</param>
		public StaticFont(string fontPath, int loadSize, bool outline)
			: base()
		{
			TrueTypeFont f = new TrueTypeFont(fontPath);

			Dependencies.Add(f);

			FontFile = f;
			LoadSize = loadSize;
			Outlined = outline;

			Charsets = new List<CharsetRange>
			{
				Charset.ControlCharacters,
				Charset.BasicLatin,
				Charset.Latin1Supplement
			};
		}

		#region Loading informations

		/// <summary>
		/// Size of glyphs in the font atlas.
		/// </summary>
		public int LoadSize { get; }

		/// <summary>
		/// True if this font is loaded with outline.
		/// </summary>
		public bool Outlined { get; }

		/// <summary>
		/// Internal font file.
		/// </summary>
		public TrueTypeFont FontFile { get; }

		/// <summary>
		/// Character ranges.
		/// </summary>
		public List<CharsetRange> Charsets { get; set; }

		#endregion

		#region Informations

		/// <summary>
		/// Gets the maximum ascender of all glyph.
		/// </summary>
		public int Ascender { get; private set; }

		/// <summary>
		/// Gets the maximum descender of all glyph.
		/// </summary>
		public int Descender { get; private set; }

		/// <summary>
		/// Default glyph for not loaded characters.
		/// </summary>
		public Glyph NullGlyph { get; private set; }

		public Glyph this[char c] => this[(int)c];

		#endregion

		#region Loading

		internal override void OnLoad()
		{
			

			// - Prepare freetype library
			using (Library fontLibrary = new Library())
			{
				// - Load font face
				using (Face fontFace = FTHelpers.GetFace(fontLibrary, FontFile, LoadSize))
				{
					// - Get max height
					Ascender = fontFace.Size.Metrics.Ascender.Value >> 6;
					Descender = fontFace.Size.Metrics.Descender.Value >> 6;

					// - Start characters loading
					foreach (CharsetRange range in Charsets)
					{
						for (int i = range.Key; i < range.Value; ++i)
						{
							Loader l = new Loader
							{
								Loaded = false
							};

							l.TextureReference = FTHelpers.GetGlyph(fontLibrary, fontFace, i, Outlined, out l.Data);

							l.Loadable = l.TextureReference.Drawable;

							l.TextureReference.Reference = this;

							textures.Add(i, l);
						}
					}
				}
			}

			base.OnLoad();
		}


		internal override void OnComplete()
		{
			base.OnComplete();
		}


		internal override bool OnAbort(IResource res, Exception e)
		{
			return base.OnAbort(res, e);
		}

		#endregion
	}
}
