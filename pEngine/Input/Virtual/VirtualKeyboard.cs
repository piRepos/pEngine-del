using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Platform.Input;

using pEngine.Utils.Timing.Base;

namespace pEngine.Input.Virtual
{
	public class VirtualKeyboard : IKeyboard
	{
		/// <summary>
		/// Makes a new instance of <see cref="GlfwKeyboard"/> class.
		/// </summary>
		/// <param name="state">Current input state.</param>
		internal VirtualKeyboard(InputState state)
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

		#region Modifiers

		/// <summary>
		/// True if caps lock is enabled.
		/// </summary>
		public bool CapsLock => throw new NotImplementedException("Cant get this information.");

		/// <summary>
		/// True if num lock is enabled.
		/// </summary>
		public bool NumLock => throw new NotImplementedException("Cant get this information.");

		/// <summary>
		/// True if scroll lock is enabled.
		/// </summary>
		public bool ScrollLock => throw new NotImplementedException("Cant get this information.");

		/// <summary>
		/// Enabled key modifiers.
		/// </summary>
		public KeyModifier Modifiers
		{
			get
			{
				bool ctrl = IsPressed(KeyboardKey.LeftControl) || IsPressed(KeyboardKey.RightControl);
				bool alt = IsPressed(KeyboardKey.LeftAlt) || IsPressed(KeyboardKey.RightAlt);
				bool super = IsPressed(KeyboardKey.LeftSuper) || IsPressed(KeyboardKey.RightSuper);
				bool shift = IsPressed(KeyboardKey.LeftShift) || IsPressed(KeyboardKey.RightShift);

				KeyModifier m = KeyModifier.None;

				if (ctrl) m |= KeyModifier.Control;
				if (alt) m |= KeyModifier.Alt;
				if (super) m |= KeyModifier.Super;
				if (shift) m |= KeyModifier.Shift;

				return m;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Triggered on keyboard key event.
		/// </summary>
		public event EventHandler<KeyboardKeyEventArgs> OnKeyEvent;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		public event EventHandler<KeyboardTypeEventArgs> OnType;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		public event EventHandler<KeyboardModTypeEventArgs> OnTypeWithMods;

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public void Update(IFrameBasedClock clock)
		{
			while(currentState != null && currentState.Events.Count > 0)
			{
				IInputEvent e = currentState.Events.Dequeue();

				if (e is InputEvent<KeyboardKeyEventArgs> keyEvent && keyEvent.Name == "OnKeyEvent")
					OnKeyEvent?.Invoke(this, keyEvent.Info);

				if (e is InputEvent<KeyboardModTypeEventArgs> modEvent && modEvent.Name == "OnModType")
					OnTypeWithMods?.Invoke(this, modEvent.Info);

				if (e is InputEvent<KeyboardTypeEventArgs> tyEvent && tyEvent.Name == "OnType")
					OnType?.Invoke(this, tyEvent.Info);
			}
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>The button state.</returns>
		public KeyState GetButtonState(KeyboardKey key)
		{
			return currentState?.GetKeyState((uint)key) ?? KeyState.Unknow;
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		public bool IsPressed(KeyboardKey key)
		{
			var state = (currentState?.GetKeyState((uint)key) ?? KeyState.Unknow);
			return state == KeyState.Pressed || state == KeyState.Holding;
		}
	}
}
