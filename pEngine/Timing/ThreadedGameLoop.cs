using System;
using System.Threading;

namespace pEngine.Timing
{
    public class ThreadedGameLoop : GameLoop
    {
		/// <summary>
		/// Makes a new instance of <see cref="GameLoop"/> class.
		/// </summary>
		/// <param name="actionToDo">Delegate that will be executed on each frame.</param>
		/// <param name="threadName">Name of the associated thread.</param>
		public ThreadedGameLoop(TimedAction actionToDo, string threadName)
			: base(actionToDo, threadName)
		{
			// - Thread initialization
			CurrentThread = new Thread(base.Run);
		}

		/// <summary>
		/// Makes a new instance of <see cref="GameLoop"/> class.
		/// </summary>
		/// <param name="actionToDo">Delegate that will be executed on each frame.</param>
		/// <param name="threadName">Name of the associated thread.</param>
		public ThreadedGameLoop(TimedAction actionToDo, Action initialization, string threadName)
			: base(actionToDo, initialization, threadName)
		{
			// - Thread initialization
			CurrentThread = new Thread(base.Run);
		}

		/// <summary>
		/// Start this gameloop.
		/// </summary>
		public override void Run()
		{
			CurrentThread.Start();
		}
	}
}
