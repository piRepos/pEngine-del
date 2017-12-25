using System;

using pEngine.Resources;
using pEngine.Audio.DSP;
using pEngine.Resources.Files;

using pEngine.Framework.Timing.Base;

namespace pEngine.Audio.Playable
{
	using Audio = File;

	public class TimedSong : Song, IAdjustableClock
	{
		public TimedSong(Audio AudioResource, bool Preview = false)
			: base(AudioResource, Preview)
		{
			
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:pEngine.Audio.Playable.TimedSong"/> is running.
		/// </summary>
		/// <value><c>true</c> if is running; otherwise, <c>false</c>.</value>
		public virtual bool IsRunning => PlaybackState == Base.PlaybackState.Playing;
	}
}
