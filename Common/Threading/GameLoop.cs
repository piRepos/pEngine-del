using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using pEngine.Common.Timing.Base;
using pEngine.Common.Timing.Clocks;
using pEngine.Common.Diagnostic;

namespace pEngine.Common.Threading
{
	/// <summary>
	/// Handle an action that have a clock.
	/// </summary>
	/// <param name="clock">Action clock.</param>
	public delegate void TimedAction(IFrameBasedClock clock);

	public class GameLoop
    {
		TimedAction loopAction;
		string threadName;

		public GameLoop(TimedAction actionToDo, string threadName)
		{
			loopAction = actionToDo;
			ExitRequest = false;
			this.threadName = threadName;

			// - Initialize performance collector
			Performance = new PerformanceCollector(threadName);

			// - Initialize scheduler
			Scheduler = new Scheduler();

			// - Initialize clock
			Clock = new ThrottledFramedClock();
		}

		/// <summary>
		/// Game loop object.
		/// </summary>
		public ThrottledFramedClock Clock { get; }

		/// <summary>
		/// Performance collector for this loop.
		/// </summary>
		public PerformanceCollector Performance { get; }

		/// <summary>
		/// Scheduler for this thread, this is usefull to invoke function
		/// on this thread from others threads.
		/// </summary>
		public Scheduler Scheduler { get; }

		/// <summary>
		/// Game loop thread.
		/// </summary>
		public Thread CurrentThread { get; protected set; }

		/// <summary>
		/// Current frame id.
		/// </summary>
		public long FrameId { get; private set; }

		/// <summary>
		/// Current loop must close?
		/// </summary>
		public bool ExitRequest { get; private set; }

		/// <summary>
		/// Check if actually i'm on the loop thread.
		/// </summary>
		public bool ImOnThisThread => Thread.CurrentThread == CurrentThread;

		/// <summary>
		/// Triggered on loop initialization.
		/// </summary>
		public event Action OnInitialize;

		/// <summary>
		/// Start this gameloop.
		/// </summary>
		public virtual void Run()
		{
			Thread.CurrentThread.Name = threadName;
			CurrentThread = Thread.CurrentThread;

			Scheduler.SetCurrentThread();

			OnInitialize?.Invoke();

			while (!ExitRequest)
				ProcessFrame();
		}

		/// <summary>
		/// Begin loop end.
		/// </summary>
		public void Stop()
		{
			ExitRequest = true;
		}

		private void ProcessFrame()
		{
			using (Performance.StartCollect("Scheduler"))
				Scheduler.Update();

			using (Performance.StartCollect("Task"))
				loopAction?.Invoke(Clock);

			using (Performance.StartCollect("Idle"))
				Clock.ProcessFrame();

			FrameId = (FrameId + 1) % long.MaxValue;
		}
    }
}
