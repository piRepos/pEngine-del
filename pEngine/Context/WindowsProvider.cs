using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Framework;
using pEngine.Framework.Modules;

using pEngine.Timing;
using pEngine.Timing.Base;

namespace pEngine.Context
{
	public class WindowsProviderService : Service
	{
		/// <summary>
		/// Makes a new instace of <see cref="WindowsProviderService"/> class.
		/// </summary>
		/// <param name="module">Parent module.</param>
		/// <param name="mainLoop">Service use loop.</param>
		public WindowsProviderService(WindowsProvider module, GameLoop mainLoop) 
			: base(module, mainLoop)
		{

			mainLoop.Scheduler.AddDelayed(() =>
			{
				foreach (var vwin in Module.Windows)
				{
					vwin.SendChanges();
				}

			}, 0, true);
		}

		/// <summary>
		/// Input engine.
		/// </summary>
		public new WindowsProvider Module => base.Module as WindowsProvider;

		/// <summary>
		/// Application main window.
		/// </summary>
		[ServiceProperty("MainWindow")]
		public VirtualWindow MainWindow { get; private set; }

		#region Window creation

		/// <summary>
		/// Makes a new game window.
		/// </summary>
		[ServiceMethod(ReferencesTo = "CreateNewWindow")]
		protected void CreateNewWindow(VirtualWindow vir) { }

		/// <summary>
		/// Gets a new game window.
		/// </summary>
		/// <returns>A window.</returns>
		public VirtualWindow GetNewWindow()
		{
			VirtualWindow virt = new VirtualWindow(MainWindow);
			CreateNewWindow(virt);
			return virt;
		}

		#endregion

		#region Data sharing

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);

			foreach (var vwin in Module.Windows)
			{
				vwin.ApplyChanges();
			}
		}

		#endregion
	}

	public class WindowsProvider : Module
	{
		/// <summary>
		/// Makes a new instance of <see cref="WindowsProvider"/> class.
		/// </summary>
		/// <param name="host">Game host parent.</param>
		/// <param name="moduleLoop">Owner's gameloop.</param>
		public WindowsProvider(GameHost host, GameLoop moduleLoop) 
			: base(host, moduleLoop)
		{
			Handlers = new Dictionary<VirtualWindow, IWindow>();
		}

		/// <summary>
		/// Application main window.
		/// </summary>
		public VirtualWindow MainWindow { get; private set; }

		/// <summary>
		/// Application main window context.
		/// </summary>
		public IPlatformWindow MainContext { get; private set; }

		/// <summary>
		/// Initialize this module.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// - Make main window
			MainWindow = new VirtualWindow();

			// - Gets the real window for this reference
			IPlatformWindow win = Host.Platform.GetWindow();
			Handlers.Add(MainWindow, win);
			MainContext = win;
			MainWindow.RealWindow = win;
			
			// - Make the main window
			win.Make();
		}

		#region Windows management

		/// <summary>
		/// Controller - Handler dictionary.
		/// </summary>
		protected Dictionary<VirtualWindow, IWindow> Handlers { get; }

		/// <summary>
		/// Gets all abstract windows.
		/// </summary>
		public IEnumerable<VirtualWindow> Windows => Handlers.Keys;

		/// <summary>
		/// Gets all platform windows.
		/// </summary>
		public IEnumerable<IWindow> PlatformWindows => Handlers.Values;

		/// <summary>
		/// Makes a new game window.
		/// </summary>
		public void CreateNewWindow(VirtualWindow vir)
		{
			// - Gets the real window for this reference
			IPlatformWindow win = Host.Platform.GetWindow(Handlers[MainWindow]);
			Handlers.Add(vir, win);
			vir.RealWindow = win;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vir"></param>
		/// <returns></returns>
		public IWindow GetHandler(VirtualWindow vir)
		{
			return Handlers[vir];
		}

		#endregion

		#region Serice getter

		/// <summary>
		/// Settings for this module.
		/// </summary>
		public override Service GetSettings(GameLoop mainLoop)
		{
			WindowsProviderService s = new WindowsProviderService(this, mainLoop);
			s.Initialize();
			Services.Add(s);
			return s;
		}

		#endregion

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public override void Update(IFrameBasedClock clock)
		{
			base.Update(clock);
		}
	}
}
