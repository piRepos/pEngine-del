using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Utils.Threading;
using pEngine.Framework.Timing;
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
		public Module(GameHost host, Scheduler scheduler)
		{
			Host = host;
			Scheduler = scheduler;
		}
		
		/// <summary>
		/// Module owner.
		/// </summary>
		public GameHost Host { get; }

		/// <summary>
		/// Thread schduler for this module.
		/// </summary>
		public Scheduler Scheduler { get; }

		/// <summary>
		/// Settings for this module.
		/// </summary>
		public virtual Service GetSettings(Scheduler mainScheduler)
		{
			Service s = new Service(this, mainScheduler);
			s.Initialize();
			return s;
		}

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

		}
	}
}
