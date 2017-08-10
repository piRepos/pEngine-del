using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Textures;

using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Data.Files;
using pEngine.Core.Data;

namespace pEngine.Core.Graphics.Textures
{
	using DependencyManager = FrameDependencyManager<StandaloneTexture, TextureDescriptor>;

	public class TextureStore : DependencyManager
	{

		/// <summary>
		/// Makes a new instance of <see cref="TextureStore"/>.
		/// </summary>
		/// <param name="host"></param>
		public TextureStore(pEngine host)
			: base(host)
		{
		}

		/// <summary>
		/// Loads a <see cref="Texture"/> for rendering.
		/// </summary>
		/// <param name="tex">Texture to load.</param>
		public void LoadTexture(StandaloneTexture tex)
		{
			tex.Completed += (IResource r) => AddDependency(tex);
			host.Resources.Load(tex);
		}

		/// <summary>
		/// Loads a <see cref="Texture"/> asyncronously.
		/// </summary>
		/// <param name="tex">Texture to load.</param>
		public void LoadTextureAsync(StandaloneTexture tex)
		{
			tex.Completed += (IResource r) => AddDependency(tex);
			host.Resources.LoadAsync(tex);
		}

		/// <summary>
		/// Gets a texture from a file path.
		/// </summary>
		/// <param name="path">Texture image file path.</param>
		/// <returns>A texture resource.</returns>
		public StandaloneTexture GetTexture(string path, bool async = false)
		{
			Image image = new Image(path);

			StandaloneTexture tx = new StandaloneTexture(image);

			if (async)
				LoadTextureAsync(tx);
			else
			{
				tx.Aborted += (IResource res, Exception e) => throw e;
				LoadTexture(tx);
			}

			return tx;
		}

		/// <summary>
		/// Gets an empty texture.
		/// </summary>
		/// <returns>A texture resource.</returns>
		public StandaloneTexture GetTexture(Vector2i size)
		{
			StandaloneTexture tx = new StandaloneTexture(new Common.Memory.PixelBuffer(size));

			tx.Aborted += (IResource res, Exception e) => throw e;
			LoadTexture(tx);

			return tx;
		}

		#region Frame dependency

		/// <summary>
		/// Add a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to add.</param>
		public override void AddDependency(StandaloneTexture dependency)
		{
			base.AddDependency(dependency);
		}

		/// <summary>
		/// Remove a dependency to the manager.
		/// </summary>
		/// <param name="dependency">Dependency to remove.</param>
		public override void RemoveDependency(StandaloneTexture dependency)
		{
			base.RemoveDependency(dependency);
		}

		/// <summary>
		/// Set a dependency as loaded from the loader thread.
		/// </summary>
		/// <param name="frame">Frame when is loaded.</param>
		public override void SetDependencyLoaded(long descriptorId, long frame)
		{
			base.SetDependencyLoaded(descriptorId, frame);
		}

		#endregion
	}
}
