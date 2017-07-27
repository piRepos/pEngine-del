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

using ManagedBass.Fx;
using ManagedBass;

namespace pEngine.Core.Audio.DSP
{
    /// <summary>
    /// BassFx Chorus Effect.
    /// </summary>
    /// <remarks>
    /// <para>
    /// True vintage chorus works the same way as flanging. 
    /// It mixes a varying delayed signal with the original to produce a large number of harmonically related notches in the frequency response. 
    /// Chorus uses a longer delay than flanging, so there is a perception of "spaciousness", although the delay is too short to hear as a distinct slap-back echo. 
    /// There is also little or no feedback, so the effect is more subtle.
    /// </para>
    /// <para>
    /// The <see cref="DryMix"/> is the volume of Input signal and the <see cref="WetMix"/> is the volume of delayed signal. 
    /// The <see cref="Feedback"/> sets feedback of chorus. 
    /// The <see cref="Rate"/>, <see cref="MinSweep"/> and <see cref="MaxSweep"/> control how fast and far the frequency notches move. 
    /// The <see cref="Rate"/> is the rate of delay change in millisecs per sec, <see cref="MaxSweep"/>-<see cref="MinSweep"/> is the range or width of sweep in ms.
    /// </para>
    /// </remarks>
    public class Chorus : IEffect
    {


        public Chorus()
        {
            Params = new ChorusParameters();

            SyncProcedure = (a, b, c, d) => Dispose();

            PinParams();
        }

        public void Dispose()
        {
            ParamsHandler.Free();
        }

        #region Params

        public float fDryMix;
        public float fFeedback;
        public float fMaxSweep;
        public float fMinSweep;
        public float fRate;
        public float fWetMix;
        public FXChannelFlags lChannel = FXChannelFlags.All;

        public int priority;

        /// <summary>
        /// Dry (unaffected) signal mix (-2...+2). Default = 0.9
        /// </summary>
        public double DryMix
        {
            get { return fDryMix; }
            set
            {
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
                fFeedback = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Maximum delay in ms (0&lt;...6000). Default = 400.
        /// </summary>
        public double MaxSweep
        {
            get { return fMaxSweep; }
            set
            {
                fMaxSweep = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Minimum delay in ms (0&lt;...6000). Default = 1.
        /// </summary>
        public double MinSweep
        {
            get { return fMinSweep; }
            set
            {
                fMinSweep = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Rate in ms/s (0&lt;...1000). Default = 200.
        /// </summary>
        public double Rate
        {
            get { return fRate; }
            set
            {
                fRate = (float)value;

                RefreshState();
            }
        }

        /// <summary>
        /// Wet (affected) signal mix (-2...+2). Default = 0.35.
        /// </summary>
        public double WetMix
        {
            get { return fWetMix; }
            set
            {
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
        public void Flanger()
        {
            fDryMix = 1;
            fWetMix = 0.35f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 5;
            fRate = 1;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Exaggerated()
        {
            fDryMix = 0.7f;
            fWetMix = 0.25f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 200;
            fRate = 50;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void MotorCycle()
        {
            fDryMix = 0.9f;
            fWetMix = 0.45f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 100;
            fRate = 25;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Devil()
        {
            fDryMix = 0.9f;
            fWetMix = 0.35f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 50;
            fRate = 200;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void ManyVoices()
        {
            fDryMix = 0.9f;
            fWetMix = 0.35f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 400;
            fRate = 200;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void BackChipmunk()
        {
            fDryMix = 0.9f;
            fWetMix = -0.2f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            MaxSweep = 400;
            fRate = 400;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Water()
        {
            fDryMix = 0.9f;
            fWetMix = -0.4f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 2;
            fRate = 1;

            RefreshState();
        }

        /// <summary>
        /// Set up a Preset.
        /// </summary>
        public void Airplane()
        {
            fDryMix = 0.3f;
            fWetMix = 0.4f;
            fFeedback = 0.5f;
            fMinSweep = 1;
            fMaxSweep = 10;
            fRate = 5;

            RefreshState();
        }

        #endregion

        #region Params

        private ChorusParameters Params;
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
            Params.fMinSweep = fMinSweep;
            Params.fMaxSweep = fMaxSweep;

            Params.lChannel = lChannel;

            Bass.FXSetParameters(EffectHandler, ParamsHandler.AddrOfPinnedObject());
        }
    }
}
