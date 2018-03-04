using System;

using pEngine.Timing.Base;

namespace pEngine.Timing.Clocks
{
	/// <summary>
	/// Takes a clock source and separates time reading on a per-frame level.
	/// The CurrentTime value will only change on initial construction and whenever ProcessFrame is run.
	/// </summary>
	public class FramedClock : IFrameBasedClock
	{
		public IClock Source { get; }

		/// <summary>
		/// Construct a new FramedClock with an optional source clock.
		/// </summary>
		/// <param name="source">A source clock which will be used as the backing time source. If null, a StopwatchClock will be created. When provided, the CurrentTime of <see cref="source" /> will be transferred instantly.</param>
		public FramedClock(IClock source = null)
		{
			if (source != null)
			{
				CurrentTime = LastFrameTime = source.CurrentTime;
				Source = source;
			}
			else
				Source = new StopwatchClock(true);
		}
		
		/// <summary>
		/// Current average execution time for the frames.
		/// </summary>
		public double AverageFrameTime { get; private set; }

		/// <summary>
		/// Frames per seconds.
		/// </summary>
		public double FramesPerSecond { get; private set; }

		/// <summary>
		/// Elapsed time from clock start.
		/// </summary>
		public virtual double CurrentTime { get; protected set; }

		/// <summary>
		/// Last frame time executiom.
		/// </summary>
		protected virtual double LastFrameTime { get; set; }

		/// <summary>
		/// The rate this clock is running at.
		/// </summary>
		public double Rate => Source.Rate;

		/// <summary>
		/// Time elapsed from the last frame execution.
		/// </summary>
		public double ElapsedFrameTime => CurrentTime - LastFrameTime;

		/// <summary>
		/// True if this clock is running.
		/// </summary>
		public bool IsRunning => Source?.IsRunning ?? false;

		#region Calculation

		protected double SourceTime => Source.CurrentTime;
		
		private double timeUntilNextCalculation;
		private double timeSinceLastCalculation;
		private int framesSinceLastCalculation;

		/// <summary>
		/// Whether we should run <see cref="ProcessFrame"/> on the underlying <see cref="Source"/> (in the case it is an <see cref="IFrameBasedClock"/>).
		/// </summary>
		public bool ProcessSourceClockFrames = true;

		private const int fps_calculation_interval = 250;

		public virtual void ProcessFrame()
		{
			if (ProcessSourceClockFrames)
				(Source as IFrameBasedClock)?.ProcessFrame();

			if (timeUntilNextCalculation <= 0)
			{
				timeUntilNextCalculation += fps_calculation_interval;

				if (framesSinceLastCalculation == 0)
					FramesPerSecond = 0;
				else
					FramesPerSecond = (int)System.Math.Ceiling(framesSinceLastCalculation * 1000f / timeSinceLastCalculation);
				timeSinceLastCalculation = framesSinceLastCalculation = 0;
			}

			framesSinceLastCalculation++;
			timeUntilNextCalculation -= ElapsedFrameTime;
			timeSinceLastCalculation += ElapsedFrameTime;

			AverageFrameTime = Utils.Math.Interpolation.Damp(AverageFrameTime, ElapsedFrameTime, 0.01, System.Math.Abs(ElapsedFrameTime) / 1000);

			LastFrameTime = CurrentTime;
			CurrentTime = SourceTime;
		}

		#endregion
	}

}
