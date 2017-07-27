using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using SharpFont;

using pEngine.Core.Graphics.Textures;

using pEngine.Core.Data.Files;

namespace pEngine.Core.Graphics.Fonts
{
	using CharsetRange = KeyValuePair<int, int>;

	public class DynamicFont : DynamicTextureAtlas<int, Glyph>, IFont
    {

		/// <summary>
		/// Makes a new instance of <see cref="DynamicFont"/>.
		/// </summary>
		/// <param name="fontFile">Font source file.</param>
		/// <param name="loadSize">Size of glyphs in the font atlas.</param>
		/// <param name="outline">Load outlined glyphs.</param>
		public DynamicFont(TrueTypeFont fontFile, int loadSize, bool outline)
			: base()
		{
			Dependencies.Add(fontFile);

			FontFile = fontFile;
			LoadSize = loadSize;
			Outlined = outline;

			Charsets = new List<CharsetRange>
			{
				Charset.BasicLatin,
				Charset.Latin1Supplement
			};
		}

		/// <summary>
		/// Makes a new instance of <see cref="DynamicFont"/>.
		/// </summary>
		/// <param name="fontFile">Font source file path.</param>
		/// <param name="loadSize">Size of glyphs in the font atlas.</param>
		/// <param name="outline">Load outlined glyphs.</param>
		public DynamicFont(string fontPath, int loadSize, bool outline)
			: base()
		{
			TrueTypeFont f = new TrueTypeFont(fontPath);

			Dependencies.Add(f);

			FontFile = f;
			LoadSize = loadSize;
			Outlined = outline;

			Charsets = new List<CharsetRange>
			{
				Charset.BasicLatin,
				Charset.Latin1Supplement
			};

			Mipmapping = false;
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

		#region Character async loading

		/// <summary>
		/// Load a character to the font atlas.
		/// </summary>
		/// <param name="c"></param>
		public void LoadCharacter(char c)
		{
			Charsets.Add(new CharsetRange(c, c + 1));

			Refresh();
		}

		/// <summary>
		/// Load a character to the font atlas.
		/// </summary>
		/// <param name="c"></param>
		public void LoadCharacterAsync(char c)
		{
			Charsets.Add(new CharsetRange(c, (int)c + 1));

			Refresh(true);
		}

		/// <summary>
		/// Loads all characters not already loaded from a string.
		/// </summary>
		/// <param name="c">String with characters to load.</param>
		public void LoadCharacters(string c)
		{
			bool found = false;

			foreach (char curr in c)
			{
				if (!Charsets.Any((x) => x.Key <= (int)curr && x.Value >= (int)curr + 1))
				{
					found = true;
					Charsets.Add(new CharsetRange(curr, (int)curr + 1));
				}
			}

			if (found)
				Refresh();
		}

		/// <summary>
		/// Loads all characters not already loaded from a string asynchronously.
		/// </summary>
		/// <param name="c">String with characters to load.</param>
		public void LoadCharactersAsync(string c)
		{
			bool found = false;

			foreach (char curr in c)
			{
				if (!Charsets.Any((x) => x.Key <= (int)curr && x.Value >= (int)curr))
				{
					found = true;
					Charsets.Add(new CharsetRange(curr, (int)curr + 1));
				}
			}

			if (found)
				Refresh(true);
		}


		#endregion

		#region Loading

		internal override void OnLoad()
		{
			if (!Charsets.Contains(new CharsetRange(-1, 0)))
				Charsets.Add(new CharsetRange(-1, 0));

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

							textures.TryAdd(i, l);
						}
					}
				}

				NullGlyph = this[-1];
			}

			base.OnLoad();
		}

		internal override bool OnAbort(Data.IResource res, Exception e)
		{
			return base.OnAbort(res, e);
		}

		internal override void OnComplete()
		{
			base.OnComplete();
		}

		#endregion

	}
}
