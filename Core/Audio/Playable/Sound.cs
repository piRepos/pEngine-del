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

using pEngine.Core.Audio.DSP;
using pEngine.Core.Audio.Base;
using pEngine.Core.Data.Files;
using pEngine.Core.Data;

namespace pEngine.Core.Audio.Playable
{
    using Audio = Core.Data.Files.File;
	using PlaybackState = ManagedBass.PlaybackState;

    public class Sound : Resource, IPlayableComponent
    {

        public Sound(Audio AudioResource, bool DisposeOnFinish = false)
        {
            Effects = new List<IEffect>();

            // Dependency
            Dependencies.Add(AudioResource);
            AudioFile = AudioResource;
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
            if (!IsLoaded)
                return;

            Bass.ChannelPlay(Channel);
        }

        /// <summary>
        /// Stop reproduction and reset position.
        /// </summary>
        public void Stop()
        {
            if (!IsLoaded)
                return;

            Bass.ChannelStop(Channel);
        }

        /// <summary>
        /// Reproduction state of this sound.
        /// </summary>
        public Base.PlaybackState State
        {
            get
            {
                if (!IsLoaded)
					return (Base.PlaybackState)PlaybackState.Stopped;

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

            if (State == Base.PlaybackState.Stalled)
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
            if (!IsLoaded)
            {
                Completed += (IResource R) =>
                {
                    if (Effects.Contains(Target))
                        return;

                    Target.Bind(SampleHandle, Priority);

                    Effects.Add(Target);
                };
            }
            else
            {
                if (Effects.Contains(Target))
                    return;

                Target.Bind(SampleHandle, Priority);

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

            Target.Unbind(SampleHandle);

            Effects.Remove(Target);
        }

        #endregion

        #region Handle management

        private Audio AudioFile;
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

		internal override void OnLoad()
		{
			LoadSample();
		}

		#endregion

	}
}
