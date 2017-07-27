using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using pEngine.Common.Diagnostic;

namespace pEngine.Debug
{
	public class DebugModule : IDisposable
	{
		/// <summary>
		/// Parent host session.
		/// </summary>
		private pEngine host;

		/// <summary>
		/// Make a new hinstance of <see cref="DebugModule"/>.
		/// </summary>
		/// <param name="host">pEngine host hinstance.</param>
		public DebugModule(pEngine host)
		{
			// - Logger initialization
			Logger = new Logger(LogLevel.Verbose);

			// - Collectors manager initialization
			Collectors = new CollectorsModule();

            // - Plugins installation
            reporters = new List<IReportClass>();
		}

        /// <summary>
        /// Releases all resource used by the <see cref="T:pEngine.Debug.DebugModule"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:pEngine.Debug.DebugModule"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:pEngine.Debug.DebugModule"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:pEngine.Debug.DebugModule"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:pEngine.Debug.DebugModule"/> was occupying.</remarks>
        public void Dispose()
        {
            foreach (var rep in reporters)
                rep.Dispose();
        }

        #region Plugins

        private List<IReportClass> reporters;

        /// <summary>
        /// All installed reporter classes.
        /// </summary>
        public IEnumerable<IReportClass> Reporters => reporters;

        /// <summary>
        /// Adds a new reporter object.
        /// </summary>
        /// <param name="reporter">Reporter.</param>
        public void AddReporter(IReportClass reporter)
        {
            reporters.Add(reporter);
            reporter.Link(this);
        }

        #endregion

        #region Performance monitors

        /// <summary>
        /// This module handle all performance collectors.
        /// </summary>
        public CollectorsModule Collectors { get; }

		/// <summary>
		/// Add a collector to the module.
		/// </summary>
		/// <param name="name">Colector name.</param>
		/// <param name="collector">The collector.</param>
		public void AddCollector(string name, PerformanceCollector collector) => Collectors.AddCollector(name, collector);

		/// <summary>
		/// Remove a collector from the performance collector module.
		/// </summary>
		/// <param name="name">Collector name.</param>
		public void RemoveCollector(string name) => Collectors.RemoveCollector(name);
		
		#endregion

		#region Logs

		/// <summary>
		/// This module manage logs.
		/// </summary>
		public Logger Logger { get; }

		/// <summary>
		/// Send a log error message.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="target">Log deploy target.</param>
		public void ErrorLog(string message, LogTarget target = LogTarget.Runtime) => Logger.Error(message, target);

		/// <summary>
		/// Send a log error message.
		/// </summary>
		/// <param name="ex">Error exception.</param>
		/// <param name="description">Exception description.</param>
		/// <param name="target">Log deploy target.</param>
		public void ExceptionLog(Exception ex, string description = "", LogTarget target = LogTarget.Runtime) => Logger.Exception(ex, description, target);

		/// <summary>
		/// Send a log critical error message.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="message">Exception description.</param>
		/// <param name="target">Log deploy target.</param>
		public void CriticalErrorLog(Exception ex, string message, LogTarget target = LogTarget.Runtime) => Logger.CriticalError(ex, message, target);

		/// <summary>
		/// Send a log warning message.
		/// </summary>
		/// <param name="message">Warning message.</param>
		/// <param name="target">Log deploy target.</param>
		public void WarningLog(string message, LogTarget target = LogTarget.Runtime) => Logger.Warning(message, target);

		/// <summary>
		/// Send a log information message.
		/// </summary>
		/// <param name="message">Information message.</param>
		/// <param name="target">Log deploy target.</param>
		public void Log(string message, LogTarget target = LogTarget.Runtime) => Logger.Log(message, target);

		/// <summary>
		/// Send a log verbose message.
		/// </summary>
		/// <param name="message">Verbose message.</param>
		/// <param name="target">Log deploy target.</param>
		public void VerboseLog(string message, LogTarget target = LogTarget.Runtime) => Logger.Verbose(message, target);

		#endregion
	}
}
