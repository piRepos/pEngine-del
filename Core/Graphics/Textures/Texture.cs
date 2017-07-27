using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data;
using pEngine.Core.Data.FrameDependency;

using pEngine.Common.Memory;

namespace pEngine.Core.Graphics.Renderer.Textures
{
    public class Texture : Resource, IDependency<TextureDescriptor>
    {

		public Texture(PixelBuffer rawImage)
		{

		}

		public Texture(Bitmap bitmapImage)
		{

		}

		/// <summary>
		/// Texture image raw pixels.
		/// </summary>
		PixelBuffer Pixels { get; }

		#region Loading

		internal override void Complete()
		{
			base.Complete();
		}

		internal override bool Abort(Exception e)
		{
			base.Abort(e);

			return true;
		}

		internal override void Load()
		{
			//throw new InvalidOperationException("Cazzo");
		}

		#endregion

		#region Dependency

		/// <summary>
		/// Generate a descriptor for this dependency.
		/// This descriptor will be used as frame resource.
		/// </summary>
		/// <returns>The dependency descriptor.</returns>
		public TextureDescriptor GetDescriptor()
		{
			return new TextureDescriptor
			{

			};
		}

		/// <summary>
		/// True if the resource is changed.
		/// </summary>
		public bool Invalidated { get; set; }

		/// <summary>
		/// Dependency identifier.
		/// </summary>
		public long DependencyID { get; set; }

		#endregion

	}
}
