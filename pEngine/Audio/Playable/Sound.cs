// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;

using ManagedBass;

using pEngine.Audio.DSP;
using pEngine.Audio.Base;
using pEngine.Resources.Files;
using pEngine.Resources;
using pEngine.Resources.Dependencies;

namespace pEngine.Audio.Playable
{
    using Audio = File;
	using PlaybackState = ManagedBass.PlaybackState;

    public class Sound : Resource, IPlayableComponent
    {
        [Dependency(Load = true)]
        public Audio AudioFile { get; }

        public Sound(Audio audioResource, bool disposeOnFinish = false)
        {
            Effects = new List<IEffect>();

            // Dependency
            AudioFile = audioResource;
        }

        public override void Dispose()
        {
            Stop();

            Parent = null;

            Bass.SampleFree(SampleHandle);
            SampleHandle = 0;

            base.Dispose();
        }

        #region Properties

        private double FrequencyInternal = 0D;
        private double VolumeInternal = 100D;
        private double PanInternal = 0D;
        private bool MuteInternal = false;

        /// <summary>
        /// Reproduction frequency. (affect pitch, 0 = normal frequency)
        /// </summary>
        public double Frequency
        {
            get { return FrequencyInternal; }
            set
            {
                if (FrequencyInternal == value)
                    return;
                
                FrequencyInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Volume of this object.
        /// </summary>
        public double Volume
        {
            get { return VolumeInternal; }
            set
            {
                if (VolumeInternal == value)
                    return;

                VolumeInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Sound orientation.
        /// </summary>
        public double Pan
        {
            get { return PanInternal; }
            set
            {
                if (PanInternal == value)
                    return;

                PanInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Mute this element.
        /// </summary>
        public bool Mute
        {
            get { return MuteInternal; }
            set
            {
                if (MuteInternal == value)
                    return;

                MuteInternal = value;

                UpdateState();
            }
        }

        /// <summary>
        /// Real volume value.
        /// </summary>
        public double RelativeVolume
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.RelativeVolume * (Volume / 100D) * (Parent.Mute ? 0 : 1);
                }
                else return Volume / 100D;
            }
        }

        /// <summary>
        /// Real Sound orientation.
        /// </summary>
        public double RelativePan
        {
            get
            {
                if (Parent != null)
                {
                    return (Parent.RelativePan + (Pan / 100D)) / 2;
                }
                else return Pan / 100D;
            }
        }

        /// <summary>
        /// Hierarchy parent element.
        /// </summary>
        public IMixableComponent Parent { get; set; }

        #endregion

        #region State

        /// <summary>
        /// Reproduce the current sound.
        /// </summary>
        public void Start()
        {
            if (State != ResourceState.Loaded)
                throw new InvalidOperationException("Audio not loaded.");

            Bass.ChannelPlay(Channel);
        }

        /// <summary>
        /// Stop reproduction and reset position.
        /// </summary>
        public void Stop()
        {
            if (State != ResourceState.Loaded)
                throw new InvalidOperationException("Audio not loaded.");

            Bass.ChannelStop(Channel);
        }

        /// <summary>
        /// Reproduction state of this sound.
        /// </summary>
        public Base.PlaybackState PlaybackState
        {
            get
            {
                if (State != ResourceState.Loaded)
					return Base.PlaybackState.Stopped;

				return (Base.PlaybackState)Bass.ChannelIsActive(Channel);
            }
        }

        /// <summary>
        /// Update reproduction parameters.
        /// </summary>
        public void UpdateState()
        {
            Bass.ChannelSetAttribute(Channel, ChannelAttribute.Volume, Mute ? 0 : RelativeVolume);
            Bass.ChannelSetAttribute(Channel, ChannelAttribute.Pan, RelativePan);

            if (PlaybackState == Base.PlaybackState.Stalled)
                Dispose();
        }

        #endregion

        #region Effects

        private List<IEffect> Effects;

        /// <summary>
        /// Add an effect to this object.
        /// </summary>
        /// <param name="Target">Effect.</param>
        public void AddEffect(IEffect Target, int Priority = 0)
        {
            if (State != ResourceState.Loaded)
            {
                Loaded += (IResource R) =>
                {
                    if (Effects.Contains(Target))
                        return;

                    Target.BindStream(SampleHandle, Priority);

                    Effects.Add(Target);
                };
            }
            else
            {
                if (Effects.Contains(Target))
                    return;

                Target.BindStream(SampleHandle, Priority);

                Effects.Add(Target);
            }
        }

        /// <summary>
        /// Remove an effect to this object.
        /// </summary>
        /// <param name="Target">Effect to remove.</param>
        public void RemoveEffect(IEffect Target)
        {
            if (!Effects.Contains(Target))
                return;

            Target.UnbindStream(SampleHandle);

            Effects.Remove(Target);
        }

        #endregion

        #region Handle management

        private int SampleHandle;
        private int Channel;

        /// <summary>
        /// Stream handler for mixing.
        /// </summary>
        public int StreamHandler { get { return Channel; } }

        private void LoadSample()
        {
            // Sample loading
            SampleHandle = Bass.SampleLoad(AudioFile.Content, 0, (int)AudioFile.Size, 1, BassFlags.Default);

            // Channel management
            Channel = Bass.SampleGetChannel(SampleHandle);
        }

        #endregion

        #region Resource

        public override uint UsedSpace { get; }

		protected override void OnLoad()
		{
			LoadSample();
		}

		#endregion

	}
}
