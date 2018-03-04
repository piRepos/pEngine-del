using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

using pEngine.Timing;
using pEngine.Timing.Base;

namespace pEngine.Input.Virtual
{
	public class VirtualJoypad : IJoypad
	{
		/// <summary>
		/// Makes a new instance of <see cref="GlfwKeyboard"/> class.
		/// </summary>
		/// <param name="state">Current input state.</param>
		internal VirtualJoypad(InputState state, int index, string name)
		{
			currentState = state;
			Index = index;
			Name = name;
		}

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public void Initialize()
		{

		}

		/// <summary>
		/// Current input state.
		/// </summary>
		private InputState currentState { get; set; }

		/// <summary>
		/// Current input thread time.
		/// </summary>
		public double InputTime => currentState.Time;

		/// <summary>
		/// Sets the new current state.
		/// </summary>
		/// <param name="currentState">Input current state.</param>
		internal void UpdateCurrentState(InputState currentState)
		{
			this.currentState = currentState;
		}

		/// <summary>
		/// Joypad device index.
		/// </summary>
		public int Index { get; }

		/// <summary>
		/// Joypad name.
		/// </summary>
		public string Name { get; }

		#region Direct

		/// <summary>
		/// Gets the state for the joypad axys.
		/// </summary>
		public double[] Axes => currentState?.PositionState.Select(x => (double)x.Value).ToArray() ?? new double[0];

		/// <summary>
		/// Gets the state for each joypad button.
		/// </summary>
		public KeyState[] Buttons => currentState?.KeyState.Values.ToArray() ?? new KeyState[0];

		#endregion

		#region Events

		/// <summary>
		/// Triggered on joypad key event.
		/// </summary>
		public event EventHandler<JoypadKeyEventArgs> OnButtonPress;

		/// <summary>
		/// Triggered on axe value change.
		/// </summary>
		public event EventHandler<JoypadAxeEventArgs> OnAxeMovement;

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public void Update(IFrameBasedClock clock)
		{
			while (currentState != null && currentState.Events.Count > 0)
			{
				IInputEvent e = currentState.Events.Dequeue();

				if (e is InputEvent<JoypadKeyEventArgs> keyEvent && keyEvent.Name == "OnButtonPress")
					OnButtonPress?.Invoke(this, keyEvent.Info);

				if (e is InputEvent<JoypadAxeEventArgs> modEvent && modEvent.Name == "OnAxeMovement")
					OnAxeMovement?.Invoke(this, modEvent.Info);
			}
		}
	}
}
