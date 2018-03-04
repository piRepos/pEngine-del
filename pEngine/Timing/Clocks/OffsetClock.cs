using System;

using pEngine.Timing.Base;

namespace pEngine.Timing.Clocks
{
    public class OffsetClock : IClock
    {
        protected IClock Source;

        /// <summary>
        /// Makes a new <see cref="OffsetClock"/>.
        /// </summary>
        /// <param name="source">Source clock.</param>
        public OffsetClock(IClock source)
        {
            Source = source;
        }

        /// <summary>
        /// Offset to add to source clock.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// The current time of this clock, in milliseconds.
        /// </summary>
        public double CurrentTime => Source.CurrentTime + Offset;

        /// <summary>
        /// The rate this clock is running at.
        /// </summary>
        public double Rate => Source.Rate;

        /// <summary>
        /// Whether this clock is running.
        /// </summary>
        public bool IsRunning => Source.IsRunning;

        
    }
}
