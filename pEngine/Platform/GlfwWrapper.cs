using System;
using System.Collections.Generic;

using pEngine.Input;
using pEngine.Context;

using pEngine.Platform.Input;
using pEngine.Platform.Forms;
using pEngine.Platform.Monitors;

using Anotar.Custom;

namespace pEngine.Platform
{
	public class GlfwWrapper : IPlatform
	{
		/// <summary>
		/// Makes a new instance of <see cref="GlfwWrapper"/> class.
		/// </summary>
		public GlfwWrapper()
		{
		}

		/// <summary>
		/// Gets the current platform name.
		/// </summary>
		public string PlatformName => "Windows";

		/// <summary>
		/// Gets the operative system version.
		/// </summary>
		public Version OSVersion => System.Environment.OSVersion.Version;

		/// <summary>
		/// Gets the primary monitor on this system.
		/// </summary>
		public IMonitor MainMonitor => GlfwMonitor.GetDefaultMonitor();

		/// <summary>
		/// Gets all system monitors.
		/// </summary>
		public IEnumerable<IMonitor> AvaiableMonitors => GlfwMonitor.AvaiableMonitors();

		/// <summary>
		/// Input hardware manager.
		/// </summary>
		/// <param name="context">Main window.</param>
		public DeviceManager GetInput(IWindow context)
		{
			if (context is GlfwWindow glfwContext)
				return new GlfwDeviceManager(glfwContext);
			else throw new ArgumentException("Can't mix a glfw input context with other window types.");
		}

		/// <summary>
		/// Gets a window for this application.
		/// </summary>
		public IPlatformWindow GetWindow()
		{
			return new GlfwWindow(null);
		}

		/// <summary>
		/// Gets a window for this application.
		/// </summary>
		/// <param name="shared">Parent window.</param>
		public IPlatformWindow GetWindow(IWindow shared)
		{
			if (shared is GlfwWindow glfwShared)
				return new GlfwWindow(glfwShared);
			else throw new ArgumentException("Can't mix a glfw windows with other window types.");
		}

		/// <summary>
		/// Initialize this platform API.
		/// </summary>
		public void Initialize()
		{
			if (!Glfw3.Glfw.Init())
			{
				LogTo.Fatal("Unable to initialize Glfw library.");
				System.Environment.Exit(1);
			}
		}
	}
}
