using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using pEngine.Common.Math;
using pEngine.Common.Memory;

using pEngine.Core.Data.Files;

using SharpFont;
using SharpFont.Internal;

namespace pEngine.Core.Graphics.Fonts
{
	using Spans = List<Span>;

	public class FTHelpers
	{

		static public Face GetFace(Library lib, TrueTypeFont font, int size)
		{
			Face fontFace = new Face(lib, font.Content, 0);

			// - Set unicode and font size
			fontFace.SelectCharmap(SharpFont.Encoding.Unicode);

			// - Set load size
			fontFace.SetCharSize(0, size, 72, 72);

			return fontFace;
		}

		static private void RasterCallback(int y, int count, NativeReference<Span> spans, IntPtr user)
		{

		}



		static public Glyph GetGlyph(Library lib, Face fontFace, int index, bool outlined, out PixelBuffer pixels)
		{
			Glyph currentGlyph = new Glyph();

			GlyphSlot currentSlot = null;
			FTBitmap glyphBitmap = null;
			GlyphSlot outlineSlot = null;
			FTBitmap outlinedBitmap = null;
			BBox outlineBbox = new BBox();
			BBox centerBbox = new BBox();

			LoadFlags Flags = new LoadFlags();

			Flags |= LoadFlags.Render;
			Flags |= LoadFlags.ForceAutohint;
			Flags |= LoadFlags.Color;

			uint GlyphIndex = index < 0 ? 0 : fontFace.GetCharIndex((uint)index);

			if (GlyphIndex == 0 && index > 0)
			{
				pixels = new PixelBuffer();
				return null;
			}

			try
			{
				// - Normal glyph rendering
				fontFace.LoadGlyph(GlyphIndex, Flags, LoadTarget.Normal);

				SharpFont.Glyph g = fontFace.Glyph.GetGlyph();

				currentSlot = fontFace.Glyph;
				centerBbox = g.GetCBox(GlyphBBoxMode.Pixels);
				glyphBitmap = g.ToBitmapGlyph().Bitmap;

				// - Outlined glyph rendering
				if (outlined)
				{
					fontFace.LoadGlyph(GlyphIndex, LoadFlags.NoBitmap | LoadFlags.ForceAutohint, LoadTarget.Normal);

					using (var Stroker = new Stroker(lib))
					{

						Stroker.Set(72 * 3, StrokerLineCap.Round, StrokerLineJoin.Round, 0);

						g = fontFace.Glyph.GetGlyph();


						g = g.StrokeBorder(Stroker, true, false);

						g.ToBitmap(RenderMode.Normal, new FTVector26Dot6(0, 0), true);

						// - Get character bitmap and metrics
						outlineSlot = fontFace.Glyph;
						outlineBbox = g.GetCBox(GlyphBBoxMode.Pixels);
						outlinedBitmap = g.ToBitmapGlyph().Bitmap;
					}
				}

			}
			catch (Exception )
			{
				pixels = new PixelBuffer();
				return null;
			}


			// Character metrics
			if (outlined)
				currentGlyph.Size = new Vector2i(outlinedBitmap.Width, outlinedBitmap.Rows);
			else
				currentGlyph.Size = new Vector2i(glyphBitmap.Width, glyphBitmap.Rows);

			currentGlyph.HoriAdvance = outlined ? outlineSlot.Advance.X : currentSlot.Advance.X;
			currentGlyph.VertAdvance = outlined ? outlineSlot.Advance.Y : currentSlot.Advance.Y;
			currentGlyph.VertBearingX = (outlined ? outlineSlot.Metrics.VerticalBearingX.ToInt32() : currentSlot.Metrics.VerticalBearingX.ToInt32());
			currentGlyph.VertBearingY = (outlined ? outlineSlot.Metrics.VerticalBearingY.ToInt32() : currentSlot.Metrics.VerticalBearingY.ToInt32());
			currentGlyph.HoriBearingX = (outlined ? outlineSlot.Metrics.HorizontalBearingX.ToInt32() : currentSlot.Metrics.HorizontalBearingX.ToInt32());
			currentGlyph.HoriBearingY = (outlined ? outlineSlot.Metrics.HorizontalBearingY.ToInt32() : currentSlot.Metrics.HorizontalBearingY.ToInt32());

			pixels = new PixelBuffer(currentGlyph.Size);

			if (glyphBitmap.Buffer == IntPtr.Zero)
			{
				currentGlyph.Drawable = false;
				return currentGlyph;
			}

			Vector2i offset = Vector2i.Zero;

			if (outlined)
			{
				float centerX = ((float)(outlineBbox.Right - outlineBbox.Left) - (centerBbox.Right - centerBbox.Left)) / 2;
				float centerY = ((float)(outlineBbox.Top - outlineBbox.Bottom) - (centerBbox.Top - centerBbox.Bottom)) / 2;

				int centerXi = (int)Math.Round(centerX);
				int centerYi = (int)Math.Round(centerY);

				offset = new Vector2i(centerXi, centerYi);
			}
			
			byte[] pixel = glyphBitmap.BufferData;
			byte[] outlinePixel = null;

			if (outlined)
				outlinePixel = outlinedBitmap.BufferData;

			for (int r = 0; r < currentGlyph.Size.Y; ++r)
			{
				for (int c = 0; c < currentGlyph.Size.X; ++c)
				{
					if (outlined)
					{
						byte val = outlinePixel[c + ((outlinedBitmap.Rows - 1 - r) * outlinedBitmap.Width)];
						pixels.Pixels[(c * 4) + (r * currentGlyph.Size.Width * 4) + 0] = val;
						pixels.Pixels[(c * 4) + (r * currentGlyph.Size.Width * 4) + 3] = val;
					}

					if ((c >= offset.X && r >= offset.Y && c < glyphBitmap.Width + offset.X && r < glyphBitmap.Rows + offset.Y) || !outlined)
					{
						byte val = 0;

						if (glyphBitmap.PixelMode == PixelMode.Mono)
						{
							val = (byte)(glyphBit(currentSlot, (c - offset.X), ((glyphBitmap.Rows - 1) - (r - offset.Y))) ? 255 : 0);
						}
						else
						{
							val = pixel[(c - offset.X) + (((glyphBitmap.Rows - 1) - (r - offset.Y)) * glyphBitmap.Width)];
						}

						pixels.Pixels[(c * 4) + (r * currentGlyph.Size.Width * 4) + 1] = val;

						if (!outlined)
							pixels.Pixels[(c * 4) + (r * currentGlyph.Size.Width * 4) + 3] = val;

					}
				}

			}

			return currentGlyph;
		}


		private static bool glyphBit(GlyphSlot glyph, int x, int y)
		{
			int pitch = Math.Abs(glyph.Bitmap.Pitch);

			byte cValue;

			unsafe
			{
				byte* buffer = (byte*)glyph.Bitmap.Buffer;
				byte *row = &buffer[pitch * y];
				cValue = row[x >> 3];
			}

			return (cValue & (128 >> (x & 7))) != 0;
		}
	}
}
