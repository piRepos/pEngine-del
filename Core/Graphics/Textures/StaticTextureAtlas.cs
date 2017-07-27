using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data;
using pEngine.Core.Data.Files;

using pEngine.Common.Memory;
using pEngine.Common.Math;

namespace pEngine.Core.Graphics.Textures
{
	/// <summary>
	/// Pack all specified texture in a single texture for
	/// rendering optimization.
	/// </summary>
	public class StaticTextureAtlas : StaticTextureAtlas<string, DependencyTexture>
	{
		public StaticTextureAtlas(bool adaptSize2Pow = false)
			: base(adaptSize2Pow)
		{
		}

		/// <summary>
		/// Loads a collection of images.
		/// </summary>
		/// <param name="files">Files to load (tuple of image name and path).</param>
		public void LoadFromFiles(params (string, string)[] files)
		{
			foreach (var file in files)
			{
				TextureFromFile(file.Item1, file.Item2);
			}
		}

		/// <summary>
		/// Loads a collection of images.
		/// </summary>
		/// <param name="images">Images to load (tuple of image name and image class).</param>
		public void LoadFromImages(params (string, Image)[] images)
		{
			foreach (var image in images)
			{
				TextureFromImage(image.Item1, image.Item2);
			}
		}

		/// <summary>
		/// Gets a texture from a file path.
		/// </summary>
		/// <param name="path">Texture image file path.</param>
		/// <returns>A texture resource.</returns>
		public DependencyTexture TextureFromFile(string name, string path)
		{
			Image image = new Image(path);

			return TextureFromImage(name, image);
		}

		/// <summary>
		/// Gets a texture from an image.
		/// </summary>
		/// <param name="path">Image object.</param>
		/// <returns>A texture resource.</returns>
		public DependencyTexture TextureFromImage(string name, Image image)
		{
			DependencyTexture d = Add(name, new PixelBuffer());

			if (!image.IsLoaded)
			{
				image.Aborted += (IResource res, Exception e) => throw e;
				image.Completed += (IResource res) =>
				{
					textures[name].Data = image.RawPixels;
					textures[name].TextureReference.Size = image.RawPixels.BufferSize;
					textures[name].TextureReference.Reference = this;
				};

				Dependencies.Add(image);
			}
			else
			{
				textures[name].Data = image.RawPixels;
				textures[name].TextureReference.Size = image.RawPixels.BufferSize;
				textures[name].TextureReference.Reference = this;
			}

			return d;
		}
	}

