using System;
using System.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using FreeImageAPI;

using pEngine.Common.Math;

namespace pEngine.Common.Memory
{
	public enum FilteringType
	{
		Bicubic = FREE_IMAGE_FILTER.FILTER_BICUBIC,
		Bilinear = FREE_IMAGE_FILTER.FILTER_BILINEAR,
		Box = FREE_IMAGE_FILTER.FILTER_BOX,
		Bspline = FREE_IMAGE_FILTER.FILTER_BSPLINE,
		Catmullrom = FREE_IMAGE_FILTER.FILTER_CATMULLROM,
		Lanczos3 = FREE_IMAGE_FILTER.FILTER_LANCZOS3
	}

	/// <summary>
	/// Manage an array of pixels RGBA.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct PixelBuffer : IEnumerable<byte>, IEquatable<PixelBuffer>
	{

		public static PixelBuffer Empty => new PixelBuffer();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:pEngine.Common.PixelBuffer"/> class.
		/// </summary>
		/// <param name="Pixels">RGBA Pixel array.</param>
		/// <param name="BufferSize">Buffer size.</param>
		public PixelBuffer(byte[] Pixels, Vector2i BufferSize)
		{
			this.Pixels = Pixels;
			this.BufferSize = BufferSize;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:pEngine.Common.PixelBuffer"/> class.
		/// </summary>
		/// <param name="BufferSize">Buffer size.</param>
		public PixelBuffer(Vector2i BufferSize)
		{
			Pixels = new byte[BufferSize.Width * BufferSize.Height * 4];
			this.BufferSize = BufferSize;
		}


		#region Buffer

		/// <summary>
		/// Gets or sets the pixels.
		/// </summary>
		public byte[] Pixels { get; private set; }

		/// <summary>
		/// Gets or sets the size of the buffer.
		/// </summary>
		public Vector2i BufferSize { get; private set; }

		/// <summary>
		/// Sets a new buffer.
		/// </summary>
		/// <param name="Pixels">Buffer pixels.</param>
		/// <param name="BufferSize">Buffer size.</param>
		void SetBuffer(byte[] Pixels, Vector2i BufferSize)
		{
			this.Pixels = Pixels;
			this.BufferSize = BufferSize;
		}

		/// <summary>
		/// Gets the specified pixel color..
		/// </summary>
		/// <param name="Position">Pixel position.</param>
		Color4 GetPixelColor(Vector2i Position)
		{
			if (Position.X > BufferSize.Width || Position.Y > BufferSize.Height)
				throw new IndexOutOfRangeException();
			
			int Index = Position.X + (Position.Y * BufferSize.Width);
			return new Color4(Pixels[Index], Pixels[Index + 1], Pixels[Index + 2], Pixels[Index + 3]);
		}

		#endregion

		#region Bleeding

		public PixelBuffer GetBleededBuffer()
		{
			byte[] NewBuffer = Pixels.ToArray();
			MakeBleeding(NewBuffer, BufferSize.Width, BufferSize.Height);
			return new PixelBuffer(NewBuffer, BufferSize);
		}

		private void MakeBleeding(byte[] image, int width, int height)
		{
			int N = width * height;

			List<int> opaque = new List<int>(new int[N]);
			List<bool> loose = new List<bool>(new bool[N]);
			List<int> pending = new List<int>(new int[N]);
			List<int> pendingNext = new List<int>(new int[N]);

			int[,] offsets = new int[8, 2]
				{
					{-1, -1},
					{ 0, -1},
					{ 1, -1},
					{-1,  0},
					{ 1,  0},
					{-1,  1},
					{ 0,  1},
					{ 1,  1}
				};

			for (int i = 0, j = 3; i < N; i++, j += 4)
			{
				if (image[j] < 255)
				{
					bool isLoose = true;

					int x = MathHelpers.Mod(i, width);
					int y = i / width;

					for (int k = 0; k < 8; k++)
					{
						int s = offsets[k, 0];
						int t = offsets[k, 1];

						if (x + s >= 0 && x + s < width && y + t >= 0 && y + t < height)
						{
							int index = j + 4 * (s + t * width);

							if (image[index] != 0)
							{
								isLoose = false;
								break;
							}
						}
					}

					if (!isLoose)
						pending.Add(i);
					else
						loose[i] = true;
				}
				else
				{
					opaque[i] = -1;
				}
			}

			while (pending.Count > 0)
			{
				pendingNext.Clear();

				for (int p = 0; p < pending.Count; p++)
				{
					int i = pending[p] * 4;
					int j = pending[p];

					int x = MathHelpers.Mod(j, width);
					int y = j / width;

					int r = 0;
					int g = 0;
					int b = 0;

					int count = 0;

					for (int k = 0; k < 8; k++)
					{
						int s = offsets[k, 0];
						int t = offsets[k, 1];

						if (x + s >= 0 && x + s < width && y + t >= 0 && y + t < height)
						{
							t *= width;

							if ((opaque[j + s + t] & 1) != 0)
							{
								int index = i + 4 * (s + t);

								r += image[index + 0];
								g += image[index + 1];
								b += image[index + 2];

								count++;
							}
						}
					}

					if (count > 0)
					{
						image[i + 0] = (byte)(r / count);
						image[i + 1] = (byte)(g / count);
						image[i + 2] = (byte)(b / count);

						opaque[j] = 0xFE;

						for (int k = 0; k < 8; k++)
						{
							int s = offsets[k, 0];
							int t = offsets[k, 1];

							if (x + s >= 0 && x + s < width && y + t >= 0 && y + t < height)
							{
								int index = j + s + t * width;

								if (loose[index])
								{
									pendingNext.Add(index);
									loose[index] = false;
								}
							}
						}
					}
					else
					{
						pendingNext.Add(j);
					}
				}

				if (pendingNext.Count > 0)
				{
					for (int p = 0; p < pending.Count; p++)
						opaque[pending[p]] >>= 1;
				}

				List<int> Tmp;
				Tmp = pending;
				pending = pendingNext;
				pendingNext = Tmp;
			}
		}

		#endregion

		#region Scaling

		/// <summary>
		/// Gets a new buffer with the specified size.
		/// </summary>
		/// <returns>The scaled buffer.</returns>
		/// <param name="NewSize">New size.</param>
		public PixelBuffer GetScaledBuffer(Vector2i newSize, FilteringType filtering)
		{
			FreeImageBitmap Scaled;

			using (FreeImageBitmap Bmp = new FreeImageBitmap(BufferSize.Width, BufferSize.Height, BufferSize.Width * 4, PixelFormat.Format32bppArgb, Pixels))
			{
				// Get scaled bitmap
				Scaled = Bmp.GetScaledInstance(newSize, (FREE_IMAGE_FILTER)filtering);
			}

			PixelBuffer buffer = new PixelBuffer(newSize);

			// Convert from GBRA to RGBA
			int z = 0;
			for (int i = newSize.Height - 1; i >= 0; i--, z++)
			{
				Marshal.Copy(Scaled.Bits + (z * newSize.Width * 4), buffer.Pixels, i * newSize.Width * 4, newSize.Width * 4);
			}

			Scaled.Dispose();

			return buffer;
		}


		#endregion

		#region Equitable

		public static bool operator ==(PixelBuffer b1, PixelBuffer b2)
		{
			return b1.Equals(b2);
		}

		public static bool operator !=(PixelBuffer b1, PixelBuffer b2)
		{
			return !b1.Equals(b2);
		}

		public bool Equals(PixelBuffer other)
		{
			if (Pixels == null && other.Pixels == null)
				return true;
			else if (Pixels == null || other.Pixels == null)
				return false;

			if (other.Pixels.Length != Pixels.Length)
				return false;

			if (other.BufferSize != BufferSize)
				return false;

			if (other.Pixels != Pixels)
				return false;

			return true;
		}

        public override bool Equals(object obj)
        {
            if (obj is PixelBuffer)
                return Equals((PixelBuffer)obj);

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 1;

            unchecked
            {
                foreach (var v in Pixels)
                    hash ^= v;
            }

            return hash;
        }

		#endregion

		#region Enumeration

		public Color4 this[int x, int y]
		{
			get => GetPixelColor(new Vector2i(x, y));
			set
			{
				Array.Copy(new byte[] { value.Rb, value.Gb, value.Bb, value.Ab }, 0, Pixels, (y * BufferSize.Width * 4) + (x * 4), 4);
			}
		}

		public IEnumerator<byte> GetEnumerator()
		{
			for (int i = 0; i < Pixels.Length; i++)
			{
				yield return Pixels[i];
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
