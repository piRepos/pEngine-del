using System;
using System.Collections.Generic;
using System.Text;

using Glfw3;

using pEngine.Input;

using pEngine.Timing.Base;
using pEngine.Platform.Forms;

namespace pEngine.Platform.Input
{
    public class GlfwKeyboard : IKeyboard
    {
        readonly Glfw.KeyFunc keyCallback;
		readonly Glfw.CharFunc charCallback;
		readonly Glfw.CharModsFunc charModsCallback;

		/// <summary>
		/// Makes a new instance of <see cref="GlfwKeyboard"/> class.
		/// </summary>
		/// <param name="window">Current context.</param>
		public GlfwKeyboard(GlfwWindow window)
		{
			handler = window;

			handler.OnRestore += Handler_OnRestore;

            keyCallback = (w, key, scancode, state, modifiers) =>
            {
                OnKeyEvent?.Invoke(this, new KeyboardKeyEventArgs
				{
					Key = (KeyboardKey)key,
					Scancode = scancode,
					Action = (KeyState)state,
					Modifiers = (KeyModifier)modifiers
				});
            };

            charCallback = (w, c) =>
            {
                OnType?.Invoke(this, new KeyboardTypeEventArgs
				{
					Point = (char)c
				});
            };

            charModsCallback = (w, c, modifiers) =>
            {
                OnTypeWithMods?.Invoke(this, new KeyboardModTypeEventArgs
				{
					Point = (char)c,
					Modifiers = (KeyModifier)modifiers
				});
            };
		}

		private void Handler_OnRestore(object sender, EventArgs e)
		{
			Initialize();
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
		public void Update(IFrameBasedClock Clock)
		{
			
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>The button state.</returns>
		public KeyState GetButtonState(KeyboardKey key)
		{
			return Glfw.GetKey(handler.Handle, (int)key) ? KeyState.Pressed : KeyState.Released;
		}

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		public bool IsPressed(KeyboardKey key)
		{
			return Glfw.GetKey(handler.Handle, (int)key);
		}

	}
}