	/// <summary>
	/// Pack all specified texture in a single texture for
	/// rendering optimization.
	/// </summary>
	public class StaticTextureAtlas<Key, TextureType> : StandaloneTexture, IEnumerable<TextureType>
		where TextureType : DependencyTexture
    {

		/// <summary>
		/// Creates a new <see cref="StaticTextureAtlas"/>.
		/// </summary>
		/// <param name="images">Couples of tewxture identifier and texture image.</param>
		public StaticTextureAtlas(bool adaptSize2Pow = false)
			: base()
		{
			AdaptSize2Pow = adaptSize2Pow;
			textures = new Dictionary<Key, Loader>();
			AtlasMaxSize = new Vector2i(int.MaxValue, int.MaxValue);
			IsPartial = false;
		}

		/// <summary>
		/// Loaded textures.
		/// </summary>
		public IEnumerable<TextureType> Textures => textures.Values.Select(x => x.TextureReference);

		/// <summary>
		/// Texture atlas max size.
		/// </summary>
		public Vector2i AtlasMaxSize { get; }

		/// <summary>
		/// Ceil the size to next pow of 2.
		/// </summary>
		public bool AdaptSize2Pow { get; }

		#region Load management

		protected Dictionary<Key, Loader> textures;

		protected class Loader
		{
			/// <summary>
			/// Data to load.
			/// </summary>
			public PixelBuffer Data;

			/// <summary>
			/// Target texture.
			/// </summary>
			public TextureType TextureReference;

			/// <summary>
			/// True if loaded.
			/// </summary>
			public bool Loaded;

			/// <summary>
			/// If false object will not loaded.
			/// </summary>
			public bool Loadable = true;
		}

		/// <summary>
		/// Adds a texture to this atlas for loading.
		/// </summary>
		/// <param name="name">Texture identifier.</param>
		/// <param name="pixels">Texture to load.</param>
		protected TextureType Add(Key name, PixelBuffer pixels)
		{
			TextureType tx = Activator.CreateInstance<TextureType>();

			var loader = new Loader
			{
				Data = pixels,
				TextureReference = tx,
				Loaded = false
			};
			
			textures.Add(name, loader);
			Dependencies.Add(tx);

			return tx;
		}

		#endregion


		#region Loading

		internal override void OnLoad()
		{
			RectanglePacker<DependencyTexture> packer = new RectanglePacker<DependencyTexture>();
			packer.Spacing = new Vector2i(16, 16);

			packer.AddObjects(textures.Where(x => x.Value.Loadable).Select(x => x.Value.TextureReference).ToArray());

			// - Calculate positions
			packer.Fit();

			// - Check used space
			if (packer.UsedSpace.Width >= AtlasMaxSize.Width || packer.UsedSpace.Height >= AtlasMaxSize.Height)
				throw new OutOfMemoryException("No space in the atlas.");

			// - Calculate the mipmap level
			MipmapTargetLevel = (uint)Math.Ceiling(Math.Log(Math.Max(packer.UsedSpace.Width, packer.UsedSpace.Height), 2)) + 1;
			Mipmaps = new PixelBuffer[MipmapTargetLevel];

			// - Create pixel array
			int maxSize = MathHelpers.NextPowerOfTwo(Math.Max(packer.UsedSpace.Width, packer.UsedSpace.Height));
			if (AdaptSize2Pow)
				Mipmaps[0] = new PixelBuffer(new Vector2i(maxSize, maxSize));
			else
				Mipmaps[0] = new PixelBuffer(packer.UsedSpace);

			// - For each texture write pixels in the atlas
			foreach (var tx in textures)
			{
				if (!tx.Value.Loadable)
					continue;

				WriteImage(tx.Value.Data, tx.Value.TextureReference.Position);
				tx.Value.Data = new PixelBuffer();
			}

			base.OnLoad();
		}

		internal override void OnComplete()
		{
			foreach (var tx in textures)
			{
				DependencyTexture d = tx.Value.TextureReference;
				d.RasterPosition = d.Position / (Vector2)Size;
				d.RasterSize = d.Size / (Vector2)Size;

				if (!tx.Value.Loaded)
				{
					d.OnComplete();
					tx.Value.Loaded = true;
				}
			}

			base.OnComplete();
		}

		internal override bool OnAbort(IResource res, Exception e)
		{
			return base.OnAbort(res, e);
		}

		private void WriteImage(PixelBuffer Pixels, Vector2i Position)
		{
			for (int i = 0; i < Pixels.BufferSize.Height; ++i)
			{
				int DestPosY = (i + Position.Y) * Size.Width;
				int DestPosX = Position.X;

				int SourceY = i * Pixels.BufferSize.Width;

				Array.Copy(Pixels.Pixels, (SourceY * 4), Mipmaps[0].Pixels, (DestPosX + DestPosY) * 4, Pixels.BufferSize.Width * 4);
			}
		}

		#endregion

		#region Enumeration

		/// <summary>
		/// Gets a texture using the string identifier.
		/// </summary>
		/// <param name="index">Texture name.</param>
		/// <returns>Texture found.</returns>
		public TextureType this[Key index] => textures[index].TextureReference;

		/// <summary>
		/// Gets the texture enumerator for this atlas.
		/// </summary>
		/// <returns>Texture enumerator.</returns>
		public IEnumerator<TextureType> GetEnumerator()
		{
			return textures.Values.Select(x => x.TextureReference).GetEnumerator();
		}

		/// <summary>
		/// Generic enumerator.
		/// </summary>
		/// <returns>Enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

	}
}
