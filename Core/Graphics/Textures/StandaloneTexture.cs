using System;

using pEngine.Common.Math;
using pEngine.Core.Data;
using pEngine.Core.Data.Files;
using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Graphics.Renderer.Textures;

using pEngine.Common.Extensions;
using pEngine.Common.Memory;

namespace pEngine.Core.Graphics.Textures
{
    public class StandaloneTexture : Resource, IDependency<TextureDescriptor>, ITexture
    {
		/// <summary>
		/// Creates a new <see cref="Texture"/>.
		/// </summary>
		public StandaloneTexture()
		{
			// - Create first mipmap
			Mipmaps = new PixelBuffer[1];

			Mipmapping = true;
			LegacyMipmapping = true;
		}

		/// <summary>
		/// Creates a new <see cref="Texture"/> from a pixel buffer.
		/// </summary>
		/// <param name="rawImage">Texture pixels</param>
		public StandaloneTexture(PixelBuffer rawImage)
		{ 
			// - Create first mipmap
			Mipmaps = new PixelBuffer[1];

			// - Set first mipmap
			Mipmaps[0] = rawImage;

			Mipmapping = true;
			LegacyMipmapping = true;
		}

		/// <summary>
		/// Load a new <see cref="Texture"/> from an <see cref="Image"/> resource.
		/// </summary>
		/// <param name="bitmapImage">Image to load.</param>
		public StandaloneTexture(Image bitmapImage)
		{
			Mipmaps = new PixelBuffer[1];

			bitmapImage.Completed += (r) => Mipmaps[0] = bitmapImage.RawPixels;

			Dependencies.Add(bitmapImage);

			Mipmapping = true;
			LegacyMipmapping = true;
		}

		/// <summary>
		/// Texture image raw pixels.
		/// </summary>
		public PixelBuffer Pixels => Mipmaps[0];

		/// <summary>
		/// Texture mipmaps.
		/// </summary>
		public PixelBuffer[] Mipmaps { get; protected set; }

		/// <summary>
		/// Enable or disable mipmapping.
		/// </summary>
		public bool Mipmapping { get => MipmapMaxLevel > 1; set => MipmapMaxLevel = value ? uint.MaxValue : 1; }

		/// <summary>
		/// Limits mipmap level.
		/// </summary>
		public uint MipmapMaxLevel { get; set; }

		/// <summary>
		/// Target mipmap level for this texture.
		/// </summary>
		public uint MipmapTargetLevel { get; protected set; }

		/// <summary>
		/// Current mipmap level.
		/// </summary>
		public uint CurrentMipmapLevel => Math.Min(MipmapMaxLevel, MipmapTargetLevel);

		/// <summary>
		/// Calculate mipmaps using GPU.
		/// </summary>
		public bool LegacyMipmapping { get; set; } = true;

		/// <summary>
		/// Texture relative size (value from 0 to 1).
		/// </summary>
		public virtual Vector2 RasterSize { get; internal set; } = new Vector2(1, 1);

		/// <summary>
		/// Texture relative position (value from 0 to 1).
		/// </summary>
		public virtual Vector2 RasterPosition { get; internal set; } = new Vector2(0, 0);

		/// <summary>
		/// Real texture size in pixels.
		/// </summary>
		public Vector2i Size => Mipmaps[0].BufferSize;

		/// <summary>
		/// Real texture position if ther's a parent.
		/// </summary>
		public Vector2i Position { get; set; }

		/// <summary>
		/// Top left corner texture position.
		/// </summary>
		public Vector2 TopLeft => RasterPosition + new Vector2(0, RasterSize.Height);

		/// <summary>
		/// Top right corner texture position.
		/// </summary>
		public Vector2 TopRight => RasterPosition + new Vector2(RasterSize.Width, RasterSize.Height);

		/// <summary>
		/// Bottom right corner texture position.
		/// </summary>
		public Vector2 BotRight => RasterPosition + new Vector2(RasterSize.Width, 0);

		/// <summary>
		/// Bottom left corner texture position.
		/// </summary>
		public Vector2 BotLeft => RasterPosition + new Vector2(0, 0);

		/// <summary>
		/// Used space in RAM memory.
		/// </summary>
		public override uint UsedSpace => (uint)Pixels.Pixels.Length * sizeof(byte);

		/// <summary>
		/// Releases all resource used by the <see cref="Texture"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Texture"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Texture"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Texture"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Texture"/> was occupying.</remarks>
		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// Clear pixel buffer.
		/// </summary>
		public void FreeBuffer()
		{
			Mipmaps = null;
		}

		#region Loading

		/// <summary>
		/// Loads this texture.
		/// </summary>
		public override void Load(Game game)
		{
			game.Host.Textures.LoadTexture(this);
		}

		/// <summary>
		/// Loads this texture asyncronously.
		/// </summary>
		public override void LoadAsync(Game game)
		{
			game.Host.Textures.LoadTextureAsync(this);
		}

		/// <summary>
		/// Complete loading.
		/// </summary>
		internal override void OnComplete()
		{
			base.OnComplete();

			Dependencies.RemoveAll(x => x is Image);
			Invalidated = true;
		}

		/// <summary>
		/// Abort resource loading.
		/// </summary>
		internal override bool OnAbort(IResource res, Exception e)
		{
			base.OnAbort(res, e);

			return true;
		}

		/// <summary>
		/// Load this resource.
		/// </summary>
		internal override void OnLoad()
		{
			MipmapTargetLevel = (uint)Math.Ceiling(Math.Log(Math.Max(Size.Width, Size.Height), 2));

			if (Mipmapping && !LegacyMipmapping)
			{
				for (int i = 1; i < CurrentMipmapLevel; ++i)
				{
					Vector2i NewSize = Pixels.BufferSize / Math.Pow(2, i);

					NewSize.Width = Math.Max(1, (int)Math.Floor(Pixels.BufferSize.Width / Math.Pow(2, i)));
					NewSize.Height = Math.Max(1, (int)Math.Floor(Pixels.BufferSize.Height / Math.Pow(2, i)));

					Mipmaps[i] = Pixels.GetScaledBuffer(NewSize, FilteringType.Lanczos3);
				}
			}
		}

		/// <summary>
		/// Calculate mipmap for specified level.
		/// </summary>
		/// <param name="Level">Mipmap level.</param>
		/// <returns>Mipmap pixels.</returns>
		public PixelBuffer GetMipmap(int Level)
		{
			return Mipmaps[Level];
		}

		#endregion

		#region Dependency

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		public virtual TextureDescriptor GetDescriptor()
		{
			PixelBuffer[] mipmaps = new PixelBuffer[CurrentMipmapLevel];
			Array.Copy(Mipmaps, mipmaps, Math.Min(Mipmaps.Length, CurrentMipmapLevel));

			return new TextureDescriptor
			{
				Buffer = mipmaps,
				DescriptorID = DependencyID,
				Mipmaps = CurrentMipmapLevel,
				GenerateMipmaps = LegacyMipmapping && Mipmapping
			};
		}

		/// <summary>
		/// True if the resource is changed.
		/// </summary>
		public virtual bool Invalidated { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public virtual long DependencyID { get; set; }

		#endregion

	}
}
