// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

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
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// Reproduction frequency. (affect pitch, 1 = normal frequency)
        /// </summary>
        double Frequency { get; set; }

    }
}
