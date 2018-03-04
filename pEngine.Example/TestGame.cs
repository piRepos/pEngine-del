using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Input;
using pEngine.Games;
using pEngine.Context;
using pEngine.Timing.Base;

namespace pEngine.Example
{
	public class TestGame : Game
	{
		/// <summary>
		/// Game tree service injection.
		/// </summary>
		[Service]
		protected InputService Input { get; private set; }

		/// <summary>
		/// Windows manager injection.
		/// </summary>
		[Service]
		protected WindowsProviderService Windows { get; private set; }

		/// <summary>
		/// Game loading routine.
		/// </summary>
		protected override void OnLoad()
		{
			base.OnLoad();

			// - Set general keys
			Input.Keyboard.OnKeyEvent += Keyboard_OnKeyEvent;
		}

		/// <summary>
		/// Game closing routine.
		/// </summary>
		public override bool OnClose()
		{
			return true;
		}

		#region Game general keys

		private void Keyboard_OnKeyEvent(object sender, KeyboardKeyEventArgs e)
		{
			if (e.Key == KeyboardKey.F12 && e.Action == KeyState.Pressed)
			{
				var monitor = Windows.MainWindow.Monitor;

				if (Windows.MainWindow.Fullscreen)
					Windows.MainWindow.Size = new Vector2i(1000, 1000);
				else Windows.MainWindow.Size = monitor.CurrentResolution.ResolutionSize;

				Windows.MainWindow.Fullscreen = !Windows.MainWindow.Fullscreen;

			}
		}

		#endregion
	}
}
