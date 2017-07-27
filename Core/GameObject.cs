using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using pEngine.Common.Timing.Base;
using pEngine.Common.Diagnostic;
using pEngine.Common.DataModel;
using pEngine.Common;

using pEngine.Core.Graphics.Renderer;

using pEngine.Debug;

namespace pEngine.Core
{
    public abstract class GameObject : NotifyPropertyChanged, IGameObject, IStateObject, IDisposable
    {
		private static long objectCount = 0;

		/// <summary>
		/// Makes a new instance of <see cref="GameObject"/>.
		/// </summary>
		public GameObject()
		{
			ObjectId = objectCount++;
			CreationTime = DateTime.Now;

			State = LoadState.NotLoaded;
			PerformanceMonitor = new PerformanceCollector(ToString());
		}

		/// <summary>
		/// Releases all resource used by the <see cref="GameObject"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="GameObject"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="GameObject"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="GameObject"/> so
		/// the garbage collector can reclaim the memory that the <see cref="GameObject"/> was occupying.</remarks>
		public virtual void Dispose()
		{

		}

		#region Metadata

		/// <summary>
		/// Object identifier.
		/// </summary>
		public long ObjectId { get; }

		/// <summary>
		/// Object creation time.
		/// </summary>
		public DateTime CreationTime { get; }

		/// <summary>
		/// How much time this object is alive.
		/// </summary>
		public TimeSpan LifeTime => DateTime.Now - CreationTime;

		#endregion

		#region Hierarchy

		/// <summary>
		/// Parent node in game object tree.
		/// </summary>
		public IGameObject Parent { get; internal set; }

		/// <summary>
		/// Return the tree root.
		/// </summary>
		protected Game Game { get; private set; }

		/// <summary>
		/// Game host (the engine which is running this game).
		/// </summary>
		protected pEngine Host => Game.Host;

		#endregion

		#region State

		/// <summary>
		/// Object load state.
		/// </summary>
		public LoadState State { get; internal set; }

		/// <summary>
		/// True if this object is loaded.
		/// </summary>
		public bool IsLoaded => State == LoadState.Loaded || State == LoadState.Loaded;

		/// <summary>
		/// Triggered on object load.
		/// </summary>
		public event EventHandler OnLoad;

		/// <summary>
		/// Loads the game object async.
		/// </summary>
		/// <returns>The loader task.</returns>
		/// <param name="callback">Callback to call when loading is done.</param>
		public T LoadAsync<T>(Game game, Action callback) where T : GameObject
		{
			Task taskLoader = Task.Run(() =>
			{
				Load<T>(game);

			}).ContinueWith((task) =>
			{
				Host.PhysicsLoop.Scheduler.Add(() =>
				{
					callback?.Invoke();
				});
			});

			return this as T;
		}

		/// <summary>
		/// Loads the game object sync.
		/// </summary>
		public T Load<T>(Game game) where T : GameObject
		{
			lock (this)
			{
				if (IsLoaded) return this as T;

				Game = game;

				State = LoadState.Loading;

				using (PerformanceMonitor.StartCollect("Loading"))
					Host.Loader.LoadSync(this);

				State = LoadState.Loaded;

				// - Invalidate current object
				Invalidate(InvalidationType.All, InvalidationDirection.Children, this);

				OnLoad?.Invoke(this, EventArgs.Empty);

				bool physicsThread = Host.PhysicsLoop.ImOnThisThread;
				if (physicsThread && PerformanceMonitor["Loading"]?.Max.Milliseconds > 200)
					Debug.WarningLog($@"[{ToString()}] has take {PerformanceMonitor["Loading"].Max.Milliseconds:00.000} seconds to load sync.");
			}

			return this as T;
		}

		#endregion

		#region Diagnostic

		/// <summary>
		/// Game debug module.
		/// </summary>
		protected DebugModule Debug => Host.Debug;

		/// <summary>
		/// Collect performances of this object.
		/// </summary>
		public PerformanceCollector PerformanceMonitor { get; }

		#endregion

		#region Invalidation

		/// <summary>
		/// Invalidate a property of this object/tree.
		/// </summary>
		/// <param name="type">Property to invalidate.</param>
		/// <param name="propagation">Propagation direction.</param>
		/// <param name="sender">Object sender.</param>
		public virtual void Invalidate(InvalidationType type, InvalidationDirection propagation, IGameObject sender)
		{
			if (propagation.HasFlag(InvalidationDirection.Parent) && Parent != null && !(Parent is Game))
			{
				Parent.Invalidate(type, InvalidationDirection.Parent, sender);
			}
		}

		#endregion

		#region .Net overrides

		/// <summary>
		/// Check if two object are equals.
		/// </summary>
		/// <param name="obj">Second object.</param>
		/// <returns>True if are equals.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (GetType() != obj.GetType())
				return false;

			GameObject o = obj as GameObject;

			if (o.ObjectId == ObjectId)
				return true;

			return false;
		}

		/// <summary>
		/// String rappresentation of a gameobject.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return base.ToString() + $"#{ObjectId}";
		}

		/// <summary>
		/// Get an hash code for game objects.
		/// </summary>
		/// <returns>Hash for this object.</returns>
		public override int GetHashCode()
		{
			return (int)checked(ObjectId % int.MaxValue);
		}

		#endregion

		/// <summary>
		/// Update this object physics.
		/// </summary>
		/// <param name="clock">Gameloop clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{

		}

		/// <summary>
		/// Calculate the assets which can be rendered.
		/// </summary>
		/// <returns>The assets.</returns>
		public virtual IEnumerable<Asset> GetAssets()
		{
			return null;
		}

	}
}
