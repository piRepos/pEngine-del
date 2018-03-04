using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Framework;
using pEngine.Games;
using pEngine.Platform;

namespace pEngine.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			// - Initialize the engine environment
			Platform.Environment.Initialize();

			bool primary = Platform.Environment.FirstInstance;

			// - Make a new gamehost
			using (GameHost host = new GameHost())
			{
				// - Make a new test game
				Game game = new TestGame();

				// - Run the test game
				host.Run(game);
			}
		}
	}
}
