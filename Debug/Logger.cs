using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace pEngine.Debug
{

	public class LogEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:pEngine.Core.Stats.LogEventArgs"/> class.
		/// </summary>
		/// <param name="Ex">Exception optional.</param>
		/// <param name="Description">Description optional.</param>
		/// <param name="Trace">Trace.</param>
		public LogEventArgs(Exception Ex, string Description, LogType Type, StackTrace Trace)
		{
			Time = DateTime.Now;
			Exception = Ex;
			this.Description = Description;
			this.Trace = Trace;
			this.Type = Type;
		}

		/// <summary>
		/// Gets the exception.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets the stack trace.
		/// </summary>
		public StackTrace Trace { get; }

		/// <summary>
		/// Gets the log time.
		/// </summary>
		public DateTime Time { get; }

		/// <summary>
		/// Gets the log type.
		/// </summary>
		public LogType Type { get; }

		/// <summary>
		/// Gets the log description.
		/// </summary>
		public string Description { get; }
	}

	/// <summary>
	/// Log event handler.
	/// </summary>
	public delegate void LogEventHandler(object sender, LogEventArgs e);

	/// <summary>
	/// Log level.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Shows noly errors.
		/// </summary>
		Errors,

		/// <summary>
		/// Shows errors and warnings.
		/// </summary>
		Warnings,

		/// <summary>
		/// Show all.
		/// </summary>
		Verbose
	}

	/// <summary>
	/// Log types.
	/// </summary>
	public enum LogType
	{
		/// <summary>
		/// Exception log.
		/// </summary>
		Exception,

		/// <summary>
		/// Error log.
		/// </summary>
		CriticalError,

		/// <summary>
		/// Error log.
		/// </summary>
		Error,

		/// <summary>
		/// Warning log.
		/// </summary>
		Warning,

		/// <summary>
		/// Generic information.
		/// </summary>
		Information,

		/// <summary>
		/// Per frame information. (Can slow down the system)
		/// </summary>
		Verbose

	}

    /// <summary>
    /// Log target.
    /// </summary>
	public enum LogTarget
	{

		/// <summary>
		/// Shown on normal runtime.
		/// </summary>
		Runtime,

		/// <summary>
		/// Shown only in debug mode.
		/// </summary>
		Debug

	}

	public class Logger
	{

		/// <summary>
		/// Initialize the logger.
		/// </summary>
		/// <param name="Level">Log level.</param>
		public Logger(LogLevel Level)
		{
			if (Initialized)
				return;

			LogLevel = Level;

			Log("Logger initialized.");

			Initialized = true;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:pEngine.Core.Stats.Logger"/> is initialized.
		/// </summary>
		public bool Initialized { get; private set; }

		/// <summary>
		/// Gets or sets the log level.
		/// </summary>
		public LogLevel LogLevel { get; set; }

		/// <summary>
		/// Occurs when the engine or the game send a log.
		/// </summary>
		public event LogEventHandler OnLog;

		#region Log functions

		/// <summary>
		/// Send a log error message.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="target">Log deploy target.</param>
		public void Error(string message, LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(null, message, LogType.Error, Trace);

			OnLog?.Invoke(this, Args);
		}

		/// <summary>
		/// Send a log error message.
		/// </summary>
		/// <param name="ex">Error exception.</param>
		/// <param name="description">Exception description.</param>
		/// <param name="target">Log deploy target.</param>
		public void Exception(Exception ex, string description = "", LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(ex, description, LogType.Exception, Trace);

			OnLog?.Invoke(this, Args);
		}


		/// <summary>
		/// Send a log critical error message.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="message">Exception description.</param>
		/// <param name="target">Log deploy target.</param>
		public void CriticalError(Exception ex, string message, LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(ex, message, LogType.CriticalError, Trace);

			OnLog?.Invoke(this, Args);
		}

		/// <summary>
		/// Send a log warning message.
		/// </summary>
		/// <param name="message">Warning message.</param>
		/// <param name="target">Log deploy target.</param>
		public void Warning(string message, LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(null, message, LogType.Warning, Trace);

			OnLog?.Invoke(this, Args);
		}

		/// <summary>
		/// Send a log information message.
		/// </summary>
		/// <param name="message">Information message.</param>
		/// <param name="target">Log deploy target.</param>
		public void Log(string message, LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(null, message, LogType.Information, Trace);

			OnLog?.Invoke(this, Args);
		}

		/// <summary>
		/// Send a log verbose message.
		/// </summary>
		/// <param name="message">Verbose message.</param>
		/// <param name="target">Log deploy target.</param>
		public void Verbose(string message, LogTarget target = LogTarget.Runtime)
		{
			StackTrace Trace = new StackTrace(1, true);

#if !DEBUG
			if (target == LogTarget.Debug)
				return;
#endif

			LogEventArgs Args = new LogEventArgs(null, message, LogType.Verbose, Trace);

			OnLog?.Invoke(this, Args);
		}



		#endregion
	}
}
