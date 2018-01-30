using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Utils.Threading;
using pEngine.Utils.Timing;
using pEngine.Utils.Timing.Base;

namespace pEngine.Framework.Modules
{
	public class Module : pObject, IUpdatable
	{
		/// <summary>
		/// Makes a new instance of <see cref="Module"/> class.
		/// </summary>
		/// <param name="host">Game host owner.</param>
		/// <param name="scheduler">Running thread scheduler.</param>
		public Module(GameHost host, GameLoop moduleLoop)
		{
			Host = host;
			ModuleLoop = moduleLoop;
			Services = new List<Service>();
		}

		/// <summary>
		/// Dispose all resources used from this class.
		/// </summary>
		/// <param name="disposing">Dispose managed resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Services.Clear();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Module owner.
		/// </summary>
		public GameHost Host { get; }

		/// <summary>
		/// Thread schduler for this module.
		/// </summary>
		public GameLoop ModuleLoop { get; }

		/// <summary>
		/// Initialize this module.
		/// </summary>
		public virtual void Initialize()
		{
			
		}

		/// <summary>
		/// Update the state of this element.
		/// </summary>
		/// <param name="clock">Game clock.</param>
		public virtual void Update(IFrameBasedClock clock)
		{
			foreach (Service s in Services)
				s.Update(clock);
		}

		#region Services

		protected List<Service> Services { get; }

		/// <summary>
		/// Settings for this module.
		/// </summary>
		public virtual Service GetSettings(GameLoop mainLoop)
		{
			Service s = new Service(this, mainLoop);
			s.Initialize();
			Services.Add(s);
			return s;
		}

		#endregion
	}
}
