using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

using pEngine.Utils.Timing.Base;

namespace pEngine.Input.Virtual
{
	public class VirtualMouse : IMouse
	{
		/// <summary>
		/// Makes a new instance of <see cref="VirtualMouse"/> class.
		/// </summary>
		/// <param name="state">Current input state.</param>
		internal VirtualMouse(InputState state)
		{
			currentState = state;
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
		/// Sets the new current state.
		/// </summary>
		/// <param name="currentState">Input current state.</param>
		internal void UpdateCurrentState(InputState currentState)
		{
			this.currentState = currentState;
		}

		/// <summary>
		/// Mouse position.
		/// </summary>
		public Vector2 Position => new Vector2(currentState.GetPositionState(0), currentState.GetPositionState(1));

		#region Buttons

		/// <summary>
		/// Current input thread time.
		/// </summary>
		public double InputTime => currentState.Time;

		/// <summary>
		/// Mouse left button state (true if pressed).
		/// </summary>
		public bool LeftButton => IsPressed(MouseButton.ScrollButton);

		/// <summary>
		/// Mouse right button state (true if pressed).
		/// </summary>
		public bool RightButton => IsPressed(MouseButton.RightButton);

		/// <summary>
		/// Mouse center button state (true if pressed).
		/// </summary>
		public bool ScrollButton => IsPressed(MouseButton.ScrollButton);
		
		#endregion

		#region Events

		/// <summary>
		/// Triggered on mouse move.
		/// </summary>
		public event EventHandler<MousePositiontEventArgs> OnMove;

		/// <summary>
		/// Triggered on mouse scroll movement.
		/// </summary>
		public event EventHandler<MouseOffsetEventArgs> OnScroll;

		/// <summary>
		/// Triggered on mouse button event.
		/// </summary>
		public event EventHandler<MouseKeyEventArgs> OnButtonEvent;

		#endregion

		public void Update(IFrameBasedClock clock)
		{
			while (currentState != null && currentState.Events.Count > 0)
			{
				IInputEvent e = currentState.Events.Dequeue();

				if (e is InputEvent<MouseKeyEventArgs> keyEvent && keyEvent.Name == "OnButtonEvent")
					OnButtonEvent?.Invoke(this, keyEvent.Info);

				if (e is InputEvent<MousePositiontEventArgs> modEvent && modEvent.Name == "OnMove")
					OnMove?.Invoke(this, modEvent.Info);

				if (e is InputEvent<MouseOffsetEventArgs> tyEvent && tyEvent.Name == "OnScroll")
					OnScroll?.Invoke(this, tyEvent.Info);
			}
		}

		/// <summary>
		/// Gets the state of a specified mouse button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>The button state.</returns>
		public KeyState GetButtonState(MouseButton button)
		{
			return currentState.GetKeyState((uint)button);
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		public bool IsPressed(MouseButton key)
		{
			var state = currentState.GetKeyState((uint)key);
			return state == KeyState.Pressed || state == KeyState.Holding;
		}
	}
}
