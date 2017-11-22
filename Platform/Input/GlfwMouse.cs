using System;
using System.Collections.Generic;
using System.Text;

using Glfw3;

using pEngine.Common.Timing.Base;
using pEngine.Platform.Windows;
using pEngine.Core.Input;

namespace pEngine.Platform.Input
{
    public class GlfwMouse : IMouse
    {
        Glfw.CursorPosFunc cursorCallback;
        Glfw.CursorPosFunc scrollCallback;
        Glfw.MouseButtonFunc buttonCallback;

		/// <summary>
		/// Makes a new instance of <see cref="GlfwMouse"/> class.
		/// </summary>
		/// <param name="window">Current context.</param>
		public GlfwMouse(GlfwWindow window)
		{
			handler = window;

            cursorCallback = (w, x, y) =>
            {
                OnMove?.Invoke(new Vector2((float)x, (float)y));
            };

            scrollCallback = (w, x, y) =>
            {
                OnScroll?.Invoke(new Vector2((float)x, (float)y));
            };

            buttonCallback = (w, button, action, modifiers) =>
            {
                OnButtonEvent?.Invoke((MouseButton)button, (KeyState)action, (KeyModifier)modifiers);
            };
		}

		/// <summary>
		/// Initialize the device.
		/// </summary>
		public void Initialize()
		{
            Glfw.SetCursorPosCallback(handler.Handle, cursorCallback);
            Glfw.SetScrollCallback(handler.Handle, scrollCallback);
			Glfw.SetMouseButtonCallback(handler.Handle, buttonCallback);
		}

		/// <summary>
		/// Private window handler.
		/// </summary>
		private GlfwWindow handler { get; }

		#region Buttons

		/// <summary>
		/// Mouse left button state (true if pressed).
		/// </summary>
		public bool LeftButton => GetButtonState(MouseButton.ScrollButton);

		/// <summary>
		/// Mouse right button state (true if pressed).
		/// </summary>
		public bool RightButton => GetButtonState(MouseButton.RightButton);

		/// <summary>
		/// Mouse center button state (true if pressed).
		/// </summary>
		public bool ScrollButton => GetButtonState(MouseButton.ScrollButton);

		#endregion

		#region Position

		public Vector2 Position
		{
			get
			{
				double x = 0, y = 0;

				Glfw.GetCursorPos(handler.Handle, out x, out y);

				return new Vector2((float)x, (float)y);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Triggered on mouse move.
		/// </summary>
		public event MouseMoveEventHandler OnMove;

		/// <summary>
		/// Triggered on mouse scroll movement.
		/// </summary>
		public event MouseScrollEventHandler OnScroll;

		/// <summary>
		/// Triggered on mouse button event.
		/// </summary>
		public event MouseButtonEventHandler OnButtonEvent;

		#endregion

		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		public void Update(IFrameBasedClock Clock)
		{

		}

		/// <summary>
		/// Gets the state of a specified mouse button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		public bool GetButtonState(MouseButton button)
		{
			return Glfw.GetMouseButton(handler.Handle, (Glfw.MouseButton)button);
		}
	}
}
