using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Shading;
using pEngine.Core.Graphics.Fonts;

using pEngine.Common.DataModel;
using pEngine.Common.Math;
using pEngine.Common;
using pEngine.Common.Timing.Base;

namespace pEngine.Core.Graphics.Drawables
{
	public class Character : ManagedSprite
	{
		/// <summary>
		/// Makes a new instance of <see cref="Character"/> class.
		/// </summary>
		/// <param name="glyph">Current character.</param>
		public Character(Glyph glyph)
			: base(glyph)
		{

		}

		/// <summary>
		/// Current glyph.
		/// </summary>
		public Glyph Glyph
		{
			get => Texture as Glyph;
			set => Texture = value;
		}

		/// <summary>
		/// Parent glyph's font.
		/// </summary>
		public IFont Font => Glyph.Reference as IFont;
	}

    public class Paragraph : SpriteBatch<Character>
    {

		/// <summary>
		/// Makes a new instance of <see cref="Paragraph"/> class.
		/// </summary>
		/// <param name="font">Font for this paragraph.</param>
		public Paragraph(IFont font)
			: base(font)
		{
			PropertyChanged += ParagraphPropertyChange;
			Font.PropertyChanged += FontChange;
		}

		[LoaderFunction]
		private void Load(BatchesStore batches)
		{
			
			underline = batches.GetBatch<QuadVertexBatch>(1);

			foreach (Character c in Children)
				if (c.State == LoadState.NotLoaded)
					c.Load(Game);
		}

		/// <summary>
		/// Font to use.
		/// </summary>
		public IFont Font
		{
			get => TextureSource as IFont;
			set => TextureSource = value;
		}

		/// <summary>
		/// Paragraph's text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Text content color.
		/// </summary>
		public Color4 Color { get; set; }

		/// <summary>
		/// Outline options.
		/// </summary>
		public Outline Outline { get; set; }

		/// <summary>
		/// Underline options.
		/// </summary>
		public Underline Underline { get; set; }

		/// <summary>
		/// Additional space between each character.
		/// </summary>
		public double LetterSpacing { get; set; }

		/// <summary>
		/// String visual characters.
		/// </summary>
		public ObservableCollection<Character> Characters => Children;

		#region Characters building

		private string textCache = "";

		public void PreloadString()
		{
			if (redraw == true)
				SetPositions();
		}

		private void FontChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			SetPositions(true);
		}

