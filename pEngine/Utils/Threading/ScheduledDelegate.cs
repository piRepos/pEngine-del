using System;
using System.Collections.Generic;
using System.Text;

namespace pEngine.Utils.Threading
{
	public class ScheduledDelegate : IComparable<ScheduledDelegate>
	{
		/// <summary>
		/// Makes a new instance of <see cref="ScheduledDelegate"/> class.
		/// </summary>
		/// <param name="task">Task to execute.</param>
		/// <param name="waitTime">Optional delay.</param>
		/// <param name="repeatInterval">Repeat interval (-1 if not repeat).</param>
		public ScheduledDelegate(Action task, double waitTime, double repeatInterval = -1)
		{
			WaitTime = waitTime;
			RepeatInterval = repeatInterval;
			this.task = task;
		}

		/// <summary>
		/// True if task is completed.
		/// </summary>
		public bool Completed { get; set; }

		/// <summary>
		/// True if task is aborted.
		/// </summary>
		public bool Cancelled { get; private set; }

		/// <summary>
		/// Time before execution. Zero value will run instantly.
		/// </summary>
		public double WaitTime { get; set; }

		/// <summary>
		/// Time between repeats of this task. -1 means no repeats.
		/// </summary>
		public double RepeatInterval { get; set; }

		/// <summary>
		/// The work task.
		/// </summary>
		private readonly Action task;

		/// <summary>
		/// Set to true to skip scheduled executions until we are ready.
		/// </summary>
		internal bool Waiting;

		/// <summary>
		/// Skip skeduling until we are ready.
		/// </summary>
		public void Wait()
		{
			Waiting = true;
		}

		/// <summary>
		/// Resume skeduling from a wait command.
		/// </summary>
		public void Continue()
		{
			Waiting = false;
		}

		/// <summary>
		/// Executed the scheduled task.
		/// </summary>
		public void RunTask()
		{
			if (!Waiting)
				task();
			Completed = true;
		}

		/// <summary>
		/// Abort this scheduling.
		/// </summary>
		public void Cancel()
		{
			Cancelled = true;
		}

		/// <summary>
		/// Compare this <see cref="ScheduledDelegate"/> time to another.
		/// </summary>
		/// <param name="other">The other schedule.</param>
		public int CompareTo(ScheduledDelegate other)
		{
			return WaitTime == other.WaitTime ? -1 : WaitTime.CompareTo(other.WaitTime);
		}
	}
}
