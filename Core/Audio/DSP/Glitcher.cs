// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Core.Audio.DSP
{
    public class Glitcher : DSP
    {

        public Glitcher()
        {

        }

        #region Properties

        /// <summary>
        /// Beat divisor.
        /// </summary>
        public uint Divisor { get; set; } = 4;

        /// <summary>
        /// Number of bars between each state change.
        /// </summary>
        public uint Bars { get; set; } = 2;

        /// <summary>
        /// Song beats per minute.
        /// </summary>
        public double BPM { get; set; } = 120;

        #endregion

        #region State

        float[] Buffer = new float[0];

        bool Recorded = false;

        int[] RecordedIndex = new int[2];
        int[] PlaybackIndex = new int[2];

        /// <summary>
        /// Reset this DSP.
        /// </summary>
        public void Reset()
        {
            PlaybackIndex[0] = 0;
            PlaybackIndex[1] = 0;
            RecordedIndex[0] = 0;
            RecordedIndex[1] = 0;
            Recorded = false;
        }

        /// <summary>
        /// Reset buffers.
        /// </summary>
        public void Resample()
        {
            Reset();
        }

        /// <summary>
        /// Enable this DSP.
        /// </summary>
        public void Enable()
        {
            Bypass = false;
            Reset();
        }

        /// <summary>
        /// Disable this DSP.
        /// </summary>
        public void Disable()
        {
            Bypass = true;
            Reset();
        }

        #endregion

        protected override void Callback(IntPtr AudioBuffer, int Length)
        {

            // Check audio buffer size
            uint BufferSize = (uint)((((60.0 / BPM) / Divisor) * Bars) * 44100);

            // Resize buffer
            if (BufferSize != Buffer.Length)
            {
                float[] TmpBuffer = new float[BufferSize * 2];
                Buffer.CopyTo(TmpBuffer, 0);
                Buffer = TmpBuffer;
            }

            for (uint Sample = 0; Sample < Length / 4; Sample += 2)
            {
                // Output
                for (int CH = 0; CH < 2; CH++)
                {
                    unsafe
                    {
                        float* BOut = (float*)AudioBuffer;
                        float* BIn = (float*)AudioBuffer;

                        if (!Recorded)
                        {
                            Buffer[RecordedIndex[CH] * 2 + CH] = BIn[Sample + CH];
                            RecordedIndex[CH]++;
                            if (RecordedIndex[1] >= BufferSize)
                                Recorded = true;
                        }

                        if (Recorded)
                        {
                            BOut[Sample + CH] = Buffer[PlaybackIndex[CH] * 2 + CH];
                            PlaybackIndex[CH] = (int)((PlaybackIndex[CH] + 1) % BufferSize);
                        }
                        else
                            BOut[Sample + CH] = BIn[Sample + CH];
                    }
                }
            }
        }
    }
}
