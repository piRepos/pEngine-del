using System;

using pEngine.Core.Data;
using pEngine.Core.Audio.DSP;
using pEngine.Core.Data.Files;

using pEngine.Common.Timing.Base;

namespace pEngine.Core.Audio.Playable
{
	using Audio = Core.Data.Files.File;

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
		public virtual bool IsRunning => State == Base.PlaybackState.Playing;
	}
}
