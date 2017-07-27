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

using pEngine.Core.Audio.DSP;

namespace pEngine.Core.Audio.Base
{
    public interface IMixableComponent
    {

        /// <summary>
        /// Volume of this object.
        /// </summary>
        double Volume { get; set; }
        
        /// <summary>
        /// Sound orientation.
        /// </summary>
        double Pan { get; set; }

        /// <summary>
        /// Mute this element.
        /// </summary>
        bool Mute { get; set; }

        /// <summary>
        /// Stream handler for mixing.
        /// </summary>
        int StreamHandler { get; }

        /// <summary>
        /// Real volume value.
        /// </summary>
        double RelativeVolume { get; }

        /// <summary>
        /// Real Sound orientation.
        /// </summary>
        double RelativePan { get; }

        /// <summary>
        /// Hierarchy parent element.
        /// </summary>
        IMixableComponent Parent { get; set; }

        /// <summary>
        /// Update all stream values.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Add an effect to this object.
        /// </summary>
        /// <param name="Target">Effect.</param>
        void AddEffect(IEffect Target, int Priority);

        /// <summary>
        /// Remove an effect to this object.
        /// </summary>
        /// <param name="Target">Effect to remove.</param>
        void RemoveEffect(IEffect Target);

    }
}
