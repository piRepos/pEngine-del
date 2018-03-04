using System;

namespace pEngine.Timing.Base
{
    /// <summary>
    /// A clock which will only update its current time when a frame process is triggered.
    /// Useful for keeping a consistent time state across an individual update.
    /// </summary>
    public interface IFrameBasedClock : IClock
    {
        /// <summary>
        /// Elapsed time from last process frame.
        /// </summary>
        double ElapsedFrameTime { get; }

        /// <summary>
        /// Processes one frame. Generally should be run once per update loop.
        /// </summary>
        void ProcessFrame();
    }
}
