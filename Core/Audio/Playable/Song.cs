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
using ManagedBass.Fx;

using pEngine.Core.Data;
using pEngine.Core.Audio.DSP;
using pEngine.Core.Data.Files;
using pEngine.Core.Audio.Base;


namespace pEngine.Core.Audio.Playable
{
    using Audio = Core.Data.Files.File;
	using PlaybackState = ManagedBass.PlaybackState;

    public class Song : Resource, IPlayableComponent
    {

        public Song(Audio AudioResource, bool Preview = false)
        {
            Effects = new List<IEffect>();

            this.Preview = Preview;

            // Dependency
            Dependencies.Add(AudioResource);
            AudioFile = AudioResource;
        }

        public override void Dispose()
        {
            Stop();

            if (CleanStream != 0) Bass.StreamFree(CleanStream);

            Stream = 0;
            CleanStream = 0;

            base.Dispose();
        }

        #region Properties

        private float InitialFrequency = 0F;

        private float FrequencyInternal = 1F;
        private double TempoInternal = 1D;
        private double PitchInternal = 0D;
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

                FrequencyInternal = (float)value;

                UpdateState();
            }
        }

        /// <summary>
        /// Reproduction tempo. (not affect pitch, 1 = Normal tempo, 0 = Stop)
        /// </summary>
        public double Tempo
        {
            get { return TempoInternal; }
            set
            {
                if (TempoInternal == value)
                    return;

                TempoInternal = value;

                UpdateState();
            }
        }


        /// <summary>
        /// Gets or Sets the Pitch in Semitones (-60 ... 0 ... 60).
        /// </summary>
        public double Pitch
        {
            get { return PitchInternal; }
            set
            {
                if (PitchInternal == value)
                    return;

                PitchInternal = value;

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

                if (Mute)
                    return;

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

        /// <summary>
        /// If this song will be pre loaded or not.
        /// </summary>
        public bool Preview { get; }

        /// <summary>
        /// If this song is in play state.
        /// </summary>
        public bool IsPlaying { get { return State == Base.PlaybackState.Playing; } }

        /// <summary>
        /// Song length in milliseconds.
        /// </summary>
        public double Length { get; protected set; }

        /// <summary>
        /// Current song position in milliseconds.
        /// </summary>
        public double CurrentTime
        {
            get
            {
                return Bass.ChannelBytes2Seconds(Stream, Bass.ChannelGetPosition(Stream)) * 1000;
            }
        }

        /// <summary>
        /// Song bitrate.
        /// </summary>
        public int Bitrate
        {
            get
            {
                return (int)Bass.ChannelGetAttribute(Stream, ChannelAttribute.Bitrate);
            }
        }

        /// <summary>
        /// Song rate.
        /// </summary>
        public double Rate
        {
            get
            {
                return BassFreq / InitialFrequency * Tempo;
            }
        }

        #endregion

        #region State

        private int Direction = 0;

        /// <summary>
        /// Reproduce the current sound.
        /// </summary>
        public void Start()
        {
            if (!IsLoaded)
                return;

            UpdateState();

            Bass.ChannelPlay(Stream);
        }

        /// <summary>
        /// Stop reproduction and reset position.
        /// </summary>
        public void Stop()
        {
            if (!IsLoaded)
                return;

            if (State == Base.PlaybackState.Playing)
                TogglePause();

            Seek(0);
        }

        /// <summary>
        /// Stop/play reproduction without change position.
        /// </summary>
        public void TogglePause()
        {
            if (!IsLoaded)
                return;

            if (Base.PlaybackState.Playing == State)
                Bass.ChannelPause(Stream);
            else
                Bass.ChannelPlay(Stream, false);
        }

        /// <summary>
        /// Reset current audio.
        /// </summary>
        public void Reset()
        {
            Stop();
            Seek(0);
            Volume = 100D;
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

				return (Base.PlaybackState)Bass.ChannelIsActive(Stream);
            }
        }

        /// <summary>
        /// Set song position to SeekTime value.
        /// </summary>
        /// <param name="SeekTime">Position in milliseconds.</param>
        /// <returns>False if this is an invalid position.</returns>
        public bool Seek(double SeekTime)
        {
            double Clamped = Math.Min(Math.Max(SeekTime, 0), Length);

            if (Clamped != CurrentTime)
            {
                long Pos = Bass.ChannelSeconds2Bytes(Stream, Clamped / 1000d);
                Bass.ChannelSetPosition(Stream, Pos);
            }

            return Clamped == SeekTime;
        }

        /// <summary>
        /// Reverse direction effect.
        /// </summary>
        /// <param name="Reverse">If enable this effect.</param>
        public void SetDirection(bool Reverse)
        {
            int NewDirection = Reverse ? -1 : 1;

            if (Direction == NewDirection)
                return;

            Direction = NewDirection;

            Bass.ChannelSetAttribute(Stream, ChannelAttribute.ReverseDirection, Direction);
        }

        /// <summary>
        /// Update reproduction parameters.
        /// </summary>
        public void UpdateState()
        {
            Bass.ChannelSetAttribute(Stream, ChannelAttribute.Frequency, BassFreq);
            Bass.ChannelSetAttribute(TempoStream, ChannelAttribute.Tempo, (Tempo - 1) * 100);
            Bass.ChannelSetAttribute(TempoStream, ChannelAttribute.Pitch, Pitch);
            Bass.ChannelSetAttribute(Stream, ChannelAttribute.Volume, Mute ? 0 : RelativeVolume);
            Bass.ChannelSetAttribute(Stream, ChannelAttribute.Pan, RelativePan);
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

                    Target.Bind(Stream, Priority);

                    Effects.Add(Target);
                };
            }
            else
            {
                if (Effects.Contains(Target))
                    return;

                Target.Bind(Stream, Priority);

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

            Target.Unbind(Stream);

            Effects.Remove(Target);
        }

        #endregion

        #region Handler management

        private Audio AudioFile;

        private int TempoStream;
        private int CleanStream;
        private int Stream;

        /// <summary>
        /// Stream handler for mixing.
        /// </summary>
        public int StreamHandler { get { return CleanStream; } }

        private void LoadSong()
        {
            BassFlags Flags = Preview ? 0 : (BassFlags.Decode | BassFlags.Prescan);

            // Create a new stream
            CleanStream = Bass.CreateStream(AudioFile.Content, 0, AudioFile.Size, Flags);

            if (!Preview)
            {
                Stream = BassFx.TempoCreate(CleanStream, BassFlags.Decode | BassFlags.FxFreeSource);

                TempoStream = Stream;

                Stream = BassFx.ReverseCreate(Stream, 5f, BassFlags.FxFreeSource);

                Bass.ChannelSetAttribute(Stream, ChannelAttribute.TempoUseQuickAlgorithm, 1);
                Bass.ChannelSetAttribute(Stream, ChannelAttribute.TempoOverlapMilliseconds, 4);
                Bass.ChannelSetAttribute(Stream, ChannelAttribute.TempoSequenceMilliseconds, 30);
            }
            else Stream = CleanStream;


            Length = (Bass.ChannelBytes2Seconds(Stream, Bass.ChannelGetLength(Stream)) * 1000);
            Bass.ChannelGetAttribute(Stream, ChannelAttribute.Frequency, out InitialFrequency);
            
            SetDirection(false);
        }

        #endregion

        #region Private info

        int BassFreq => (int)Math.Min(Math.Max(Math.Abs(InitialFrequency * FrequencyInternal), 100), 100000);

		#endregion

		#region Resource

		public override uint UsedSpace => 0;

		internal override void OnLoad()
		{
			LoadSong();
		}

        #endregion
    }
}
