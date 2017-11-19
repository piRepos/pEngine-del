using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Input
{

    public class InputManager
    {

		public InputManager(pEngine host)
		{
			Host = host;
		}

		/// <summary>
		/// Parent game host.
		/// </summary>
		protected pEngine Host { get; }

	}
}
