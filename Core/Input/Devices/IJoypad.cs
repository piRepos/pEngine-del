using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Core.Input
{
    public interface IJoypad : IDevice
    {
		/// <summary>
		/// Joypad device index.
		/// </summary>
		int Index { get; }

		/// <summary>
		/// Joypad name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the state for the joypad axys.
		/// </summary>
		double[] Axes { get; }

		/// <summary>
		/// Gets the state for each joypad button.
		/// </summary>
		KeyState[] Buttons { get; }
    }
}
