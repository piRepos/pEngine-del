// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Text;
using System.Collections.Generic;

using ManagedBass;

using pEngine.Framework.Timing.Base;
using pEngine.Framework.Timing;
using pEngine.Audio.Mixing;

namespace pEngine.Audio
{
    public delegate void AudioDevicesChangedEventHandler(List<AudioDevice> CurrentDevices);

    public class AudioEngine : IDisposable, IUpdatable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEngine"/> class.
        /// </summary>
        public AudioEngine()
        {

			Frequency = 44100;

            Bass.FloatingPointDSP = true;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Initialize()
        {
            // Initialize device informations
            AudioDevice CurrentDevice = GetDevices().Find((X) => X.IsDefault);

            SetAudioDevice(CurrentDevice);

            MasterMixer = new Mixer();
        }

        /// <summary>
        /// Update the audio system.
        /// </summary>
        /// <param name="Delta">Delta time.</param>
        public void Update(IFrameBasedClock Clock)
        {
            DeviceAutoCheckTimer += Clock.ElapsedFrameTime / 1000D;
            if (DeviceAutoCheckTimer > 3D)
            {
                CheckAudioDeviceChanged();
                DeviceAutoCheckTimer = 0;
            }

            MasterMixer.UpdateState();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="pEngine.Audio.AudioEngine"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="pEngine.Audio.AudioEngine"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="pEngine.Audio.AudioEngine"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="pEngine.Audio.AudioEngine"/> so
        /// the garbage collector can reclaim the memory that the <see cref="pEngine.Audio.AudioEngine"/> was occupying.</remarks>
        public void Dispose()
        {
            Bass.Free();
        }

        #region Devices

        private AudioDevice CurrentDevice;
        private bool DefaultDeviceSelector;
        private int LastAvaiableDevices;
        private double DeviceAutoCheckTimer;

        /// <summary>
        /// Change on device list.
        /// </summary>
        public event AudioDevicesChangedEventHandler onDeviceChange;

        /// <summary>
        /// Current device.
        /// </summary>
        public AudioDevice DeviceEnabled { get { return CurrentDevice; } }

        /// <summary>
        /// Set automatically the default device.
        /// </summary>
        public bool DefaultDevice
        {
            get { return DefaultDeviceSelector; }
            set
            {
                if (value == DefaultDeviceSelector)
                    return;
                DefaultDeviceSelector = value;

                AudioDevice D = new AudioDevice(Bass.GetDeviceInfo(Bass.DefaultDevice), Bass.DefaultDevice);

                SetAudioDevice(D);
            }
        }

        /// <summary>
        /// Gets alla avaiable devices.
        /// </summary>
        /// <returns>List of devices.</returns>
        public List<AudioDevice> GetDevices()
        {
            int Count = Bass.DeviceCount;

            List<AudioDevice> Devices = new List<AudioDevice>();
            for (int i = 0; i < Count; i++)
                Devices.Add(new AudioDevice(Bass.GetDeviceInfo(i), i));

            return Devices;
        }

        /// <summary>
        /// Sets an audio device to current output.
        /// </summary>
        /// <param name="Device">Device to set.</param>
        /// <returns>True if this device is setted.</returns>
        public bool SetAudioDevice(AudioDevice Device)
        {
            if (Device.Name == CurrentDevice.Name)
                return true;

            bool OldDeviceVaild = CurrentDevice.IsEnabled && CurrentDevice.IsInitialized;

            if (OldDeviceVaild && (Device.Driver == null || !Device.IsEnabled))
            {
                // handles the case we are trying to load a user setting which is currently unavailable,
                // and we have already fallen back to a sane default.
                return true;
            }

            // Clear current cache
            Bass.Free();
            
            if (!Bass.Init(Device.Index, Frequency))
            {
                // let's try again using the default device.
                SetAudioDevice(CurrentDevice);

                return false;
            }

            // we have successfully initialised a new device.
            CurrentDevice = Device;

            Bass.PlaybackBufferLength = 100;
            Bass.UpdatePeriod = 5;

            return true;
        }

        private void CheckAudioDeviceChanged()
        {
            if (DefaultDevice)
            {
                int CurrentDevice = Bass.CurrentDevice;
                try
                {
                    DeviceInfo Device = Bass.GetDeviceInfo(CurrentDevice);
                    if (Device.IsDefault && Device.IsEnabled)
                        return;
                }
                catch
                {
                    return;
                }
            }

            int availableDevices = 0;

            foreach (AudioDevice Device in GetDevices())
            {
                if (Device.Driver == null) continue;

                bool IsCurrentDevice = Device.Name == CurrentDevice.Name;

                if (Device.IsEnabled)
                {
                    if (IsCurrentDevice && !Device.IsDefault && DefaultDevice)
                    {
                        AudioDevice D = new AudioDevice(Bass.GetDeviceInfo(Bass.DefaultDevice), Bass.DefaultDevice);

                        SetAudioDevice(D);
                    }

                    availableDevices++;
                }
            }

            if (LastAvaiableDevices != availableDevices && LastAvaiableDevices > 0)
                onDeviceChange?.Invoke(GetDevices());

            LastAvaiableDevices = availableDevices;
        }

        #endregion

        #region Master mixer

        /// <summary>
        /// Main game mixer.
        /// </summary>
        public Mixer MasterMixer { get; protected set; }

        #endregion

        #region Parameters

        /// <summary>
        /// Gets or sets the audio frequency.
        /// </summary>
        /// <value>The audio frequency.</value>
        public int Frequency { get; }

        #endregion
    }
}

