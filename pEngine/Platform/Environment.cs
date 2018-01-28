using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Linq;

using Anotar.Custom;

namespace pEngine.Platform
{
	public static class Environment
	{
		/// <summary>
		/// Gets the running platform.
		/// </summary>
		public static IPlatform Platform { get; private set; }

		/// <summary>
		/// Check if this instance of application is the first.
		/// </summary>
		public static bool FirstInstance { get; private set; }

		/// <summary>
		/// Used for check multiple instances.
		/// </summary>
		public static string SharedGuid = @"010314b1-b641-4702-9fa0-7b968673405b";

		/// <summary>
		/// Command line arguments.
		/// </summary>
		public static string[] Parameters => System.Environment.GetCommandLineArgs();

		/// <summary>
		/// Engine execution directory.
		/// </summary>
		public static string ExecutionPath => System.Environment.CurrentDirectory;

		#region Initialization

		/// <summary>
		/// True if environment is already initialized.
		/// </summary>
		private static bool Initialized { get; set; }

		/// <summary>
		/// Initialize the environment.
		/// </summary>
		public static void Initialize(bool forcePlatformNative = false)
		{
			if (Initialized)
				return;

			// - Check multiple instances
			var mut = new Mutex(true, "Global\\" + SharedGuid, out bool mutexCreated);
			FirstInstance = mut.WaitOne(0, false);

			// - Check for the current platform
			var os = System.Environment.OSVersion;

			switch (os.Platform)
			{
				case PlatformID.Win32NT: // - Windows
                    if (!forcePlatformNative)
                        Platform = new GlfwWrapper();
					break;
                case PlatformID.Unix: // - OSX / Linux
                    if (!forcePlatformNative)
                        Platform = new GlfwWrapper();
                    break;
			}

			// - Initialize the current platform
			Platform.Initialize();

			Initialized = true;
		}

		#endregion
	}
}
