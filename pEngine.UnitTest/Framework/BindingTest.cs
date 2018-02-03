using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using pEngine.Framework;
using pEngine.Framework.Binding;

namespace pEngine.UnitTest.Framework
{
    public class Circle : pObject
    {
        /// <summary>
        /// Gets or sets the circle radius.
        /// </summary>
        [Bindable(Direction = BindingMode.TwoWay)]
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the circle radius.
        /// </summary>
        [Bindable(Direction = BindingMode.ReadOnly)]
        public double RadiusReadonly { get; set; }

        /// <summary>
        /// Gets or sets the circle radius.
        /// </summary>
        [Bindable(Direction = BindingMode.TwoWay)]
        public double Diameter { get; set; }

        /// <summary>
        /// Is large if Radius > 30
        /// </summary>
        [Bindable]
        public bool Large { get; set; }

        /// <summary>
        /// Makes a new instance of <see cref="Circle"/> class.
        /// </summary>
        /// <param name="radius">Circle radius in degrees.</param>
        public Circle(double radius) : base()
        {
            Radius = radius % 360;
        }
    }


    [TestClass]
    public static class BindingTest
    {
		[TestMethod]
		public static void FrameworkBindingTwoWayBinding()
        {
            Circle c1 = new Circle(90);
            Circle c2 = new Circle(0);

            Assert.AreEqual(c1.Radius, 90);
            Assert.AreEqual(c2.Radius, 0);

            // - Apply the binding
            c1.Bind("Radius", c2, "Radius");

            Assert.AreEqual(c1.Radius, c2.Radius);

            c1.Radius = 180;

            Assert.AreEqual(c1.Radius, c2.Radius);

            c1.Radius = c2.Radius;

            Assert.AreEqual(c1.Radius, c2.Radius);

            c1.Radius += c2.Radius;

            Assert.AreEqual(c1.Radius, c2.Radius);
        }

		[TestMethod]
		public static void FrameworkBindingOneWayBinding()
        {
            Circle c1 = new Circle(90);
            Circle c2 = new Circle(0);

            Assert.AreEqual(c1.Radius, 90);
            Assert.AreEqual(c2.Radius, 0);

            try
            {
                c1.Bind("RadiusReadonly", c2, "Radius", BindingMode.ReadOnly);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }

            c1.Bind("Radius", c2, "RadiusReadonly", BindingMode.ReadOnly);

            Assert.AreEqual(c1.Radius, c2.RadiusReadonly);

            c1.Radius = 180;

            Assert.AreEqual(c1.Radius, 180);
            Assert.AreEqual(c2.RadiusReadonly, 0);

            c2.RadiusReadonly = 70;

            Assert.AreEqual(c1.Radius, c2.RadiusReadonly);
            Assert.AreEqual(c1.Radius, 70);
        }

		[TestMethod]
		public static void FrameworkBindingBindRemove()
        {
            Circle c1 = new Circle(90);
            Circle c2 = new Circle(0);

            c1.Bind("Radius", c2, "Radius");

            c2.Radius = 20;

            Assert.AreEqual(c1.Radius, c2.Radius);

            c2.Unbind("Radius", c1, "Radius");

            c1.Radius = 30;

            Assert.AreEqual(c1.Radius, 30);
            Assert.AreEqual(c2.Radius, 20);

            try
            {
                c1.Unbind("Radius", c2, "Radius");

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }
        }

		[TestMethod]
		public static void FrameworkBindingBindAdapter()
        {
            Circle c1 = new Circle(90);

            try
            {
                c1.Bind("Radius", c1, "Large");

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }

            c1.Radius = 40;

            Assert.IsFalse(c1.Large);

            c1.Bind("Radius", c1, "Large", BindingMode.TwoWay, 
                new Func<object, object>((a) => (double)a > 30), 
                new Func<object, object>((a) => (bool)a ? 100 : 0));

            Assert.IsTrue(c1.Large);

            c1.Large = false;

            Assert.AreEqual(c1.Radius, 0);
        }

		[TestMethod]
		public static void FrameworkBindingBindDispose()
		{
			Circle c1 = new Circle(90);

			using (Circle c2 = new Circle(0))
			{
				Assert.AreEqual(c1.Radius, 90);
				Assert.AreEqual(c2.Radius, 0);

				// - Apply the binding
				c1.Bind("Radius", c2, "Radius");

				Assert.AreEqual(c1.Radius, c2.Radius);
			}

			c1.Radius = 180;

		}
	}
}
