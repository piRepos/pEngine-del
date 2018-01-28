using System;

using pEngine.Utils.Timing.Base;

namespace pEngine.Utils.Timing.Clocks
{
    class ManualClock : IClock
    {
        /// <summary>
        /// The current time of this clock, in milliseconds.
        /// </summary>
        public double CurrentTime { get; }

        /// <summary>
        /// The rate this clock is running at.
        /// </summary>
        public double Rate { get; }

        /// <summary>
        /// Whether this clock is running.
        /// </summary>
        public bool IsRunning { get; }
    }
}
