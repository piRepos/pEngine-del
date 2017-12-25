using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Utils.Threading
{
	public class ScheduledDelegate : IComparable<ScheduledDelegate>
	{
		public ScheduledDelegate(Action task, double waitTime, double repeatInterval = -1)
		{
			WaitTime = waitTime;
			RepeatInterval = repeatInterval;
			this.task = task;
		}

		/// <summary>
		/// The work task.
		/// </summary>
		private readonly Action task;

		/// <summary>
		/// Set to true to skip scheduled executions until we are ready.
		/// </summary>
		internal bool Waiting;

		public void Wait()
		{
			Waiting = true;
		}

		public void Continue()
		{
			Waiting = false;
		}

		public void RunTask()
		{
			if (!Waiting)
				task();
			Completed = true;
		}

		public bool Completed;

		public bool Cancelled { get; private set; }

		public void Cancel()
		{
			Cancelled = true;
		}

		/// <summary>
		/// Time before execution. Zero value will run instantly.
		/// </summary>
		public double WaitTime;

		/// <summary>
		/// Time between repeats of this task. -1 means no repeats.
		/// </summary>
		public double RepeatInterval;

		public int CompareTo(ScheduledDelegate other)
		{
			return WaitTime == other.WaitTime ? -1 : WaitTime.CompareTo(other.WaitTime);
		}
	}
}
