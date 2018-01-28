using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Graphics.Containers;

namespace pEngine
{
	public abstract class Game : Container<Layer>
	{
		/// <summary>
		/// Makes a new instance of <see cref="Game"/> class.
		/// </summary>
		public Game()
		{

		}

		protected override void OnLoad()
		{

		}

	}
}
