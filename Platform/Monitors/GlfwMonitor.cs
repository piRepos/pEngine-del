using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Math;

using Glfw3;

namespace pEngine.Platform.Monitors
{
	class GlfwMonitor : IMonitor
	{

		#region Sigleton

		/// <summary>
		/// Returns all avaiable monitors.
		/// </summary>
		/// <returns>Enumeration of monitors.</returns>
		public static IEnumerable<IMonitor> AvaiableMonitors()
		{
			Glfw.Monitor[] monitors = Glfw.GetMonitors();

			foreach (Glfw.Monitor monitor in monitors)
			{
				yield return new GlfwMonitor(monitor);
			}
		}

		/// <summary>
		/// Gets the primary monitor.
		/// </summary>
		/// <returns>A glfw monitor.</returns>
		public static IMonitor GetDefaultMonitor()
		{
			return new GlfwMonitor(Glfw.GetPrimaryMonitor());
		}

		#endregion

		Resolution[] supportedResolutions;

		private GlfwMonitor(Glfw.Monitor handle)
		{
			Handle = handle;
			int i = 0;

			// - Name set
			Name = Glfw.GetMonitorName(handle);

			// - Position set
			Vector2i position;
			Glfw.GetMonitorPos(handle, out position.X, out position.Y);
			Position = position;

			// - Supported resolutions
			Glfw.VideoMode[] modes = Glfw.GetVideoModes(handle);
			Glfw.VideoMode current = Glfw.GetVideoMode(handle);
			supportedResolutions = new Resolution[modes.Length];
			foreach (Glfw.VideoMode mode in modes)
			{
				Resolution r = new Resolution
				{
					ResolutionSize = new Vector2i(mode.Width, mode.Height),
					RefreshRate = mode.RefreshRate,
					RedBits = mode.RedBits,
					BlueBits = mode.BlueBits,
					GreenBits = mode.GreenBits
				};

				supportedResolutions[i++] = r;

				if (current == mode)
					CurrentResolution = r;
			}
		}

		/// <summary>
		/// Glfw pointer to this monitor.
		/// </summary>
		private Glfw.Monitor Handle { get; }

		/// <summary>
		/// Monitor name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Monitor position.
		/// </summary>
		public Vector2i Position { get; private set; }

		/// <summary>
		/// Current monitor resolution.
		/// </summary>
		public Resolution CurrentResolution { get; }

		/// <summary>
		/// List of all avaiable resolutions.
		/// </summary>
		public IEnumerable<Resolution> SupportedResolutions => supportedResolutions;

	}
}
