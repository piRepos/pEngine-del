using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Textures;
using pEngine.Core.Graphics.Textures;

using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Data.Files;
using pEngine.Core.Data;

namespace pEngine.Core.Graphics.Fonts
{
	using DependencyManager = FrameDependencyManager<StandaloneTexture, TextureDescriptor>;

	public class FontStore : DependencyManager
	{

		/// <summary>
		/// Makes a new instance of <see cref="FontStore"/>.
		/// </summary>
		/// <param name="host"></param>
		public FontStore(pEngine host)
			: base(host)
		{
		}

		/// <summary>
		/// Loads a <see cref="StaticFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFont(StaticFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(fnt);
			host.Resources.Load(fnt);
		}

		/// <summary>
		/// Loads a <see cref="DynamicFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFont(DynamicFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(fnt);
			host.Resources.Load(fnt);
		}

		/// <summary>
		/// Loads a <see cref="StaticFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFontAsync(StaticFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(fnt);
			host.Resources.LoadAsync(fnt);
		}

		/// <summary>
		/// Loads a <see cref="DynamicFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFontAsync(DynamicFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(fnt);
			host.Resources.LoadAsync(fnt);
		}

		/// <summary>
		/// Gets a font from a file path.
		/// </summary>
		/// <param name="path">Font ttf file path.</param>
		/// <returns>A font resource.</returns>
		public DynamicFont GetFont(string path, int size, bool outline = false, bool async = false)
		{
            TrueTypeFont fontFile = new TrueTypeFont(path);

			DynamicFont fnt = new DynamicFont(fontFile, size, outline);

			if (async)
				LoadFontAsync(fnt);
			else
			{
                fnt.Aborted += (IResource res, Exception e) => throw e;
				LoadFont(fnt);
			}

            return fnt;
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