		private void ParagraphPropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Text":
					redraw = true;
					break;
			}
		}

		private void SetPositions(bool forceUpdate = false)
		{
			int poolPosition = 0;
			int firstDifference = 0;
			double cursorAdvance = 0;
			Glyph glyph;

			string curr, cache;

			try
			{
				if (!Font[' '].Drawable)
				{
					curr = Text.Replace(" ", String.Empty);
					cache = textCache.Replace(" ", String.Empty);
				}
				else
				{
					curr = Text;
					cache = textCache;
				}
			}
			catch (Exception )
			{
				curr = Text.Replace(" ", String.Empty);
				cache = textCache.Replace(" ", String.Empty);
			}

			// - Remove unused characters in the character pool
			if (curr.Length - cache.Length < 0)
			{
				for (int i = 0; i < cache.Length - curr.Length; ++i)
				{
					Children.Remove(Children.Last());
				}
			}

			// - Add missing characters in the pool
			for (int i = 0; i < curr.Length - cache.Length; ++i)
			{
				Children.Add(new Character(Font.NullGlyph));
			}

			// - Take the index of the first different chacater btween the old string and the new
			firstDifference = Text.Zip(textCache, (c1, c2) => c1 == c2).TakeWhile(b => b).Count();

			firstDifference = forceUpdate ? 0 : firstDifference;

			textCache = Text;

			if (firstDifference < Text.Length && Text != string.Empty)
			{
				// - If dynamic atlas loads all needed characters
				if (Font is DynamicFont font)
					font.LoadCharactersAsync(Text);

				foreach (char c in Text)
				{
					// - Skip new lines
					if (c == '\n')
						continue;

					try
					{
						// - Try to load character.
						glyph = Font[c];
					}
					catch (KeyNotFoundException)
					{
						// - If not avaiable set null character.
						glyph = Font.NullGlyph;
					}

					// - If is not a control character.
					if (glyph.Drawable)
					{
						Vector2i position = new Vector2i();

						// - Current character position
						position.Y = (int)(-glyph.HoriBearingY + Font.Ascender);
						position.X = (int)(cursorAdvance + glyph.HoriBearingX);

						Character ca = Children[poolPosition];

						ca.Texture = glyph;
						ca.InternalOffset = position;
						ca.Size = glyph.Size;
						ca.BlendColor = Color;

						if (State == LoadState.Loaded)
							ca.Load(Game);

						poolPosition++;
					}

					cursorAdvance += glyph.HoriAdvance + LetterSpacing;
				}
			}

			if (Size == Vector2i.Zero && Text != string.Empty)
			{
				float w = (Children.Last().InternalOffset.X + Children.Last().Position.X + Children.Last().Size.Width);
				int h = Font.Ascender + (Underline.Enabled ? Underline.Thickness + 3 : 0);
				Size = new Vector2i((int)w, h);
			}

			SetUnderline();
		}

		#endregion

		#region Update

		bool redraw = false;

		public override void Update(IFrameBasedClock clock)
		{
			if (redraw)
			{
				SetPositions();
				redraw = false;
			}

			base.Update(clock);
		}

		#endregion

		#region Underline

		QuadVertexBatch underline;

		private void SetUnderline()
		{
			if (Underline.Enabled && Text != string.Empty)
			{
				float width = (Children.Last().InternalOffset.X + Children.Last().Position.X + Children.Last().Size.Width) 
					- (Children.First().InternalOffset.X + Children.First().Position.X);

				Vector2 pos = new Vector2(Children.First().InternalOffset.X + Children.First().Position.X, Font.Ascender + 3) / Size;
				Vector2 siz = new Vector2(width, Underline.Thickness) / Size;

				underline.Vertexs[0] = new GLVertex
				{
					X = pos.X,
					Y = pos.Y,
					R = Underline.Color.Rf,
					G = Underline.Color.Gf,
					B = Underline.Color.Bf,
					A = Visible ? (float)Opacity * Underline.Color.Af : 0
				};

				underline.Vertexs[1] = new GLVertex
				{
					X = pos.X + siz.Width,
					Y = pos.Y,
					R = Underline.Color.Rf,
					G = Underline.Color.Gf,
					B = Underline.Color.Bf,
					A = Visible ? (float)Opacity * Underline.Color.Af : 0
				};

				underline.Vertexs[2] = new GLVertex
				{
					X = pos.X + siz.Width,
					Y = pos.Y + siz.Height,
					R = Underline.Color.Rf,
					G = Underline.Color.Gf,
					B = Underline.Color.Bf,
					A = Visible ? (float)Opacity * Underline.Color.Af : 0
				};

				underline.Vertexs[3] = new GLVertex
				{
					X = pos.X,
					Y = pos.Y + siz.Height,
					R = Underline.Color.Rf,
					G = Underline.Color.Gf,
					B = Underline.Color.Bf,
					A = Visible ? (float)Opacity * Underline.Color.Af : 0
				};

				underline.Invalidated = true;
				underline.InvalidationType |= BatchInvalidationType.Vertexs;
			}
		}

		#endregion

		#region Assets

		protected override List<Asset> CalculateAssets()
		{
			List<Asset> assets = new List<Asset>();

			assets.Add(new Asset
			{
				Transformation = DrawMatrix,
				Shader = new TextShader
				{
					ContentMaskByte = 2,
					OutlineMaskByte = 1,
					OutlineColor = Outline.Color,
					TextureAttachment = 0
				},
				Textures = new KeyValuePair<int, long>[]
				{
					new KeyValuePair<int, long>(0, TextureSource.DependencyID)
				},
				Elements = new DrawElement[]
				{
					new DrawElement
					{
						ElementOffset = (int)Vertexs.Indexes.Offset,
						ElementSize = (int)Vertexs.Indexes.Size,
						Primitive = Vertexs.Primitive,
					}
				},
				AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
				AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
				ColorBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
				ColorBlendingSrc = OpenGL.BlendingFactor.One,
				TargetID = -1
			});

			if (Underline.Enabled)
			{
				assets.Add(new Asset
				{
					Transformation = DrawMatrix,
					Shader = new FillShader(),
					Elements = new DrawElement[]
					{
					new DrawElement
					{
						ElementOffset = (int)underline.Indexes.Offset,
						ElementSize = (int)underline.Indexes.Size,
						Primitive = underline.Primitive,
					}
					},
					AlphaBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
					AlphaBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
					ColorBlendingDst = OpenGL.BlendingFactor.OneMinusSrcAlpha,
					ColorBlendingSrc = OpenGL.BlendingFactor.SrcAlpha,
					TargetID = -1
				});
			}

			return assets;
		}

		#endregion
	}

	public struct Outline
	{
		/// <summary>
		/// Outline color.
		/// </summary>
		public Color4 Color { get; set; }

		/// <summary>
		/// If true outline is enabled.
		/// </summary>
		public bool Enabled { get; set; }
	}

	public struct Underline
	{
		/// <summary>
		/// Underline color.
		/// </summary>
		public Color4 Color { get; set; }

		/// <summary>
		/// Underline width.
		/// </summary>
		public int Thickness { get; set; }

		/// <summary>
		/// If true will show this underline.
		/// </summary>
		public bool Enabled { get; set; }
	}
}
