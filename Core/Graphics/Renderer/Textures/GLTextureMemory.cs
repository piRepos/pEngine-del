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

		#region Descriptor dispatching

		/// <summary>
		/// Loads a texture in the VRAM.
		/// </summary>
		/// <param name="descriptor">Texture desciptor to load.</param>
		public void DispatchTexture(TextureDescriptor descriptor)
		{
			switch (descriptor.State)
			{
				case Data.FrameDependency.DependencyState.NotLoaded:
					LoadTexture(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Modified:
					UpdateTexture(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Disposed:
					DisposeTexture(descriptor);
					break;
				case Data.FrameDependency.DependencyState.Loaded:
					return;
			}
		}

		#endregion

		#region Load

		private void LoadTexture(TextureDescriptor info)
		{
            if (!loadedTextures.ContainsKey(info.DescriptorID))
            {
                GLTexture texture = new GLTexture();

                texture.MipmapLevel = info.Mipmaps;

                if (info.Buffer == null)
                    return;

                if (info.GenerateMipmaps)
                {
                    texture.Upload(info.Buffer[0]);
                    texture.GenerateMipmaps();
                }
                else texture.Upload(info.Buffer);

                if (!loadedTextures.ContainsKey(info.DescriptorID))
                    loadedTextures.Add(info.DescriptorID, texture);
            }
		}

        #endregion

        #region Update

        private void UpdateTexture(TextureDescriptor info)
        {
            if (loadedTextures.ContainsKey(info.DescriptorID))
            {
                GLTexture texture = new GLTexture();

                texture.MipmapLevel = info.Mipmaps;

                if (info.Buffer == null)
                    return;

                if (info.GenerateMipmaps)
                {
                    texture.Upload(info.Buffer[0]);
                    texture.GenerateMipmaps();
                }
                else texture.Upload(info.Buffer);


                if (!loadedTextures.ContainsKey(info.DescriptorID))
                    loadedTextures.Add(info.DescriptorID, texture);

                var oldTexture = loadedTextures[info.DescriptorID];
                oldTexture.Dispose();

                loadedTextures[info.DescriptorID] = texture;
            }
        }

		#endregion

		#region Dispose

		private void DisposeTexture(TextureDescriptor info)
		{
			if (loadedTextures.ContainsKey(info.DescriptorID))
			{
				var oldTexture = loadedTextures[info.DescriptorID];
				oldTexture.Dispose();

				loadedTextures.Remove(info.DescriptorID);
			}
		}

		#endregion
	}
}
