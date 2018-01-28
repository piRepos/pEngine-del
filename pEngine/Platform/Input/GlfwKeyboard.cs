using System;
using System.Collections.Generic;
using System.Text;

using Glfw3;

using pEngine.Utils.Timing.Base;
using pEngine.Platform.Forms;

namespace pEngine.Platform.Input
{
    public class GlfwKeyboard : IKeyboard
    {
        Glfw.KeyFunc keyCallback;
        Glfw.CharFunc charCallback;
        Glfw.CharModsFunc charModsCallback;

		/// <summary>
		/// Makes a new instance of <see cref="GlfwKeyboard"/> class.
		/// </summary>
		/// <param name="window">Current context.</param>
		public GlfwKeyboard(GlfwWindow window)
		{
			handler = window;

            keyCallback = (w, key, scancode, state, modifiers) =>
            {
                OnKeyEvent?.Invoke((KeyboardKey)key, scancode, (KeyState)state, (KeyModifier)modifiers);
            };

            charCallback = (w, c) =>
            {
                OnType?.Invoke((char)c);
            };

            charModsCallback = (w, c, modifiers) =>
            {
                OnTypeWithMods?.Invoke((char)c, (KeyModifier)modifiers);
            };
		}

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public void Initialize()
		{
            Glfw.SetKeyCallback(handler.Handle, keyCallback);
            Glfw.SetCharCallback(handler.Handle, charCallback);
            Glfw.SetCharModsCallback(handler.Handle, charModsCallback);
		}

		/// <summary>
		/// Private window handler.
		/// </summary>
		private GlfwWindow handler { get; }

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
				bool ctrl = GetButtonState(KeyboardKey.LeftControl) || GetButtonState(KeyboardKey.RightControl);
				bool alt = GetButtonState(KeyboardKey.LeftAlt) || GetButtonState(KeyboardKey.RightAlt);
				bool super = GetButtonState(KeyboardKey.LeftSuper) || GetButtonState(KeyboardKey.RightSuper);
				bool shift = GetButtonState(KeyboardKey.LeftShift) || GetButtonState(KeyboardKey.RightShift);

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
		public event KeyboardKeyEventHandler OnKeyEvent;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		public event KeyboardTypeEventHandler OnType;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		public event KeyboardModTypeEventHandler OnTypeWithMods;

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public void Update(IFrameBasedClock Clock)
		{
			
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		public bool GetButtonState(KeyboardKey key)
		{
			return Glfw.GetKey(handler.Handle, (int)key);
		}

	}
}
