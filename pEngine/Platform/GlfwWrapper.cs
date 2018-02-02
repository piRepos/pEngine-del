using System;
using System.Collections.Generic;

using pEngine.Input;

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
			ApplicationWindow = new GlfwWindow();
			Input = new GlfwDeviceManager(ApplicationWindow as GlfwWindow);
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
		/// Gets the window for this application.
		/// </summary>
		public IWindow ApplicationWindow { get; }

		/// <summary>
		/// Input hardware manager.
		/// </summary>
		public DeviceManager Input { get; }

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
