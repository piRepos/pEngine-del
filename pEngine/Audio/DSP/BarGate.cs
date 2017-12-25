// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;

using pEngine.Framework.Binding;

namespace pEngine.Audio.DSP
{
    /// <summary>
    /// Implements a bar gate audio DSP.
    /// </summary>
    public class BarGate : DSP
    {
        /// <summary>
        /// Makes a new instance of <see cref="BarGate"/> class.
        /// </summary>
        public BarGate() : base()
        {

        }

        #region Properties

        /// <summary>
        /// Beat divisor.
        /// </summary>
        [Bindable]
        public uint Divisor { get; set; } = 4;

        /// <summary>
        /// Number of bars between each state change.
        /// </summary>
        [Bindable]
        public uint Bars { get; set; } = 2;

        /// <summary>
        /// Song beats per minute.
        /// </summary>
        [Bindable]
        public double BPM { get; set; } = 120;

        /// <summary>
        /// Volume On gate open.
        /// </summary>
        [Bindable]
        public uint MaxVol { get; set; } = 100;

        /// <summary>
        /// Volume On gate close.
        /// </summary>
        [Bindable]
        public uint MinVol { get; set; } = 0;

        /// <summary>
        /// Milliseconds of the high - low voume transition
        /// (Default 2 for smooth volume change)
        /// </summary>
        [Bindable]
        public double EasingTime { get; set; } = 2;

        /// <summary>
        /// Song offset.
        /// </summary>
        [Bindable]
        public double Offset { get; set; }

        /// <summary>
        /// Bypass this DSP.
        /// </summary>
        [Bindable]
        public new bool Bypass { get; set; }

        #endregion

        #region State

        double StartEasing = 0;

        double Time = 0;

        bool Low = false;

        #endregion

        protected override void Callback(IntPtr Buffer, int Length)
        {
            double Volume = 0;

            for (uint Sample = 0; Sample < Length / 4; Sample += 2)
            {
                this.Time += 1D / 44.1D;

                if (Bypass)
                    continue;

                double Time = this.Time - Offset;

                // Bar calculation
                if (((int)(((Time / 1000.0) / 60.0) * BPM * Divisor) % Bars) == 0 && Low)
                {
                    Low = false;
                    StartEasing = Time;
                }
                if (((int)(((Time / 1000.0) / 60.0) * BPM * Divisor) % Bars) != 0 && !Low)
                {
                    Low = true;
                    StartEasing = Time;
                }

                // Easing
                if (!Low)
                    Volume = (MaxVol / 100) - (EasingTime == 0 ? 0 : ((Time - StartEasing) / (float)EasingTime));
                else if (Low)
                    Volume = (EasingTime == 0 ? 0 : ((Time - StartEasing) / (float)EasingTime));

                Volume = Math.Min(Volume, 1.0F);
                Volume = Math.Max(Volume, 0.0F);

                // Output
                for (int CH = 0; CH < 2; CH++)
                {
                    unsafe
                    {
                        float* BOut = (float*)Buffer;
                        float* BIn = (float*)Buffer;
                        BOut[Sample + CH] = BIn[Sample + CH] * (float)Volume;
                    }
                }
            }
        }

    }
}
