using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;

using pEngine.Common.Threading;


namespace pEngine.Debug.ReportFiles
{
    using LogFormat = Func<object, LogEventArgs, string>;

    [Flags]
    public enum ReportPolicy
    {
        /// <summary>
        /// All messages are listed in the report file.
        /// </summary>
        Verbose = 1 << 1,

        Warnings = 1 << 2,

        Errors = 1 << 3,

        None = 0
    }

    public class TextReportFile : IReportClass
    {
        static string defaultHeader = "--------------------------------------------------------\n" +
                                      "                   pEngine report file                  \n" +
                                      "--------------------------------------------------------\n";

        public TextReportFile(string filePath)
        {
            Path = filePath;
            threadWriter = new Thread(ThreadWorker);
            scheduler = new Scheduler(threadWriter);

            LogFormat = (object sender, LogEventArgs e) =>
            {
                string fileName = "";
                string functionName = "";
                int lineNumber = 0;
                int columnNumber = 0;

                if (e.Trace != null)
                {
                    var frame = e.Trace.GetFrame(1);
                    fileName = frame.GetFileName().Split('\\', '/').Last();
                    functionName = frame.GetMethod().Name;
                    lineNumber = frame.GetFileLineNumber();
                    columnNumber = frame.GetFileColumnNumber();
                }

                switch (e.Type)
                {
                    case LogType.Exception:
                        return $"!EXCEPTION! {e.Time} - {e.Exception.Message}\n\n{e.Exception}\n\n" +
                            "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n";
                    default:
                        return $"[{e.Type}] {e.Time} on ({fileName} {functionName}() {lineNumber}:{columnNumber}) - {e.Description}\n";
                }
            };

            Header = defaultHeader;
        }

        /// <summary>
        /// Path used for save the log file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets or sets the report file header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the log format.
        /// </summary>
        public LogFormat LogFormat { get; set; }

        /// <summary>
        /// Gets or sets the report level.
        /// </summary>
        public ReportPolicy ReportLevel { get; set; }

        /// <summary>
        /// Gets or sets the debug module.
        /// </summary>
        private DebugModule DebugModule { get; set; }

        /// <summary>
        /// Link the specified debugModule.
        /// </summary>
        public void Link(DebugModule debugModule)
        {
            DebugModule = debugModule;

            DebugModule.Logger.OnLog += (object sender, LogEventArgs e) =>
            {
				switch (ReportLevel)
				{
					case ReportPolicy.None:
						return;
					case ReportPolicy.Errors:
                        if (e.Type != LogType.Error)
                            return;
                        goto case ReportPolicy.Warnings;
                    case ReportPolicy.Warnings:
                        if (e.Type != LogType.Warning || e.Type != LogType.Warning)
                            return;
                        break;
			    }

                Write(LogFormat(sender, e));
            };

            threadWriter.Start();
        }

        #region File manager

        private Thread threadWriter;
        private bool canRun = true;

        private Scheduler scheduler;

        StreamWriter fileHandle;

        private void ThreadWorker()
        {
            bool makeHeader = !File.Exists(Path);

            fileHandle = new StreamWriter(new FileStream(Path, FileMode.Append));

            if (makeHeader)
                Write(Header + "\n\n");

            while(canRun)
            {
                scheduler.Update();
            }

            fileHandle.Close();
        }

        private void Write(string content)
        {
            scheduler.Add(() =>
            {
                try
                {
                    fileHandle.Write(content);
                }
                catch(Exception )
                {
                    
                }
            });
        }

        #endregion

        /// <summary>
        /// Releases all resource used by the <see cref="T:pEngine.Debug.ReportFiles.TextReportFile"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:pEngine.Debug.ReportFiles.TextReportFile"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:pEngine.Debug.ReportFiles.TextReportFile"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:pEngine.Debug.ReportFiles.TextReportFile"/> so the garbage collector can reclaim the memory
        /// that the <see cref="T:pEngine.Debug.ReportFiles.TextReportFile"/> was occupying.</remarks>
        public void Dispose()
        {
            canRun = false;
            threadWriter.Join();
        }

    }
}
