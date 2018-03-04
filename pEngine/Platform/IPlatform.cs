using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using pEngine.Input;
using pEngine.Context;
using pEngine.Platform.Monitors;

namespace pEngine.Platform
{
	/// <summary>
	/// Implemented by an OS api class.
	/// </summary>
	public interface IPlatform
	{
		/// <summary>
		/// Gets the current platform name.
		/// </summary>
		string PlatformName { get; }

		/// <summary>
		/// Gets the operative system version.
		/// </summary>
		Version OSVersion { get; }

		/// <summary>
		/// Gets the primary monitor on this system.
		/// </summary>
		IMonitor MainMonitor { get; }

		/// <summary>
		/// Gets all system monitors.
		/// </summary>
		IEnumerable<IMonitor> AvaiableMonitors { get; }

		/// <summary>
		/// Input hardware manager.
		/// </summary>
		/// <param name="context">Main window.</param>
		DeviceManager GetInput(IWindow context);

		/// <summary>
		/// Gets a window for this application.
		/// </summary>
		IPlatformWindow GetWindow();

		/// <summary>
		/// Gets a window for this application.
		/// </summary>
		IPlatformWindow GetWindow(IWindow shared);

		/// <summary>
		/// Initialize this platform API.
		/// </summary>
		void Initialize();
	}
}
