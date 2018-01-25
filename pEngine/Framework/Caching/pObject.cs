using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pEngine.Framework.Caching;

namespace pEngine.Framework
{
	public abstract partial class pObject
	{
		/// <summary>
		/// Cache storage for this object.
		/// </summary>
		public DictionaryCache Cache { get; private set; }

		private void initializeCacheModule()
		{
			Cache = new DictionaryCache();
		}

		private void disposeCacheModule()
		{
			Cache.Dispose();
		}

	}
}
