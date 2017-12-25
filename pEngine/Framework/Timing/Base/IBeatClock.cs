using System;

namespace pEngine.Framework.Timing.Base
{

	/// <summary>
	/// Time signatures.
	/// </summary>
	public enum TimeSignature
	{
		/// <summary>
		/// Common time for rock pop etc...
		/// </summary>
		FourQuarters = 4,

		/// <summary>
		/// 3/4 signature.
		/// </summary>
		ThreeQuarters = 3,

		/// <summary>
		/// 2/4 signature, marches, polka...
		/// </summary>
		TwoQuarters = 2,

		/// <summary>
		/// 3/8 signature.
		/// </summary>
		ThreeOctaves = 6,

		/// <summary>
		/// 6/8 Tarantellaaa.
		/// </summary>
		SixOctaves = 12,

		/// <summary>
		/// 9/8 Compound time.
		/// </summary>
		NineOctaves = 18,

		/// <summary>
		/// 12/8 Slow blues.
		/// </summary>
		TwelveOctaves = 24
	}

	/// <summary>
	/// A clock that has events for music timing.
	/// </summary>
	public interface IBeatClock : IClock
	{

		/// <summary>
		/// Gets or sets the beats BPM value.
		/// </summary>
		double BeatsPerMinute { get; set; }

		/// <summary>
		/// Gets or sets the time offset in ms.
		/// </summary>
		double Offset { get; set; }

		/// <summary>
		/// Gets or sets the time signature.
		/// </summary>
		TimeSignature Signature { get; set; }

		/// <summary>
		/// Occurs when there's a normal beat in the signature.
		/// </summary>
		event Action OnLowBeat;

		/// <summary>
		/// Occurs when there's an accented beat in the signature.
		/// </summary>
		event Action OnHighBeat;
	}
}
