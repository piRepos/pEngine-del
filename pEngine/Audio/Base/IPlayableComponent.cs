// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using ManagedBass;

namespace pEngine.Audio.Base
{
    public interface IPlayableComponent : IMixableComponent
    {

        /// <summary>
        /// Reproduce the current sound.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop reproduction and reset position.
        /// </summary>
        void Stop();

        /// <summary>
        /// Reproduction state of this sound.
        /// </summary>
        PlaybackState State { get; }

        /// <summary>
        /// Reproduction frequency. (affect pitch, 1 = normal frequency)
        /// </summary>
        double Frequency { get; set; }

    }
}
