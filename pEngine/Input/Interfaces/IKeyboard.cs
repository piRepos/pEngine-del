using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Input
{
    public interface IKeyboard : IDevice
    {
		/// <summary>
		/// True if caps lock is enabled.
		/// </summary>
		bool CapsLock { get; }

		/// <summary>
		/// True if num lock is enabled.
		/// </summary>
		bool NumLock { get; }

		/// <summary>
		/// True if scroll lock is enabled.
		/// </summary>
		bool ScrollLock { get; }

		/// <summary>
		/// Enabled key modifiers.
		/// </summary>
		KeyModifier Modifiers { get; }

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>The button state.</returns>
		KeyState GetButtonState(KeyboardKey key);

		/// <summary>
		/// Gets the state of a specified keyboard button.
		/// </summary>
		/// <param name="button">Button to check.</param>
		/// <returns>True if this button is pressed, false otherwise.</returns>
		bool IsPressed(KeyboardKey key);

		/// <summary>
		/// Triggered on keyboard key event.
		/// </summary>
		event EventHandler<KeyboardKeyEventArgs> OnKeyEvent;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		event EventHandler<KeyboardTypeEventArgs> OnType;

		/// <summary>
		/// Triggered on typing.
		/// </summary>
		event EventHandler<KeyboardModTypeEventArgs> OnTypeWithMods;
	}

	public class KeyboardKeyEventArgs : EventArgs
	{
		/// <summary>
		/// Target key.
		/// </summary>
		public KeyboardKey Key { get; set; }

		/// <summary>
		/// Scancode of this key.
		/// </summary>
		public int Scancode { get; set; }

		/// <summary>
		/// Key current state.
		/// </summary>
		public KeyState Action { get; set; }

		/// <summary>
		/// Active modifier keys during this event.
		/// </summary>
		public KeyModifier Modifiers { get; set; }
	}

	public class KeyboardTypeEventArgs : EventArgs
	{
		/// <summary>
		/// Target key.
		/// </summary>
		public char Point { get; set; }
	}

	public class KeyboardModTypeEventArgs : KeyboardTypeEventArgs
	{
		/// <summary>
		/// Active modifier keys during this event.
		/// </summary>
		public KeyModifier Modifiers { get; set; }
	}

	[Flags]
	public enum KeyModifier
	{
		None = 0,

		/// <summary>
		/// Shift modifier (left or right shift).
		/// </summary>
		Shift = 0x0001,

		/// <summary>
		/// Control modifier (left or right control).
		/// </summary>
		Control = 0x0002,

		/// <summary>
		/// Alt modifier (left or right alt).
		/// </summary>
		Alt = 0x0004,

		/// <summary>
		/// Super modifier (left or right super).
		/// </summary>
		/// <remarks>
		/// It's the button between Ctrl and Alt.
		/// </remarks>
		Super = 0x0008
	}

	public enum KeyboardKey
	{
		/// <summary>
		/// Unknow key.
		/// </summary>
		Unknow = -1,
		Space = 32,

		/// <summary>
		/// "'" key.
		/// </summary>
		Apostrophe = 39,

		/// <summary>
		/// "," key.
		/// </summary>
		Comma = 44,

		/// <summary>
		/// "-" key.
		/// </summary>
		Minus = 45,

		/// <summary>
		/// "." key.
		/// </summary>
		Period = 46,

		/// <summary>
		/// "/" key.
		/// </summary>
		Slash = 47,

		Number0 = 48,
		Number1 = 49,
		Number2 = 50,
		Number3 = 51,
		Number4 = 52,
		Number5 = 53,
		Number6 = 54,
		Number7 = 55,
		Number8 = 56,
		Number9 = 57,

		/// <summary>
		/// ";" key.
		/// </summary>
		Semicolon = 59,

		/// <summary>
		/// "=" key.
		/// </summary>
		Equal = 61,

		A = 65,
		B = 66,
		C = 67,
		D = 68,
		E = 69,
		F = 70,
		G = 71,
		H = 72,
		I = 73,
		J = 74,
		K = 75,
		L = 76,
		M = 77,
		N = 78,
		O = 79,
		P = 80,
		Q = 81,
		R = 82,
		S = 83,
		T = 84,
		U = 85,
		V = 86,
		W = 87,
		X = 88,
		Y = 89,
		Z = 90,

		/// <summary>
		/// "[" key.
		/// </summary>
		LeftBracket = 91,

		/// <summary>
		/// "\" key.
		/// </summary>
		Backslash = 92,

		/// <summary>
		/// "]" key.
		/// </summary>
		RightBracket = 93,

		/// <summary>
		/// "`" key.
		/// </summary>
		GraveAccent = 96,

		/// <summary>
		/// non-US #1
		/// </summary>
		World1 = 161,

		/// <summary>
		/// non-US #2
		/// </summary>
		World2 = 162,

		Escape = 256,
		Enter = 257,
		Tab = 258,
		Backspace = 259,
		Insert = 260,
		Delete = 261,

		Right = 262,
		Left = 263,
		Down = 264,
		Up = 265,

		PageUp = 266,
		PageDown = 267,
		Home = 268,
		End = 269,

		CapsLock = 280,

		ScrollLock = 281,
		NumLock = 282,
		PrintScreen = 283,
		Pause = 284,

		F1 = 290,
		F2 = 291,
		F3 = 292,
		F4 = 293,
		F5 = 294,
		F6 = 295,
		F7 = 296,
		F8 = 297,
		F9 = 298,
		F10 = 299,
		F11 = 300,
		F12 = 301,
		F13 = 302,
		F14 = 303,
		F15 = 304,
		F16 = 305,
		F17 = 306,
		F18 = 307,
		F19 = 308,
		F20 = 309,
		F21 = 310,
		F22 = 311,
		F23 = 312,
		F24 = 313,
		F25 = 314,

		KeyPad0 = 320,
		KeyPad1 = 321,
		KeyPad2 = 322,
		KeyPad3 = 323,
		KeyPad4 = 324,
		KeyPad5 = 325,
		KeyPad6 = 326,
		KeyPad7 = 327,
		KeyPad8 = 328,
		KeyPad9 = 329,

		KeyPadDecimal = 330,
		KeyPadDivide = 331,
		KeyPadMultiply = 332,
		KeyPadSubtract = 333,
		KeyPadAdd = 334,
		KeyPadEnter = 335,
		KeyPadEqual = 336,

		LeftShift = 340,
		LeftControl = 341,
		LeftAlt = 342,
		LeftSuper = 343,

		RightShift = 344,
		RightControl = 345,
		RightAlt = 346,
		RightSuper = 347,

		Menu = 348,
		Last = Menu
	}


}