// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Audio.DSP
{
    public interface IEffect : IDisposable
    {

        /// <summary>
        /// Bind this effect on a specified stream.
        /// </summary>
        /// <param name="Stream">Target stream.</param>
        /// <param name="Priority">Effect priority.</param>
        void Bind(int Stream, int Priority);

        /// <summary>
        /// Unbind this effect from the binded stream.
        /// </summary>
        /// <param name="Stream">Target stream.</param>
        void Unbind(int Stream);

        /// <summary>
        /// Bypass this effect.
        /// </summary>
        bool Bypass { get; set; }

    }
}
