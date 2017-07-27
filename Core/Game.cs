using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Timing.Base;
using pEngine.Common.DataModel;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Containers;
using pEngine.Core.Graphics;

using pEngine.Debug;

namespace pEngine.Core
{
	using LayerManager = SortedDictionary<int, SceneManager>;

	public class Game : IGameObject, IDisposable
	{
		/// <summary>
		/// Makes a new <see cref="Game"/>.
		/// </summary>
		public Game()
		{
			Layers = new LayerManager();
		}

		/// <summary>
		/// This function will loaad all base dependencies
		/// and will initialize the first scene.
		/// </summary>
		/// <param name="debug"></param>
		[LoaderFunction(PermitNullServices = false)]
		private void GameBootstrap(DebugModule debug)
		{
			// - Background layer
			Layers.Add(0, new SceneManager(Host));

			// - Current scene layer
			Layers.Add(int.MaxValue / 2, new SceneManager(Host));
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Game"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Game"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Game"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="Game"/> so
		/// the garbage collector can reclaim the memory that the <see cref="Game"/> was occupying.</remarks>
		public virtual void Dispose()
		{
			foreach (var layer in Layers)
			{
				layer.Value.Dispose();
			}
		}

		#region Host

		/// <summary>
		/// Game host (the engine which is running this game).
		/// </summary>
		public pEngine Host { get; internal set; }

		#endregion

		#region Layers

		/// <summary>
		/// Game graphic layers.
		/// </summary>
		public LayerManager Layers { get; }

		/// <summary>
		/// Background layer scene.
		/// </summary>
		public SceneManager Background => Layers[0];

		/// <summary>
		/// Current game scene.
		/// </summary>
		public SceneManager CurrentScene => Layers[int.MaxValue / 2];

		#endregion

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{
			foreach (var layer in Layers)
			{
				layer.Value.Update(clock);
			}
		}

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public virtual void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			foreach (var layer in Layers)
			{
				layer.Value.Invalidate(type, propagation, sender);
			}
		}

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		public virtual IEnumerable<Asset> GetAssets()
		{
			List<Asset> assets = new List<Asset>();

			foreach (var layer in Layers)
			{
				assets.AddRange(layer.Value.GetAssets());
			}

			return assets;
		}

    }
}
