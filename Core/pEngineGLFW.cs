using System;

using Glfw3;

using pEngine.Platform.Windows;
using pEngine.Platform.Input;

using pEngine.Common.Timing.Base;

namespace pEngine
{
	public class pEngineGLFW : pEngine
	{

		/// <summary>
		/// Create a new hinstance of <see cref="pEngine"/>.
		/// </summary>
		/// <param name="args">Program arguments.</param>
		/// <param name="gameName">Game name.</param>
		internal pEngineGLFW(string[] args, string gameName)
			: base(args, gameName)
		{
			// - Library initialization
			try
			{
				if (!Glfw.Init())
				{
					Debug.ErrorLog("Cannot initialize GLFW");
					Environment.Exit(0);
				}
			}
			catch (DllNotFoundException ex)
			{
				Debug.CriticalErrorLog(ex, "Cannot load GLFW library");
				Environment.Exit(0);
			}

			// - Window initialization
			Window = new GlfwWindow();

            // - Input devices setting
            Input.Hardware = new GlfwDeviceManager(Window as GlfwWindow);
		}


		/// <summary>
		/// Updates the input.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		protected override void Events(IFrameBasedClock clock)
        {
            base.Events(clock);
        }

		/// <summary>
		/// Updates game tree.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		override protected void Update(IFrameBasedClock clock)
		{
			base.Update(clock);
		}

		/// <summary>
		/// Update graphics and draw the scene on the screen.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		override protected void Draw(IFrameBasedClock clock)
		{
			base.Draw(clock);
		}
	}
}
