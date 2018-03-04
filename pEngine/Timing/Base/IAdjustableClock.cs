using System;

namespace pEngine.Timing.Base
{
    /// <summary>
    /// A clock that can be started, stopped, reset etc.
    /// </summary>
    public interface IAdjustableClock : IClock
    {
        /// <summary>
        /// Stop and reset position.
        /// </summary>
        void Reset();

        /// <summary>
        /// Start (resume) running.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop (pause) running.
        /// </summary>
        void Stop();

        /// <summary>
        /// Seek to a specific time position.
        /// </summary>
        /// <returns>Whether a seek was possible.</returns>
        bool Seek(double position);
    }
}
