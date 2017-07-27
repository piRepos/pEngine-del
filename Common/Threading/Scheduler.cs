using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using pEngine.Common.Extensions;

namespace pEngine.Common.Threading
{
	/// <summary>
	/// Marshals delegates to run from the Scheduler's base thread in a threadsafe manner
	/// </summary>
	public class Scheduler : IDisposable
	{
		private readonly ConcurrentQueue<Action> schedulerQueue = new ConcurrentQueue<Action>();
		private readonly List<ScheduledDelegate> timedTasks = new List<ScheduledDelegate>();
		private readonly List<ScheduledDelegate> perUpdateTasks = new List<ScheduledDelegate>();
		private int mainThreadId;
		private readonly Stopwatch timer = new Stopwatch();

		/// <summary>
		/// The base thread is assumed to be the the thread on which the constructor is run.
		/// </summary>
		public Scheduler()
		{
			SetCurrentThread();
			timer.Start();
		}

		/// <summary>
		/// The base thread is assumed to be the the thread on which the constructor is run.
		/// </summary>
		public Scheduler(Thread mainThread)
		{
			SetCurrentThread(mainThread);
			timer.Start();
		}

		/// <summary>
		/// Returns whether we are on the main thread or not.
		/// </summary>
		protected virtual bool IsMainThread => Thread.CurrentThread.ManagedThreadId == mainThreadId;

		/// <summary>
		/// Run any pending work tasks.
		/// </summary>
		/// <returns>true if any tasks were run.</returns>
		public int Update()
		{
			//purge any waiting timed tasks to the main schedulerQueue.
			lock (timedTasks)
			{
				long currentTime = timer.ElapsedMilliseconds;
				ScheduledDelegate sd;

				while (timedTasks.Count > 0 && (sd = timedTasks[0]).WaitTime <= currentTime)
				{
					timedTasks.RemoveAt(0);
					if (sd.Cancelled) continue;

					schedulerQueue.Enqueue(sd.RunTask);

					if (sd.RepeatInterval > 0)
					{
						if (timedTasks.Count < 1000)
							sd.WaitTime += sd.RepeatInterval;
						// This should never ever happen... but if it does, let's not overflow on queued tasks.
						else
						{
							// TODO: Debug.Print("Timed tasks are overflowing. Can not keep up with periodic tasks.");
							sd.WaitTime = timer.ElapsedMilliseconds + sd.RepeatInterval;
						}

						timedTasks.AddInPlace(sd);
					}
				}

				for (int i = 0; i < perUpdateTasks.Count; i++)
				{
					ScheduledDelegate task = perUpdateTasks[i];
					if (task.Cancelled)
					{
						perUpdateTasks.RemoveAt(i--);
						continue;
					}

					schedulerQueue.Enqueue(task.RunTask);
				}
			}

			int countRun = 0;

			Action action;
			while (schedulerQueue.TryDequeue(out action))
			{
				action?.Invoke();
				countRun++;
			}

			return countRun;
		}

		internal void SetCurrentThread(Thread thread)
		{
			mainThreadId = thread?.ManagedThreadId ?? -1;
		}

		internal void SetCurrentThread()
		{
			mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		/// <summary>
		/// Add a task to be scheduled.
		/// </summary>
		/// <param name="task">The work to be done.</param>
		/// <param name="forceScheduled">If set to false, the task will be executed immediately if we are on the main thread.</param>
		/// <returns>Whether we could run without scheduling</returns>
		public virtual bool Add(Action task, bool forceScheduled = true)
		{
			if (!forceScheduled && IsMainThread)
			{
				//We are on the main thread already - don't need to schedule.
				task.Invoke();
				return true;
			}

			schedulerQueue.Enqueue(task);

			return false;
		}

		public virtual bool Add(ScheduledDelegate task)
		{
			lock (timedTasks)
			{
				if (task.RepeatInterval == 0)
					perUpdateTasks.Add(task);
				else
					timedTasks.AddInPlace(task);
			}
			return true;
		}

		/// <summary>
		/// Add a task which will be run after a specified delay.
		/// </summary>
		/// <param name="task">The work to be done.</param>
		/// <param name="timeUntilRun">Milliseconds until run.</param>
		/// <param name="repeat">Whether this task should repeat.</param>
		public ScheduledDelegate AddDelayed(Action task, double timeUntilRun, bool repeat = false)
		{
			ScheduledDelegate del = new ScheduledDelegate(task, timer.ElapsedMilliseconds + timeUntilRun, repeat ? timeUntilRun : -1);

			return Add(del) ? del : null;
		}

		/// <summary>
		/// Adds a task which will only be run once per frame, no matter how many times it was scheduled in the previous frame.
		/// </summary>
		/// <param name="task">The work to be done.</param>
		/// <returns>Whether this is the first queue attempt of this work.</returns>
		public bool AddOnce(Action task)
		{
			if (schedulerQueue.Contains(task))
				return false;

			schedulerQueue.Enqueue(task);

			return true;
		}

		#region IDisposable Support

		private bool isDisposed; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
