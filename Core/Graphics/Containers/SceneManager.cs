using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Timing.Base;

using pEngine.Core.Graphics.Renderer;

namespace pEngine.Core.Graphics.Containers
{
    public class SceneManager : IDisposable
    {
		pEngine gameHost;

		/// <summary>
		/// Makes a new <see cref="SceneManager"/>.
		/// This class will manage transition between scenes.
		/// </summary>
		public SceneManager(pEngine host)
		{
			gameHost = host;
			loadedScenes = new Stack<Scene>();
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
			foreach (var scene in loadedScenes)
			{
				scene.Dispose();
			}
		}

		/// <summary>
		/// Stack of scene calls.
		/// </summary>
		private Stack<Scene> loadedScenes;

		/// <summary>
		/// Current scene path.
		/// </summary>
		public IEnumerable<Scene> Scenes => loadedScenes;

		#region Scene change

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newScene"></param>
		/// <param name="waitTransition"></param>
		public void ReplaceScene(Scene newScene, bool waitTransition = true)
		{
			Action replace = () =>
			{
				if (loadedScenes.Count > 0)
					loadedScenes.Pop();
				loadedScenes.Push(newScene);
			};

			if (waitTransition)
			{
				// TODO: Manage transitions.
				gameHost.PhysicsLoop.Scheduler.Add(replace);
			}
			else
				gameHost.PhysicsLoop.Scheduler.Add(replace);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="newScene"></param>
		/// <param name="waitTransition"></param>
		public void PullScene(Scene newScene, bool waitTransition = true)
		{
			Action push = () =>
			{
				loadedScenes.Push(newScene);
			};

			if (waitTransition)
			{
				// TODO: Manage transitions.
				gameHost.PhysicsLoop.Scheduler.Add(push);
			}
			else
				gameHost.PhysicsLoop.Scheduler.Add(push);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="waitTransition"></param>
		public void PopScene(bool waitTransition = true)
		{
			Action pop = () =>
			{
				loadedScenes.Pop();
			};

			if (waitTransition)
			{
				// TODO: Manage transitions.
				gameHost.PhysicsLoop.Scheduler.Add(pop);
			}
			else
				gameHost.PhysicsLoop.Scheduler.Add(pop);
		}

		#endregion

		#region Per frame methods

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			foreach (var scene in loadedScenes)
			{
				scene.Invalidate(type, propagation, sender);
			}
		}

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{
			foreach (var scene in loadedScenes)
			{
				scene.Update(clock);
			}
		}

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		public virtual IEnumerable<Asset> GetAssets()
		{
			List<Asset> assets = new List<Asset>();

			foreach (var scene in loadedScenes)
			{
				assets.AddRange(scene.GetAssets());
			}

			return assets;
		}

		#endregion

	}
}
