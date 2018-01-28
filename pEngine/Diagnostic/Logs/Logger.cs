using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Diagnostic.Logs
{
	public class Logger
	{
		public void Trace(string message) { }
		public void Trace(string format, params object[] args) { }
		public void Trace(Exception exception, string format, params object[] args) { }
		public bool IsTraceEnabled { get; private set; }
		public void Debug(string message) { }
		public void Debug(string format, params object[] args) { }
		public void Debug(Exception exception, string format, params object[] args) { }
		public bool IsDebugEnabled { get; private set; }
		public void Information(string message) { }
		public void Information(string format, params object[] args) { }
		public void Information(Exception exception, string format, params object[] args) { }
		public bool IsInformationEnabled { get; private set; }
		public void Warning(string message) { }
		public void Warning(string format, params object[] args) { }
		public void Warning(Exception exception, string format, params object[] args) { }
		public bool IsWarningEnabled { get; private set; }
		public void Error(string message) { }
		public void Error(string format, params object[] args) { }
		public void Error(Exception exception, string format, params object[] args) { }
		public bool IsErrorEnabled { get; private set; }
		public void Fatal(string message) { }
		public void Fatal(string format, params object[] args) { }
		public void Fatal(Exception exception, string format, params object[] args) { }
		public bool IsFatalEnabled { get; private set; }
	}
}
