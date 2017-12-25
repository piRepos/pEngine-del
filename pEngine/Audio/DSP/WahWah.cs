// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

#pragma warning disable 0414

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManagedBass;
using ManagedBass.Fx;

namespace pEngine.Audio.DSP
{
    public class WahWah : IEffect
    {

        public WahWah()
        {
            Params = new AutoWahParameters();

            SyncProcedure = (a, b, c, d) => Dispose();

            PinParams();
        }

        public void Dispose()
        {
            ParamsHandler.Free();
        }

        #region Properties
        
        private float fDryMix;
        private float fFeedback;
        private float fFreq;
        private float fRange;
        private float fRate;
        private float fWetMix;
        private FXChannelFlags lChannel = FXChannelFlags.All;

        private int priority;

        /// <summary>
        /// Dry (unaffected) signal mix (-2...+2). Default = 0.5.
        /// </summary>
        public double DryMix
        {
            get { return fDryMix; }
            set
            {
                if (value == fDryMix)
                    return;

                fDryMix = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Feedback (-1...+1). Default = 0.5.
        /// </summary>
        public double Feedback
        {
            get { return fFeedback; }
            set
            {
                if (value == fFeedback)
                    return;

                fFeedback = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Base frequency of sweep range (0&lt;...1000). Default = 50.
        /// </summary>
        public double Frequency
        {
            get { return fFreq; }
            set
            {
                if (value == fFreq)
                    return;

                fFreq = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Sweep range in octaves (0&lt;...&lt;10). Default = 4.3.
        /// </summary>
        public double Range
        {
            get { return fRange; }
            set
            {
                if (value == fRange)
                    return;

                fRange = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Rate of sweep in cycles per second (0&lt;...&lt;10). Default = 2.
        /// </summary>
        public double Rate
        {
            get { return fRate; }
            set
            {
                if (value == fRate)
                    return;

                fRate = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Wet (affected) signal mix (-2...+2). Default = 1.5.
        /// </summary>
        public double WetMix
        {
            get { return fWetMix; }
            set
            {
                if (value == fWetMix)
                    return;

                fWetMix = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// A <see cref="FXChannelFlags" /> flag to define on which channels to apply the effect. Default: <see cref="FXChannelFlags.All"/>
        /// </summary>
        public FXChannelFlags Channels
        {
            get { return lChannel; }
            set
            {
                if (value == lChannel)
                    return;

                lChannel = value;

                RefreshState();
            }
        }

        /// <summary>
        /// Checks whether the effect is currently enabled and active.
        /// </summary>
        public bool Bypass
        {
            set
            {
                if (ChannelHandler == 0)
                    return;

                if (!value && Bypass)
                    EffectHandler = Bass.ChannelSetFX(ChannelHandler, Params.FXType, 1);

                else if (value && !Bypass && Bass.ChannelRemoveFX(ChannelHandler, EffectHandler))
                    EffectHandler = 0;

                RefreshState();
            }
            get { return ChannelHandler == 0 || EffectHandler == 0; }
        }

        /// <summary>
        /// Priority of the Effect in DSP chain.
        /// </summary>
        public int Priority
        {
            get { return priority; }
            set
            {
                if (Bypass && Bass.FXSetPriority(EffectHandler, value))
                    priority = value;
            }
        }

        #endregion

        #region Presets

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Slow()
        {
            fDryMix = 0.5f;
            fWetMix = 1.5f;
            fFeedback = 0.5f;
            fRate = 2;
            fRange = 4.3f;
            fFreq = 50;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Fast()
        {
            fDryMix = 0.5f;
            fWetMix = 1.5f;
            fFeedback = 0.5f;
            fRate = 5;
            fRange = 5.3f;
            fFreq = 50;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void HiFast()
        {
            fDryMix = 0.5f;
            fWetMix = 1.5f;
            fFeedback = 0.5f;
            fRate = 5;
            fRange = 4.3f;
            fFreq = 500;

            RefreshState();
        }

        #endregion

        #region Params

        private AutoWahParameters Params;
        private GCHandle ParamsHandler;

        private void PinParams()
        {
            ParamsHandler = GCHandle.Alloc(Params, GCHandleType.Pinned);
        }

        #endregion

        #region Handler

        private int EffectHandler;
        private int SyncHandler;
        private int ChannelHandler;

        readonly SyncProcedure SyncProcedure;

        /// <summary>
        /// Bind this effect on a specified stream.
        /// </summary>
        /// <param name="Stream">Target stream.</param>
        /// <param name="Priority">Effect priority.</param>
        public void Bind(int Channel, int Priority)
        {
            ChannelHandler = Channel;
            priority = Priority;

            SyncHandler = Bass.ChannelSetSync(Channel, SyncFlags.Free, 0, SyncProcedure);

            Bypass = false;
        }

        /// <summary>
        /// Unbind this effect from the binded stream.
        /// </summary>
        /// <param name="Stream">Target stream.</param>
        public void Unbind(int Channel)
        {
            if (Channel != ChannelHandler)
                return;

            if (Bass.ChannelRemoveFX(ChannelHandler, EffectHandler))
            {
                ChannelHandler = 0;
                EffectHandler = 0;
            }
        }

        #endregion

        private void RefreshState()
        {
            Params.fDryMix = fDryMix;
            Params.fWetMix = fWetMix;
            Params.fFeedback = fFeedback;
            Params.fRate = fRate;
            Params.fRange = fRange;
            Params.fFreq = fFreq;

            Params.lChannel = lChannel;

            Bass.FXSetParameters(EffectHandler, ParamsHandler.AddrOfPinnedObject());
        }

    }
}
