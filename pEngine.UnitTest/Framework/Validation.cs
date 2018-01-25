using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using pEngine.Framework;
using pEngine.Framework.Validation;

namespace pEngine.UnitTest.Framework
{
	class Test : pObject
	{
		[ValidateRange(Min = 0, Max = 10)]
		public int Number { get; set; }
	}

	[TestClass]
	public class Validation
	{

		[TestMethod]
		public void FrameworkValidationValidatePredicate()
		{
			Test obj = new Test();

			Assert.AreEqual(obj.Number, 0);

			obj.Number = 5;

			Assert.AreEqual(obj.Number, 5);

			try
			{
				obj.Number = 11;

				Assert.Fail();
			}
			catch (FormatException) { }
		}
	}
}
