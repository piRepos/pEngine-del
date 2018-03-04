using System;

using pEngine.Timing.Base;

namespace pEngine.Timing.Clocks
{
    using Math = System.Math;

    public class BeatClock : InterpolatedFramedClock, IBeatClock
	{
		/// <summary>
		/// Makes a new <see cref="BeatClock"/>.
		/// </summary>
		/// <param name="source">Source clock.</param>
		public BeatClock(IClock source)
			: base(source)
		{
			
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		public override double CurrentTime => base.CurrentTime + Offset;

		/// <summary>
		/// Gets or sets the time offset in ms.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		/// Gets or sets the beats per minute.
		/// </summary>
		public double BeatsPerMinute { get; set; }

		/// <summary>
		/// Gets or sets the signature.
		/// </summary>
		public TimeSignature Signature { get; set; }

		/// <summary>
		/// Occurs when there's a normal beat in the signature.
		/// </summary>
		public event Action OnLowBeat;

		/// <summary>
		/// Occurs when there's an accented beat in the signature.
		/// </summary>
		public event Action OnHighBeat;

		#region Update

		private int CurrentClic = 0;
		private double LastTime = double.MaxValue;

		public override void ProcessFrame()
		{
			base.ProcessFrame();

			double MeasureDuration = 60D / BeatsPerMinute;

			if (LastTime > Mod(CurrentTime, MeasureDuration))
			{
				if (CurrentClic == (int)Signature)
				{
					CurrentClic = 0;
					OnHighBeat?.Invoke();
				}
				else OnLowBeat?.Invoke();

				CurrentClic++;
			}

			LastTime = Mod(CurrentTime, MeasureDuration);

		}

		double Mod(double x, double m)
		{
			return x - Math.Floor(x / m) * m;
		}

		#endregion
	}
}
