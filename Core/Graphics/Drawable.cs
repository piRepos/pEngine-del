using System;
using System.Text;
using System.Collections.Generic;

using pEngine.Common.Timing.Base;

using pEngine.Core.Graphics.Renderer.FrameBuffering;
using pEngine.Core.Graphics.Renderer.Clipping;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Physics;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics
{
    public class Drawable : Movable
    {
		/// <summary>
		/// Makes a new instance of <see cref="Drawable"/>.
		/// </summary>
		public Drawable()
			: base()
		{
			invalidationId = long.MaxValue;
			Visible = true;
			Opacity = 1;
		}

		#region Visibility

		/// <summary>
		/// Gets or sets the clipping settings.
		/// </summary>
		public ClippingInformations Clipping { get; set; }

		/// <summary>
		/// Gets or sets if this object is visible.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the object opacity.
		/// </summary>
		public double Opacity { get; set; }

		/// <summary>
		/// Gets or sets the depth order relative 
		/// to parent's children
		/// </summary>
		public int zIndex { get; set; }

		#endregion

		#region Assets

		private List<Asset> assetCache;

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		public override IEnumerable<Asset> GetAssets()
		{
			if (assetCache == null)
			{
				assetCache = new List<Asset>();
				assetCache.AddRange(CalculateAssets());

				if (FrameBuffered)
				{

				}
			}

			return assetCache;
		}

		protected virtual List<Asset> CalculateAssets()
		{
			return new List<Asset>();
		}

		#endregion

		#region Update logic

		private long invalidationId;

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			if (State == LoadState.Loaded && Host.PhysicsLoop.FrameId > invalidationId)
			{
				assetCache = null;
				invalidationId = long.MaxValue;
			}
		}

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public override void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			if (State == LoadState.Loaded && type.HasFlag(InvalidationType.Assets) && invalidationId == long.MaxValue)
				invalidationId = Host.PhysicsLoop.FrameId;

			base.Invalidate(type, propagation, sender);
		}

		#endregion

		#region Loading

		/// <summary>
		/// Loads the game object async.
		/// </summary>
		/// <returns>The loader task.</returns>
		/// <param name="callback">Callback to call when loading is done.</param>
		public Drawable LoadAsync(Game game, Action callback)
		{
			return LoadAsync<Drawable>(game, callback);
		}

		/// <summary>
		/// Loads the game object sync.
		/// </summary>
		public Drawable Load(Game game)
		{
			return Load<Drawable>(game);
		}

		#endregion

		#region Frame Buffering

		/// <summary>
		/// If true this object's pixels will stored in a
		/// frame buffer, and will be renderized the buffer.
		/// </summary>
		public bool FrameBuffered { get; set; }



		#endregion

	}
}
