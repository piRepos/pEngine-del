using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Utils.Math;

namespace pEngine.Platform.Input
{
    public interface IMouse : IDevice, ISpacedf
    {
		/// <summary>
		/// Mouse left button state (true if pressed).
		/// </summary>
		bool LeftButton { get; }

		/// <summary>
		/// Mouse right button state (true if pressed).
		/// </summary>
		bool RightButton { get; }

		/// <summary>
		/// Mouse center button state (true if pressed).
		/// </summary>
		bool ScrollButton { get; }

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>The button state.</returns>
		KeyState GetButtonState(MouseButton key);

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		bool IsPressed(MouseButton key);

		/// <summary>
		/// Triggered on mouse move.
		/// </summary>
		event EventHandler<MousePositiontEventArgs> OnMove;

		/// <summary>
		/// Triggered on mouse scroll movement.
		/// </summary>
		event EventHandler<MouseOffsetEventArgs> OnScroll;

		/// <summary>
		/// Triggered on mouse button event.
		/// </summary>
		event EventHandler<MouseKeyEventArgs> OnButtonEvent;
    }

	public class MouseOffsetEventArgs : EventArgs
	{
		/// <summary>
		/// Offset value.
		/// </summary>
		public Vector2 Offset { get; set; }
	}

	public class MousePositiontEventArgs : EventArgs
	{
		/// <summary>
		/// Position value.
		/// </summary>
		public Vector2 Position { get; set; }
	}

	public class MouseKeyEventArgs : EventArgs
	{
		/// <summary>
		/// Target key.
		/// </summary>
		public MouseButton Key { get; set; }

		/// <summary>
		/// Key current state.
		/// </summary>
		public KeyState Action { get; set; }

		/// <summary>
		/// Active modifier keys during this event.
		/// </summary>
		public KeyModifier Modifiers { get; set; }
	}

	public enum MouseButton
	{
		Button1 = 0,
		Button2 = 1,
		Button3 = 2,
		Button4 = 3,
		Button5 = 4,
		Button6 = 5,
		Button7 = 6,
		Button8 = 7,

		/// <summary>
		/// Mouse left button.
		/// </summary>
		LeftButton = Button1,

		/// <summary>
		/// Mouse right button.
		/// </summary>
		RightButton = Button2,

		/// <summary>
		/// Mouse center button.
		/// </summary>
		ScrollButton = Button3
	}
}
