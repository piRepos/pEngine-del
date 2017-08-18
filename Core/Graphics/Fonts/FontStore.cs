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
	using DependencyManager = KeyFrameDependencyManager<string, StandaloneTexture, TextureDescriptor>;

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
		public void LoadFont(string name, StaticFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(name, fnt);
			host.Resources.Load(fnt);
		}

		/// <summary>
		/// Loads a <see cref="DynamicFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFont(string name, DynamicFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(name, fnt);
			host.Resources.Load(fnt);
		}

		/// <summary>
		/// Loads a <see cref="StaticFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFontAsync(string name, StaticFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(name, fnt);
			host.Resources.LoadAsync(fnt);
		}

		/// <summary>
		/// Loads a <see cref="DynamicFont"/> for rendering.
		/// </summary>
		/// <param name="fnt">Font to load.</param>
		public void LoadFontAsync(string name, DynamicFont fnt)
		{
			fnt.Completed += (IResource r) => AddDependency(name, fnt);
			host.Resources.LoadAsync(fnt);
		}

		/// <summary>
		/// Gets a font from a file path.
		/// </summary>
		/// <param name="path">Font ttf file path.</param>
		/// <returns>A font resource.</returns>
		public DynamicFont GetFont(string name, string path, int size, bool outline = false, bool async = false)
		{
            TrueTypeFont fontFile = new TrueTypeFont(path);

			DynamicFont fnt = new DynamicFont(fontFile, size, outline);

			if (async)
				LoadFontAsync(name, fnt);
			else
			{
                fnt.Aborted += (IResource res, Exception e) => throw e;
				LoadFont(name, fnt);
			}

            return fnt;
		}
	}
}
