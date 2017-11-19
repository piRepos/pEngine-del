using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Timing.Base;

namespace pEngine.Core.Input
{
	/// <summary>
	/// A phisical device interface.
	/// </summary>
    public interface IDevice : IUpdatable
    {
		/// <summary>
		/// Initialize the device.
		/// </summary>
		void Initialize();
    }

	public enum KeyState
	{
		/// <summary>
		/// We can't say in wich state is the key.
		/// </summary>
		Unknow = 0,

		/// <summary>
		/// The key is pressed.
		/// </summary>
		Pressed = 1,

		/// <summary>
		/// The key is released.
		/// </summary>
		Released = 0,

		/// <summary>
		/// Key is in hold state.
		/// </summary>
		Holding = 2
	}
}
