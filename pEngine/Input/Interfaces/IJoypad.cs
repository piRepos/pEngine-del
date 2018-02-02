using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Input
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

	public class JoypadKeyEventArgs : EventArgs
	{
		/// <summary>
		/// Target key.
		/// </summary>
		public int Key { get; set; }

		/// <summary>
		/// Key current state.
		/// </summary>
		public KeyState Action { get; set; }
	}

	public class JoypadAxeEventArgs : EventArgs
	{
		/// <summary>
		/// Target key.
		/// </summary>
		public int Axe { get; set; }

		/// <summary>
		/// Key current state.
		/// </summary>
		public double Value { get; set; }
	}
}
