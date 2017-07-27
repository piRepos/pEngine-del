// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManagedBass;

namespace pEngine.Core.Audio
{
    public struct AudioDevice
    {

        DeviceInfo RawInfo;

        public AudioDevice(DeviceInfo Informations, int Index)
        {
            Driver = Informations.Driver;
            IsDefault = Informations.IsDefault;
            IsEnabled = Informations.IsEnabled;
            IsInitialized = Informations.IsInitialized;
            Name = Informations.Name;
            DeviceType = Informations.Type;
            this.Index = Index;

            RawInfo = Informations;
        }

        /// <summary>
        /// Device index.
        /// </summary>
        public int Index { get; }

        #region Properties

        /// <summary>
        /// Output device name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Filename of driver used.
        /// </summary>
        public string Driver { get; }

        /// <summary>
        /// If this device is disabled or enabled from system.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// If this is the default system device.
        /// </summary>
        public bool IsDefault { get; }

        /// <summary>
        /// If this device is initialized.
        /// </summary>
        public bool IsInitialized { get; }

        /// <summary>
        /// If this device can be used.
        /// </summary>
        public bool IsValid { get { return IsInitialized && IsEnabled; } }

        /// <summary>
        /// Device type.
        /// </summary>
        public DeviceType DeviceType { get; }

        #endregion

        public static implicit operator DeviceInfo(AudioDevice Device) 
        {
            return Device.RawInfo;
        }

        public override string ToString()
        {
            return $"{Name} @ {Driver}";
        }

    }
}
