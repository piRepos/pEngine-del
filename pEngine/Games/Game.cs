using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pEngine.Utils.Threading;

namespace pEngine.Games
{
	public abstract class Game : Container<Layer>
	{
		/// <summary>
		/// Makes a new instance of <see cref="Game"/> class.
		/// </summary>
		public Game()
		{

		}

		#region Management

		/// <summary>
		/// Invoked on game close event.
		/// </summary>
		/// <returns>If game should close.</returns>
		public virtual bool OnClose()
		{
			// - Default close game
			return true;
		}

		#endregion

		#region Game load override

		/// <summary>
		/// Game tree manager service.
		/// </summary>
		public GameTree GameTree { get; private set; }

		/// <summary>
		/// Loads this game.
		/// </summary>
		/// <param name="loader">pEngine GameTree module.</param>
		internal void Load(GameTree loader)
		{
			GameTree = loader;
			base.Load(loader.ModuleLoop.Scheduler);
		}

		#endregion
	}
}
