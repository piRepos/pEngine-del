using System;

namespace pEngine.Games
{
	public class ServiceNotFoundException : Exception
	{
		public ServiceNotFoundException()
		{
		}

		public ServiceNotFoundException(string message)
			: base(message)
		{
		}

		public ServiceNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
