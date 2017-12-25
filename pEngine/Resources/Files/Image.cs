using System;
using System.Runtime.InteropServices;

using pEngine.Utils.Memory;

using FreeImageAPI;

namespace pEngine.Resources.Files
{
	using ImageFormat = FREE_IMAGE_FORMAT;

	public class Image : File
    {
		/// <summary>
		/// Creates a new <see cref="Image"/> file handler.
		/// </summary>
		/// <param name="path">Image path.</param>
		public Image(string path)
			: base(path)
		{

		}

		/// <summary>
		/// Image raw pixels.
		/// </summary>
		public PixelBuffer RawPixels { get; private set; }

		/// <summary>
		/// Image format.
		/// </summary>
		public ImageFormat Format { get; private set; }



		internal override void OnLoad()
		{
			if (RawPixels == PixelBuffer.Empty)
			{
				base.OnLoad();

				// Prepare unamanged pointer
				IntPtr Data = Marshal.AllocHGlobal(Content.Length);
				Marshal.Copy(Content, 0, Data, Content.Length);

				// Load memory
				FIMEMORY Memory = FreeImage.OpenMemory(Data, Size);

				// Get image format
				Format = FreeImage.GetFileTypeFromMemory(Memory, 0);

				// Get bitmap
				FIBITMAP Bitmap = FreeImage.LoadFromMemory(Format, Memory, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

				// Get depth
				uint Depth = FreeImage.GetBPP(Bitmap) / 8;

				FIBITMAP ConvertedBirmap;

				// Convert to depth 4
				if (Depth == 4) ConvertedBirmap = Bitmap;
				else
				{
					ConvertedBirmap = FreeImage.ConvertTo32Bits(Bitmap);
					FreeImage.Unload(Bitmap);
				}

				// Get meta
				Vector2i S = new Vector2i();
				S.Width = (int)FreeImage.GetWidth(ConvertedBirmap);
				S.Height = (int)FreeImage.GetHeight(ConvertedBirmap);

				RawPixels = new PixelBuffer(S);

				// Get pixels
				IntPtr Pixels = FreeImage.GetBits(ConvertedBirmap);
				Marshal.Copy(Pixels, RawPixels.Pixels, 0, RawPixels.Pixels.Length);

				FreeImage.Unload(ConvertedBirmap);

				byte Swap;

				// Convert from GBRA to RGBA
				for (int i = 0; i < RawPixels.Pixels.Length; i += 4)
				{
					Swap = RawPixels.Pixels[i];
					RawPixels.Pixels[i] = RawPixels.Pixels[i + 2];
					RawPixels.Pixels[i + 2] = Swap;
				}

				// Free data
				Marshal.FreeHGlobal(Data);
			}
		}

		internal override void OnComplete()
		{
			base.OnComplete();
		}

		internal override bool OnAbort(IResource res, Exception e)
		{
			return base.OnAbort(res, e);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override uint UsedSpace => throw new NotImplementedException();

	}
}
