using System;

namespace pEngine.Utils.Timing.Base

{
    /// <summary>
    /// A basic clock for keeping time.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// The current time of this clock, in milliseconds.
        /// </summary>
        double CurrentTime { get; }

        /// <summary>
        /// The rate this clock is running at.
        /// </summary>
        double Rate { get; }

        /// <summary>
        /// Whether this clock is running.
        /// </summary>
        bool IsRunning { get; }
    }
}
