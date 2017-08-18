using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Data.FrameDependency;
using pEngine.Core.Graphics.Renderer.Textures;

using pEngine.Common.Math;

namespace pEngine.Core.Graphics.Textures
{
    public interface ITexture : ISpaced, ISized, INotifyPropertyChanged, IDependencyReference
	{

		/// <summary>
		/// Texture relative size (value from 0 to 1).
		/// </summary>
		Vector2 RasterSize { get; }

		/// <summary>
		/// Texture relative position (value from 0 to 1).
		/// </summary>
		Vector2 RasterPosition { get; }

		/// <summary>
		/// Top left corner texture position.
		/// </summary>
		Vector2 TopLeft { get; }

		/// <summary>
		/// Top right corner texture position.
		/// </summary>
		Vector2 TopRight { get; }

		/// <summary>
		/// Bottom right corner texture position.
		/// </summary>
		Vector2 BotRight { get; }

		/// <summary>
		/// Bottom left corner texture position.
		/// </summary>
		Vector2 BotLeft { get; }

	}
}
