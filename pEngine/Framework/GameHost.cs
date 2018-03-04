using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform;
using pEngine.Platform.Input;
using pEngine.Platform.Forms;

using pEngine.Games;

using pEngine.Utils.Threading;
using pEngine.Timing.Base;
using pEngine.Timing;

using pEngine.Input;

using pEngine.Context;


namespace pEngine.Framework
{
	public partial class GameHost : pObject
	{
		/// <summary>
		/// Makes a new instance of <see cref="GameHost"/> class.
		/// </summary>
		public GameHost()
		{
			// - Initialize environment if not initialized
			pEngine.Platform.Environment.Initialize();

			// - Make game loops
			InputGameLoop = new GameLoop(HandleInput, InputInitialization, "InputThread");
			PhysicsGameLoop = new ThreadedGameLoop(HandlePhysics, PhysicsInitialization, "PhysicsThread");

			// - Modules
			Input = new InputEngine(this, InputGameLoop);
			GameTree = new GameTree(this, PhysicsGameLoop);
			Windows = new WindowsProvider(this, InputGameLoop);
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Input.Dispose();
				GameTree.Dispose();
				Windows.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Running platform.
		/// </summary>
		public IPlatform Platform => pEngine.Platform.Environment.Platform;

		/// <summary>
		/// Game window.
		/// </summary>
		public IPlatformWindow Window => Windows.MainContext;

		#region Windows

		/// <summary>
		/// Windows manager module.
		/// </summary>
		public WindowsProvider Windows { get; }

		#endregion

		#region Game management

		/// <summary>
		/// Run the specified game.
		/// </summary>
		/// <param name="game">Game to run.</param>
		public virtual void Run(Game game)
		{
			GameTree.SetRunningGame(game);

			// - Initialize windows manager
			Windows.Initialize();

			// - Make a new window
			Window.Make();
			Window.Show();

			// - Initialize input apis
			Input.Initialize();

			// - Add services
			GameTree.AddService(Input.GetSettings(PhysicsGameLoop));
			GameTree.AddService(Windows.GetSettings(PhysicsGameLoop));

			// - Load the game
			GameTree.Initialize();

			// - Start gameloops
			PhysicsGameLoop.Run();
			InputGameLoop.Run();
		}

		/// <summary>
		/// Close the running game.
		/// </summary>
		public virtual void Close()
		{
			if (!GameTree.CloseGame())
				return;

			PhysicsGameLoop.Stop();
			InputGameLoop.Stop();
		}

		#endregion

		#region Input thread

		/// <summary>
		/// Game loop for input handling.
		/// </summary>
		public GameLoop InputGameLoop { get; }

		/// <summary>
		/// The input engine.
		/// </summary>
		public InputEngine Input { get; }

		/// <summary>
		/// Initialize input modules.
		/// </summary>
		protected virtual void InputInitialization()
		{

		}

		/// <summary>
		/// Handle an input frame.
		/// </summary>
		/// <param name="clock">Timed clock.</param>
		protected virtual void HandleInput(IFrameBasedClock clock)
		{
			if (Window.ShouldClose)
				Close();

			Window.PollMesages(true);

			// - Update windows manager
			Windows.Update(clock);

			// - Update input engine
			Input.Update(clock);
		}

		#endregion

		#region Physics thread

		/// <summary>
		/// Game loop for physics handling.
		/// </summary>
		public GameLoop PhysicsGameLoop { get; }

		/// <summary>
		/// Manages the game tree.
		/// </summary>
		public GameTree GameTree { get; }

		/// <summary>
		/// Initialize physics modules.
		/// </summary>
		protected virtual void PhysicsInitialization()
		{

		}

		/// <summary>
		/// Handle a physics calculation frame.
		/// </summary>
		/// <param name="clock">Timed clock.</param>
		protected virtual void HandlePhysics(IFrameBasedClock clock)
		{
			// - Update game
			GameTree.Update(clock);
		}

		#endregion

	}
}
