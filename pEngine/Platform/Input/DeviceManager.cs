using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using pEngine.Utils.Timing.Base;

using pEngine.Utils.Timing;

namespace pEngine.Platform.Input
{
    public abstract class DeviceManager : IUpdatable
    {
		/// <summary>
		/// Makes a new instance of <see cref="DeviceManager"/> class.
		/// </summary>
		public DeviceManager()
		{

		}

		/// <summary>
		/// Initialize all input devices.
		/// </summary>
		public virtual void Initialize()
		{
			foreach (IDevice device in Devices)
			{
				device.Initialize();
			}
		}

		#region Device management

		/// <summary>
		/// Lists all connected input devices.
		/// </summary>
		public abstract IEnumerable<IDevice> Devices { get; }

		/// <summary>
		/// Gets the default keyboard.
		/// </summary>
		public IKeyboard Keyboard => Devices.Where(x => x is IKeyboard).FirstOrDefault() as IKeyboard;

		/// <summary>
		/// Gets the default mouse.
		/// </summary>
		public IMouse Mouse => Devices.Where(x => x is IMouse).FirstOrDefault() as IMouse;

		/// <summary>
		/// Gets all connected joypads.
		/// </summary>
		public IEnumerable<IJoypad> Joypads => Devices.Where(x => x is IJoypad).Select(x => x as IJoypad);

		/// <summary>
		/// Triggered on a joypad connection or disconnection.
		/// </summary>
		public event EventHandler<JoypadConnectionEventArgs> JoypadConnection;

		/// <summary>
		/// Gets the list of connetted joypad.
		/// </summary>
		/// <returns>A list of joypads.</returns>
		protected abstract IJoypad[] GetConnectedJoypads();

		protected void JoypadConnectionEvent(IJoypad j, bool connected)
		{
			JoypadConnection?.Invoke(this, new JoypadConnectionEventArgs
			{
				Connected = connected,
				Joypad = j
			});
		}

		#endregion

		#region Update

		/// <summary>
		/// Updates the device list.
		/// </summary>
		public virtual void Update(IFrameBasedClock clock)
		{
			// - Update all devices
			foreach(IDevice device in Devices)
			{
				device.Update(clock);
			}
		}

		#endregion
	}

	public class JoypadConnectionEventArgs : EventArgs
	{
		/// <summary>
		/// Target joypad.
		/// </summary>
		public IJoypad Joypad { get; set; }

		/// <summary>
		/// If true this is a connection event otherwise i'ts a disconnection event.
		/// </summary>
		public bool Connected { get; set; }
	}
}
