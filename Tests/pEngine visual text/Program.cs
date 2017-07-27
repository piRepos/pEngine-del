using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Core;

using pEngine.Common.DataModel;

using pEngineHost = pEngine.pEngine;

namespace pEngineVisualText
{
	class Program
	{
		static void Main(string[] args)
		{

			using (var host = pEngineHost.GetHost(args, @"Visual tests"))
			{
				Game G = new VisualTest();

				host.Run(G);
			}

		}
	}
}
