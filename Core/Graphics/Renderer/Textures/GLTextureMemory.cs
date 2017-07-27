using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Graphics.Renderer.Textures
{
    public class GLTextureMemory
    {

		/// <summary>
		/// Creates a new instance of <see cref="GLTextureMemory"/>.
		/// </summary>
		public GLTextureMemory()
		{
			loadedTextures = new Dictionary<long, GLTexture>();
		}

		/// <summary>
		/// Initialize OpenGL handlers.
		/// </summary>
		public void Initialize()
		{

		}


		#region Resolver

		public Dictionary<long, GLTexture> loadedTextures;

		/// <summary>
		/// Gets the opengl reference to a logical texture.
		/// </summary>
		/// <param name="textureID">Logical texture id.</param>
		/// <returns>OpenGL handler.</returns>
		public GLTexture ResolveTexture(long textureID)
		{
			// TODO: handle exceptions.
			return loadedTextures[textureID];
		}

		#endregion

		#region Loader

		/// <summary>
		/// Loads a texture in the VRAM.
		/// </summary>
		/// <param name="tex">Texture desciptor to load.</param>
		public void LoadTexture(TextureDescriptor tex)
		{
			GLTexture texture;
			if (!loadedTextures.ContainsKey(tex.DescriptorID))
				texture = new GLTexture();
			else texture = loadedTextures[tex.DescriptorID];

			texture.MipmapLevel = tex.Mipmaps;

			if (tex.Buffer == null)
				return;

			if (tex.GenerateMipmaps)
			{
				texture.Upload(tex.Buffer[0]);
				texture.GenerateMipmaps();
			}
			else texture.Upload(tex.Buffer);

			if (!loadedTextures.ContainsKey(tex.DescriptorID))
				loadedTextures.Add(tex.DescriptorID, texture);
		}

		#endregion

	}
}
