using System;


namespace pEngine.Core.Audio.Base
{

	/// <summary>
	/// Playback state.
	/// </summary>
	public enum PlaybackState
	{
		/// <summary>
		/// Audio is in stop state.
		/// </summary>
		Stopped,

		/// <summary>
		/// Audio is reproducing.
		/// </summary>
		Playing,

		/// <summary>
		/// Audio is not played.
		/// </summary>
		Stalled,

		/// <summary>
		/// Audio is in pause.
		/// </summary>
		Paused
	}
}

