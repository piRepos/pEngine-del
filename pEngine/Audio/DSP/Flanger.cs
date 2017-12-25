// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;

using pEngine.Framework.Binding;

namespace pEngine.Audio.DSP
{
    /// <summary>
    /// Audio flanger DSP.
    /// </summary>
    public class Flanger : DSP
    {
        /// <summary>
        /// Makes a new instance of <see cref="Flanger"/> class.
        /// </summary>
        public Flanger()
        {
            FlangerPosition = 0;
            FlangerSweep = FlangerBufferLen / 2.0f;
            FlangerIncrement = 0.002f;

            FlangerBuffer = new float[FlangerBufferLen, 2];
        }

        #region Properties

        /// <summary>
        /// Flanger period.
        /// </summary>
        [Bindable]
        public double Period
        {
            get { return (FlangerBufferLen / 44.1D); }
            set
            {
                FlangerBufferLen = (int)(value * 44.1D);
                FlangerBuffer = new float[FlangerBufferLen, 2];
                FlangerSweep = FlangerBufferLen / 2.0f;
            }
        }

        #endregion

        #region State

        private int FlangerBufferLen = 44;
        private int FlangerPosition;
        private float FlangerSweep;
        private float FlangerIncrement;

        private float[,] FlangerBuffer;

        #endregion

        protected override void Callback(IntPtr Buffer, int Length)
        {
            float CurrentValue;
            int Sample;

            unsafe
            {
                var BufferPointer = (float*)Buffer;


                for (Sample = 0; Sample < Length / 4; Sample += 2)
                {
                    var p1 = (FlangerPosition + (int)FlangerSweep) % FlangerBufferLen;
                    var p2 = (p1 + 1) % FlangerBufferLen;
                    var f = FlangerSweep - (int)FlangerSweep;


                    CurrentValue = (float)((BufferPointer[Sample] + (FlangerBuffer[p1, 0] * (1 - f) + FlangerBuffer[p2, 0] * f)) * 0.7);

                    FlangerBuffer[FlangerPosition, 0] = BufferPointer[Sample];
                    BufferPointer[Sample] = CurrentValue;

                    CurrentValue = (float)((BufferPointer[Sample + 1] + (FlangerBuffer[p1, 1] * (1 - f) + FlangerBuffer[p2, 1] * f)) * 0.7);

                    FlangerBuffer[FlangerPosition, 1] = BufferPointer[Sample + 1];
                    BufferPointer[Sample + 1] = CurrentValue;

                    FlangerPosition++;

                    if (FlangerPosition == FlangerBufferLen)
                        FlangerPosition = 0;

                    FlangerSweep += FlangerIncrement;

                    if (FlangerSweep < 0 || FlangerSweep > FlangerBufferLen - 1)
                    {
                        FlangerIncrement = -FlangerIncrement;
                        FlangerSweep += FlangerIncrement;
                    }
                }
            }
        }
    }
}
