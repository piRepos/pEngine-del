using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Timing;

using pEngine.Framework;
using pEngine.Framework.Modules;
using pEngine.Timing.Base;

namespace pEngine.Games
{
	public class GameTreeService : Service
	{
		/// <summary>
		/// Makes a new instance of <see cref="GameTreeService"/> class.
		/// </summary>
		/// <param name="module">Parent module.</param>
		internal GameTreeService(GameTree module, GameLoop runLoop)
			: base(module, runLoop)
		{

		}

		/// <summary>
		/// Gets the running game.
		/// </summary>
		[ServiceProperty("RunningGame")]
		public Game RunningGame { get; private set; }

	}


	public class GameTree : Module
	{
		/// <summary>
		/// Makes a new instance of <see cref="GameTree"/> class.
		/// </summary>
		/// <param name="host">Parent game host.</param>
		public GameTree(GameHost host, GameLoop moduleLoop)
			: base(host, moduleLoop)
		{
			services = new Dictionary<Type, Service>();
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		/// <summary>
		/// Initialize this module.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// - Add the first service
			AddService(GetSettings(ModuleLoop));

			// - Load game
			RunningGame?.Load(this);
		}

		#region Services injection

		/// <summary>
		/// Gets the loaded services.
		/// </summary>
		private Dictionary<Type, Service> services { get; }

		/// <summary>
		/// Gets the loaded services.
		/// </summary>
		public IEnumerable<Service> LoadedServices => services.Values;

		/// <summary>
		/// Inserts a service in the public game services.
		/// </summary>
		/// <param name="s">Service to add.</param>
		public void AddService(Service s)
		{
			services.Add(s.GetType(), s);
		}

		/// <summary>
		/// Gets a registered service.
		/// </summary>
		/// <typeparam name="T">Service type.</typeparam>
		public T GetService<T>() where T : Service
		{
			return GetService(typeof(T)) as T;
		}

		/// <summary>
		/// Gets a registered service.
		/// </summary>
		public Service GetService(Type s)
		{
			try
			{
				return services[s];
			}
			catch (KeyNotFoundException)
			{
				throw new ServiceNotFoundException($"Service: '{s.Name}' was not found.");
			}
		}

		#endregion

		#region Game management

		/// <summary>
		/// Gets the running game.
		/// </summary>
		public Game RunningGame { get; private set; }

		/// <summary>
		/// Test the running game.
		/// </summary>
		/// <param name="game">Game to run.</param>
		internal void SetRunningGame(Game game)
		{
			if (game is null)
				throw new ArgumentNullException("Game can't be null.");

			RunningGame = game;
		}

		/// <summary>
		/// Close the running game.
		/// </summary>
		public bool CloseGame() => RunningGame?.OnClose() ?? true;

		#endregion

		#region Service

		/// <summary>
		/// Settings for this module.
		/// </summary>
		public override Service GetSettings(GameLoop mainLoop)
		{
			GameTreeService service = new GameTreeService(this, mainLoop);
			service.Initialize();
			Services.Add(service);
			return service;
		}

		#endregion

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			RunningGame?.Update(clock);
		}
	}
}
