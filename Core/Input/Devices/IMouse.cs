using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;

namespace pEngine.Core.Input
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
		/// Gets the state of a specified mouse button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		bool GetButtonState(MouseButton button);

		/// <summary>
		/// Triggered on mouse move.
		/// </summary>
		event MouseMoveEventHandler OnMove;

		/// <summary>
		/// Triggered on mouse scroll movement.
		/// </summary>
		event MouseScrollEventHandler OnScroll;

		/// <summary>
		/// Triggered on mouse button event.
		/// </summary>
		event MouseButtonEventHandler OnButtonEvent;
    }

	/// <summary>
	/// Handles mouse position changes.
	/// </summary>
	/// <param name="pos">Current mouse position.</param>
	public delegate void MouseMoveEventHandler(Vector2 pos);

	/// <summary>
	/// Handles scroll or touchpad gesture for scroll offset.
	/// </summary>
	/// <param name="offset">Offset from 0 reference.</param>
	public delegate void MouseScrollEventHandler(Vector2 offset);

	/// <summary>
	/// Handles mouse buttons press events.
	/// </summary>
	/// <param name="button">Target button.</param>
	/// <param name="action">Action on this button.</param>
	/// <param name="modifiers">Keyboard modifiers enabled during this action.</param>
	public delegate void MouseButtonEventHandler(MouseButton button, KeyState action, KeyModifier modifiers);

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
