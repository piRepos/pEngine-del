using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace pEngine.Utils.Threading
{
	/// <summary>
	/// A scheduler which doesn't require manual updates (and never uses the main thread).
	/// </summary>
	public class ThreadedScheduler : Scheduler
	{
		/// <summary>
		/// Makes a new instance of <see cref="ThreadedScheduler"/> class.
		/// </summary>
		/// <param name="threadName">Thread name.</param>
		/// <param name="runInterval">Time between each loop.</param>
		public ThreadedScheduler(string threadName = null, int runInterval = 50)
		{
			WorkerThread = new Thread(() =>
			{
				while (Running && !isDisposed)
				{
					Update();
					Thread.Sleep(runInterval);
				}
			})
			{
				IsBackground = true,
				Name = threadName
			};
		}

		protected override bool IsMainThread => false;

		/// <summary>
		/// Scheduler thread.
		/// </summary>
		public Thread WorkerThread { get; }

		/// <summary>
		/// True if this thread is running.
		/// </summary>
		public bool Running { get; private set; }
		
		/// <summary>
		/// Start this threaded scheduler.
		/// </summary>
		public void Run()
		{
			Running = true;
			WorkerThread.Start();
		}

		/// <summary>
		/// Stop this thread.
		/// </summary>
		/// <param name="join">Wait for the thread exit.</param>
		/// <param name="force">Force the thread exit.</param>
		public void Stop(bool join = false, bool force = false)
		{
			Running = false;

			if (force)
				WorkerThread.Abort();

			if (join)
				WorkerThread.Join();
		}

		#region Dispose implementation

		private bool isDisposed;

		protected override void Dispose(bool disposing)
		{
			isDisposed = true;
			base.Dispose(disposing);
		}

		#endregion

	}
}
