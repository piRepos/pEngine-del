using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEngine.Diagnostic.Logs
{
	public class LoggerFactory
	{
		public static Logger GetLogger<T>()
		{
			return new Logger();
		}
	}
}
