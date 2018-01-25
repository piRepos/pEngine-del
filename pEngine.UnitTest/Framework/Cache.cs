using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using pEngine.Framework;
using pEngine.Framework.Caching;

namespace pEngine.UnitTest.Framework
{
	[TestClass]
	public class Cache : pObject
	{
		int index = 0;

		[Cached(Channel = "InvalidationChannel")]
		int SlowAndCPUBoundFunction()
		{
			return index++;
		}

		[Cached(Channel = "InvalidationChannel")]
		void VoidSlowAndCPUBoundFunction()
		{
			if (index++ == 2)
				Assert.Fail();
		}

		[TestMethod]
		public void FrameworkCacheCheckValues()
		{
			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);
			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);
			Cache.Invalidate("InvalidationChannel");
			Assert.AreEqual(SlowAndCPUBoundFunction(), 1);
			Assert.AreEqual(SlowAndCPUBoundFunction(), 1);
			Cache.Invalidate("InvalidationChannel");
			Assert.AreEqual(SlowAndCPUBoundFunction(), 2);
			Assert.AreEqual(SlowAndCPUBoundFunction(), 2);
		}

		[TestMethod]
		public void FrameworkCacheVoidCall()
		{
			VoidSlowAndCPUBoundFunction();
			VoidSlowAndCPUBoundFunction();
			Cache.Invalidate("InvalidationChannel");
			VoidSlowAndCPUBoundFunction();
			VoidSlowAndCPUBoundFunction();
		}

		[TestMethod]
		public void FrameworkCacheWrongInvalidation()
		{
			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);
			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);

			try
			{
				Cache.Invalidate("WorngInvalidation");
				Assert.Fail();
			}
			catch (KeyNotFoundException) { }

			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);
			Assert.AreEqual(SlowAndCPUBoundFunction(), 0);
		}
	}
}
