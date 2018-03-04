// Copyright (c) 2016 PK IT Andrea Demontis
//
//		pEngine / 2D Graphic engine for rythm games.
//

using pEngine.Timing.Base;

namespace pEngine.Timing
{
    public interface IUpdatable
    {
		/// <summary>
		/// Update the state of this element
		/// </summary>
		/// <param name="DeltaTime">Game clock.</param>
		void Update(IFrameBasedClock clock);
    }
}
