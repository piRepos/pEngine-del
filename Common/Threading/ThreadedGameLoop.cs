using System;
using System.Threading;

namespace pEngine.Common.Threading
{
    public class ThreadedGameLoop : GameLoop
    {

		public ThreadedGameLoop(TimedAction actionToDo, string threadName)
			: base(actionToDo, threadName)
		{
			// - Thread initialization
			CurrentThread = new Thread(base.Run);
		}

		/// <summary>
		/// Start this gameloop.
		/// </summary>
		public new virtual void Run()
		{
			CurrentThread.Start();
		}
	}
}
