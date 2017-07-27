using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using pEngine.Common;

namespace pEngine.Core.Graphics.Renderer.Batches
{
	/// <summary>
	/// OpenGL vertex data.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct GLVertex
    {
		/// <summary>
		/// Struct size.
		/// </summary>
		public static uint Stride => (uint)Marshal.SizeOf(default(GLVertex));

		/// <summary>
		/// String rappresentation of this struct.
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			return $"({X}, {Y}) -> ({Tx}, {Ty}) {new Color4(R,G,B,A).ColorToHex()}";
		}

		/// <summary>
		/// X axis choord.
		/// </summary>
		public float X;

		/// <summary>
		/// Y axis choord.
		/// </summary>
		public float Y;

		/// <summary>
		/// Texture X position.
		/// </summary>
		public float Tx;

		/// <summary>
		/// Textire Y position.
		/// </summary>
		public float Ty;

		/// <summary>
		/// Color Red component.
		/// </summary>
		public float R;

		/// <summary>
		/// Color Green component.
		/// </summary>
		public float G;

		/// <summary>
		/// Color Blue component.
		/// </summary>
		public float B;

		/// <summary>
		/// Color Alpha component.
		/// </summary>
		public float A;

	}
}
