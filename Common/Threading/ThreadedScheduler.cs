using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace pEngine.Common.Threading
{
	/// <summary>
	/// A scheduler which doesn't require manual updates (and never uses the main thread).
	/// </summary>
	public class ThreadedScheduler : Scheduler
	{
		private bool isDisposed;

		public ThreadedScheduler(string threadName = null, int runInterval = 50)
		{
			var workerThread = new Thread(() =>
			{
				while (!isDisposed)
				{
					Update();
					Thread.Sleep(runInterval);
				}
			})
			{
				IsBackground = true,
				Name = threadName
			};

			workerThread.Start();
		}

		protected override void Dispose(bool disposing)
		{
			isDisposed = true;
			base.Dispose(disposing);
		}

		protected override bool IsMainThread => false;
	}
}
