// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace pEngine
{
	/// <summary>
	/// This class manage RGBA format colors
	/// which can be set/get with byte value or
	/// float value.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Color4 : IEquatable<Color4>
	{
		float r, g, b, a;

		/// <summary>
		/// Initializes a new instance of the <see cref="pEngine.Common.Color"/> class
		/// with a specified color.
		/// </summary>
		/// <param name="R">Red (0 - 255)</param>
		/// <param name="G">Green (0 - 255)</param>
		/// <param name="B">Blue (0 - 255)</param>
		/// <param name="A">Alpha (0 - 255)</param>
		public Color4(byte R, byte G, byte B, byte A)
		{
			r = R / 255.0F;
			g = G / 255.0F;
			b = B / 255.0F;
			a = A / 255.0F;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="pEngine.Common.Color"/> class
		/// with a specified color.
		/// </summary>
		/// <param name="R">Red (0 - 1)</param>
		/// <param name="G">Green (0 - 1)</param>
		/// <param name="B">Blue (0 - 1)</param>
		/// <param name="A">Alpha (0 - 1)</param>
		public Color4(float R, float G, float B, float A)
		{
			r = R;
			g = G;
			b = B;
			a = A;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="pEngine.Common.Color"/> class
		/// with a specified hex color.
		/// </summary>
		/// <param name="hex">Hexadecimal color.</param>
		public Color4(string hex)
		{
			Color4 c = HexToColor(hex);
			r = c.r;
			g = c.g;
			b = c.b;
			a = c.a;
		}

		#region Access

		/// <summary>
		/// Gets or sets the Red color
		/// with range (0 - 255)
		/// </summary>
		/// <value>Red.</value>
		public byte Rb { get { return (byte)(r * 255); } set { r = (float)value / 255.0F; } }

		/// <summary>
		/// Gets or sets the Green color
		/// with range (0 - 255)
		/// </summary>
		/// <value>Green.</value>
		public byte Gb { get { return (byte)(g * 255); } set { g = (float)value / 255.0F; } }

		/// <summary>
		/// Gets or sets the Blue color
		/// with range (0 - 255)
		/// </summary>
		/// <value>Blue.</value>
		public byte Bb { get { return (byte)(b * 255); } set { b = (float)value / 255.0F; } }

		/// <summary>
		/// Gets or sets the Alpha color
		/// with range (0 - 255)
		/// </summary>
		/// <value>Alpha.</value>
		public byte Ab { get { return (byte)(a * 255); } set { a = (float)value / 255.0F; } }


		/// <summary>
		/// Gets or sets the Red color
		/// with range (0 - 1)
		/// </summary>
		/// <value>Red.</value>
		public float Rf { get { return r; } set { r = value; } }

		/// <summary>
		/// Gets or sets the Green color
		/// with range (0 - 1)
		/// </summary>
		/// <value>Green.</value>
		public float Gf { get { return g; } set { g = value; } }

		/// <summary>
		/// Gets or sets the Blue color
		/// with range (0 - 1)
		/// </summary>
		/// <value>Blue.</value>
		public float Bf { get { return b; } set { b = value; } }

		/// <summary>
		/// Gets or sets the Alpha color
		/// with range (0 - 1)
		/// </summary>
		/// <value>Alpha.</value>
		public float Af { get { return a; } set { a = value; } }

		#endregion

		#region Hex conversion

		/// <summary>
		/// Convert this color to an hex string with format #AARRGGBB
		/// </summary>
		/// <returns>Hex string.</returns>
		public string ColorToHex()
		{
			return String.Format("#{0}{1}{2}{3}"
				, Ab.ToString("X").Length == 1 ? String.Format("0{0}", Ab.ToString("X")) : Ab.ToString("X")
				, Rb.ToString("X").Length == 1 ? String.Format("0{0}", Rb.ToString("X")) : Rb.ToString("X")
				, Gb.ToString("X").Length == 1 ? String.Format("0{0}", Gb.ToString("X")) : Gb.ToString("X")
				, Bb.ToString("X").Length == 1 ? String.Format("0{0}", Bb.ToString("X")) : Bb.ToString("X"));
		}

		/// <summary>
		/// Convert an hex string to a <see cref="Color4"/>.
		/// </summary>
		/// <param name="hex">Hexadecimal string.</param>
		/// <returns>An instance of <see cref="Color4"/>.</returns>
		public static Color4 HexToColor(string hex)
		{
			//in case the string is formatted 0xFFFFFF
			hex = hex.Replace("0x", "");
			//in case the string is formatted #FFFFFF
			hex = hex.Replace("#", "");

			//assume fully visible unless specified in hex
			byte a = 255;
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

			//Only use alpha if the string has enough characters
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			}

			return new Color4(r, g, b, a);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Check if this color is equals to another.
		/// </summary>
		/// <param name="C1">Left operand.</param>
		/// <param name="C2">Right operand.</param>
		/// <returns>True or false.</returns>
		public static bool operator ==(Color4 C1, Color4 C2)
		{
			return C1.Equals(C2);
		}

		/// <summary>
		/// Check if this color is not equals to another.
		/// </summary>
		/// <param name="C1">Left operand.</param>
		/// <param name="C2">Right operand.</param>
		/// <returns>True or false.</returns>
		public static bool operator !=(Color4 C1, Color4 C2)
		{
			return !C1.Equals(C2);
		}

		/// <summary>
		/// Check if this color is equals to another.
		/// </summary>
		/// <param name="obj">Color to compare.</param>
		/// <returns>True or false.</returns>
		public bool Equals(Color4 obj)
		{
			return r == obj.r && g == obj.g && b == obj.b && a == obj.a;
		}

		/// <summary>
		/// Check if this object is equals to another.
		/// </summary>
		/// <param name="obj">Second object.</param>
		/// <returns>True or false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is Color4))
				return false;
			if ((Color4)obj == this)
				return true;
			return false;
		}

		/// <summary>
		/// Returns an hash code for this color.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		#region Metadata

		/// <summary>
		/// Converts this color to a string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return $"{ColorToHex()} ({Rb},{Gb},{Bb},{Ab})";
		}

		#endregion

		#region Presets

		static public Color4 Maroon = new Color4(128, 0, 0, 255);
		static public Color4 DarkRed = new Color4(139, 0, 0, 255);
		static public Color4 Brown = new Color4(165, 42, 42, 255);
		static public Color4 Firebrick = new Color4(178, 34, 34, 255);
		static public Color4 Crimson = new Color4(220, 20, 60, 255);
		static public Color4 Red = new Color4(255, 0, 0, 255);
		static public Color4 Tomato = new Color4(255, 99, 71, 255);
		static public Color4 Coral = new Color4(255, 127, 80, 255);
		static public Color4 IndianRed = new Color4(205, 92, 92, 255);
		static public Color4 LightCoral = new Color4(240, 128, 128, 255);
		static public Color4 DarkSalmon = new Color4(233, 150, 122, 255);
		static public Color4 Salmon = new Color4(250, 128, 114, 255);
		static public Color4 LightSalmon = new Color4(255, 160, 122, 255);
		static public Color4 OrangeRed = new Color4(255, 69, 0, 255);
		static public Color4 DarkOrange = new Color4(255, 140, 0, 255);
		static public Color4 Orange = new Color4(255, 165, 0, 255);
		static public Color4 Gold = new Color4(255, 215, 0, 255);
		static public Color4 DarkGoldenRod = new Color4(184, 134, 11, 255);
		static public Color4 GoldenRod = new Color4(218, 165, 32, 255);
		static public Color4 PaleGoldenRod = new Color4(238, 232, 170, 255);
		static public Color4 DarkKhaki = new Color4(189, 183, 107, 255);
		static public Color4 Khaki = new Color4(240, 230, 140, 255);
		static public Color4 Olive = new Color4(128, 128, 0, 255);
		static public Color4 Yellow = new Color4(255, 255, 0, 255);
		static public Color4 YellowGreen = new Color4(154, 205, 50, 255);
		static public Color4 DarkOliveGreen = new Color4(85, 107, 47, 255);
		static public Color4 OliveDrab = new Color4(107, 142, 35, 255);
		static public Color4 LawnGreen = new Color4(124, 252, 0, 255);
		static public Color4 ChartReuse = new Color4(127, 255, 0, 255);
		static public Color4 GreenYellow = new Color4(173, 255, 47, 255);
		static public Color4 DarkGreen = new Color4(0, 100, 0, 255);
		static public Color4 Green = new Color4(0, 128, 0, 255);
		static public Color4 ForestGreen = new Color4(34, 139, 34, 255);
		static public Color4 Lime = new Color4(0, 255, 0, 255);
		static public Color4 LimeGreen = new Color4(50, 205, 50, 255);
		static public Color4 LightGreen = new Color4(144, 238, 144, 255);
		static public Color4 PaleGreen = new Color4(152, 251, 152, 255);
		static public Color4 DarkSeaGreen = new Color4(143, 188, 143, 255);
		static public Color4 MediumSpringGreen = new Color4(0, 250, 154, 255);
		static public Color4 SpringGreen = new Color4(0, 255, 127, 255);
		static public Color4 SeaGreen = new Color4(46, 139, 87, 255);
		static public Color4 MediumAquaMarine = new Color4(102, 205, 170, 255);
		static public Color4 MediumSeaGreen = new Color4(60, 179, 113, 255);
		static public Color4 LightSeaGreen = new Color4(32, 178, 170, 255);
		static public Color4 DarkSlateGray = new Color4(47, 79, 79, 255);
		static public Color4 Teal = new Color4(0, 128, 128, 255);
		static public Color4 DarkCyan = new Color4(0, 139, 139, 255);
		static public Color4 Aqua = new Color4(0, 255, 255, 255);
		static public Color4 Cyan = new Color4(0, 255, 255, 255);
		static public Color4 LightCyan = new Color4(224, 255, 255, 255);
		static public Color4 DarkTurquoise = new Color4(0, 206, 209, 255);
		static public Color4 Turquoise = new Color4(64, 224, 208, 255);
		static public Color4 MediumTurquoise = new Color4(72, 209, 204, 255);
		static public Color4 PaleTurquoise = new Color4(175, 238, 238, 255);
		static public Color4 AquaMarine = new Color4(127, 255, 212, 255);
		static public Color4 PowderBlue = new Color4(176, 224, 230, 255);
		static public Color4 CadetBlue = new Color4(95, 158, 160, 255);
		static public Color4 SteelBlue = new Color4(70, 130, 180, 255);
		static public Color4 CornFlowerBlue = new Color4(100, 149, 237, 255);
		static public Color4 DeepSkyBlue = new Color4(0, 191, 255, 255);
		static public Color4 DodgerBlue = new Color4(30, 144, 255, 255);
		static public Color4 LightBlue = new Color4(173, 216, 230, 255);
		static public Color4 SkyBlue = new Color4(135, 206, 235, 255);
		static public Color4 LightSkyBlue = new Color4(135, 206, 250, 255);
		static public Color4 MidnightBlue = new Color4(25, 25, 112, 255);
		static public Color4 Navy = new Color4(0, 0, 128, 255);
		static public Color4 DarkBlue = new Color4(0, 0, 139, 255);
		static public Color4 MediumBlue = new Color4(0, 0, 205, 255);
		static public Color4 Blue = new Color4(0, 0, 255, 255);
		static public Color4 RoyalBlue = new Color4(65, 105, 225, 255);
		static public Color4 BlueViolet = new Color4(138, 43, 226, 255);
		static public Color4 Indigo = new Color4(75, 0, 130, 255);
		static public Color4 DarkSlateBlue = new Color4(72, 61, 139, 255);
		static public Color4 SlateBlue = new Color4(106, 90, 205, 255);
		static public Color4 MediumSlateBlue = new Color4(123, 104, 238, 255);
		static public Color4 MediumPurple = new Color4(147, 112, 219, 255);
		static public Color4 DarkMagenta = new Color4(139, 0, 139, 255);
		static public Color4 DarkViolet = new Color4(148, 0, 211, 255);
		static public Color4 DarkOrchid = new Color4(153, 50, 204, 255);
		static public Color4 MediumOrchid = new Color4(186, 85, 211, 255);
		static public Color4 Purple = new Color4(128, 0, 128, 255);
		static public Color4 Thistle = new Color4(216, 191, 216, 255);
		static public Color4 Plum = new Color4(221, 160, 221, 255);
		static public Color4 Violet = new Color4(238, 130, 238, 255);
		static public Color4 MagentaFuchsia = new Color4(255, 0, 255, 255);
		static public Color4 Orchid = new Color4(218, 112, 214, 255);
		static public Color4 MediumVioletRed = new Color4(199, 21, 133, 255);
		static public Color4 PaleVioletRed = new Color4(219, 112, 147, 255);
		static public Color4 DeepPink = new Color4(255, 20, 147, 255);
		static public Color4 HotPink = new Color4(255, 105, 180, 255);
		static public Color4 LightPink = new Color4(255, 182, 193, 255);
		static public Color4 Pink = new Color4(255, 192, 203, 255);
		static public Color4 AntiqueWhite = new Color4(250, 235, 215, 255);
		static public Color4 Beige = new Color4(245, 245, 220, 255);
		static public Color4 Bisque = new Color4(255, 228, 196, 255);
		static public Color4 BlanchedAlmond = new Color4(255, 235, 205, 255);
		static public Color4 Wheat = new Color4(245, 222, 179, 255);
		static public Color4 CornSilk = new Color4(255, 248, 220, 255);
		static public Color4 LemonChiffon = new Color4(255, 250, 205, 255);
		static public Color4 LightGoldenRodYellow = new Color4(250, 250, 210, 255);
		static public Color4 LightYellow = new Color4(255, 255, 224, 255);
		static public Color4 SaddleBrown = new Color4(139, 69, 19, 255);
		static public Color4 Sienna = new Color4(160, 82, 45, 255);
		static public Color4 Chocolate = new Color4(210, 105, 30, 255);
		static public Color4 Peru = new Color4(205, 133, 63, 255);
		static public Color4 SandyBrown = new Color4(244, 164, 96, 255);
		static public Color4 BurlyWood = new Color4(222, 184, 135, 255);
		static public Color4 Tan = new Color4(210, 180, 140, 255);
		static public Color4 RosyBrown = new Color4(188, 143, 143, 255);
		static public Color4 Moccasin = new Color4(255, 228, 181, 255);
		static public Color4 NavajoWhite = new Color4(255, 222, 173, 255);
		static public Color4 PeachPuff = new Color4(255, 218, 185, 255);
		static public Color4 MistyRose = new Color4(255, 228, 225, 255);
		static public Color4 LavenderBlush = new Color4(255, 240, 245, 255);
		static public Color4 Linen = new Color4(250, 240, 230, 255);
		static public Color4 OldLace = new Color4(253, 245, 230, 255);
		static public Color4 PapayaWhip = new Color4(255, 239, 213, 255);
		static public Color4 SeaShell = new Color4(255, 245, 238, 255);
		static public Color4 MintCream = new Color4(245, 255, 250, 255);
		static public Color4 SlateGray = new Color4(112, 128, 144, 255);
		static public Color4 LightSlateGray = new Color4(119, 136, 153, 255);
		static public Color4 LightSteelBlue = new Color4(176, 196, 222, 255);
		static public Color4 Lavender = new Color4(230, 230, 250, 255);
		static public Color4 FloralWhite = new Color4(255, 250, 240, 255);
		static public Color4 AliceBlue = new Color4(240, 248, 255, 255);
		static public Color4 GhostWhite = new Color4(248, 248, 255, 255);
		static public Color4 Honeydew = new Color4(240, 255, 240, 255);
		static public Color4 Ivory = new Color4(255, 255, 240, 255);
		static public Color4 Azure = new Color4(240, 255, 255, 255);
		static public Color4 Snow = new Color4(255, 250, 250, 255);
		static public Color4 Black = new Color4(0, 0, 0, 255);
		static public Color4 DimGrayDimGrey = new Color4(105, 105, 105, 255);
		static public Color4 GrayGrey = new Color4(128, 128, 128, 255);
		static public Color4 DarkGray = new Color4(169, 169, 169, 255);
		static public Color4 Silver = new Color4(192, 192, 192, 255);
		static public Color4 LightGrayLightGrey = new Color4(211, 211, 211, 255);
		static public Color4 Gainsboro = new Color4(220, 220, 220, 255);
		static public Color4 WhiteSmoke = new Color4(245, 245, 245, 255);
		static public Color4 White = new Color4(255, 255, 255, 255);

		#endregion
	}
}

