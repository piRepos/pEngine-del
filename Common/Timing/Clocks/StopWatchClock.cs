using System.Diagnostics;

using pEngine.Common.Timing.Base;

namespace pEngine.Common.Timing.Clocks
{
    public class StopwatchClock : Stopwatch, IAdjustableClock
    {

        /// <summary>
        /// Make a new hinstance of <see cref="StopwatchClock"/>.
        /// </summary>
        /// <param name="start"></param>
        public StopwatchClock(bool start = false)
        {
            if (start)
                Start();
        }


        /// <summary>
        /// Seek offset adjust value.
        /// </summary>
        private double SeekOffset;

        /// <summary>
        /// Keep track of how much stopwatch time we have used at previous rates.
        /// </summary>
        private double RateChangeUsed;

        /// <summary>
        /// Keep track of the resultant time that was accumulated at previous rates.
        /// </summary>
        private double RateChangeAccumulated;

        public double CurrentTime => (stopwatchMilliseconds - RateChangeUsed) * Rate + RateChangeAccumulated + SeekOffset;

        private double stopwatchMilliseconds => (double)ElapsedTicks / Frequency * 1000;

        private double InternalRate = 1;

        /// <summary>
        /// Clock speed.
        /// </summary>
        public double Rate
        {
            get { return InternalRate; }

            set
            {
                if (InternalRate == value) return;

                RateChangeAccumulated += (stopwatchMilliseconds - RateChangeUsed) * InternalRate;
                RateChangeUsed = stopwatchMilliseconds;

                InternalRate = value;
            }
        }

        /// <summary>
        /// Seek to a specific time position.
        /// </summary>
        /// <returns>Whether a seek was possible.</returns>
        public bool Seek(double position)
        {
            SeekOffset = 0;
            SeekOffset = position - CurrentTime;
            return true;
        }
    }
}