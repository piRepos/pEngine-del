using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Timing.Base;

namespace pEngine.Core.Input
{

    public class InputManager
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// </summary>
        /// <param name="host">Game host.</param>
		public InputManager(pEngine host)
		{
			Host = host;
		}

		/// <summary>
		/// Parent game host.
		/// </summary>
		protected pEngine Host { get; }

        /// <summary>
        /// Gets the device moudule.
        /// </summary>
        internal DeviceManager Hardware { get; set; }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        internal void Initialize()
        {
            Hardware.InitializeDevices();
        }

        /// <summary>
        /// Events the dispatch.
        /// </summary>
        /// <param name="clock">Clock.</param>
        internal void EventDispatch(IFrameBasedClock clock)
        {
            // - Update devices state
            Hardware.UpdateDevices(clock);


        }

	}
}
