using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using pEngine.Resources;
using pEngine.Resources.Dependencies;

namespace pEngine.UnitTest.Resources
{
    public class BigResource : Resource
    {
        public BigResource(bool fail = false)
        {
            Fail = fail;
        }

        /// <summary>
        /// Check if this resource is loaded.
        /// </summary>
        public bool ImLoaded { get; set; } = false;

        /// <summary>
        /// Generate an exception during the loading.
        /// </summary>
        public bool Fail { get; set; } = false;

        protected override void OnLoad()
        {
            ImLoaded = true;

            if (Fail)
            {
                throw new Exception("Holy sh** i'm broken.");
            }
        }

        protected override bool OnAbort(PartialResource res, Exception e)
        {
            base.OnAbort(res, e);

            return true;
        }
    }

    public class DependentResource : BigResource
    {
        public DependentResource(BigResource dependence, bool fail = false)
            : base(fail)
        {
            Fail = fail;
            Dependence = dependence;
        }

        /// <summary>
        /// Dependency.
        /// </summary>
        [Dependency(Load = true, Async = true)]
        BigResource Dependence { get; }
    }


    [TestClass]
    public class ResourceTests
    {
        [TestMethod]
        public void ResourcesResourceLoadSync()
        {
            BigResource res = new BigResource(false);

            bool loadedEnter = false;

            res.Loaded += (a) => loadedEnter = true;
            res.Aborted += (a, e) => Assert.Fail();

            res.Load();

            Assert.IsTrue(res.ImLoaded);
            Assert.IsTrue(loadedEnter);

            BigResource resFail = new BigResource(true);

            bool errorEnter = false;

            resFail.Loaded += (a) => Assert.Fail();
            resFail.Aborted += (a, e) => errorEnter = true;

            resFail.Load();

            Assert.IsTrue(resFail.ImLoaded);
            Assert.IsTrue(errorEnter);
        }

        [TestMethod]
        public void ResourcesResourceLoadAsync()
        {
            BigResource res = new BigResource(false);

            bool loadedEnter = false;

            res.Loaded += (a) => loadedEnter = true;
            res.Aborted += (a, e) => Assert.Fail();

            var task = res.LoadAsync();

            task.Wait();

            Assert.IsTrue(res.ImLoaded);
            Assert.IsTrue(loadedEnter);

            BigResource resFail = new BigResource(true);

            bool errorEnter = false;

            resFail.Loaded += (a) => Assert.Fail();
            resFail.Aborted += (a, e) => errorEnter = true;

            task = resFail.LoadAsync();

            task.Wait();

            Assert.IsTrue(resFail.ImLoaded);
            Assert.IsTrue(errorEnter);
        }

        [TestMethod]
        public void ResourcesResourceDependencyLoadSync()
        {
            BigResource res = new BigResource(false);

            bool firstLoadEnter = false;

            res.Loaded += (a) => firstLoadEnter = true;
            res.Aborted += (a, e) => Assert.Fail();

            DependentResource depRes = new DependentResource(res);

            bool secondLoadEnter = false;

            depRes.Loaded += (a) => secondLoadEnter = true;
            depRes.Aborted += (a, e) => Assert.Fail();

            depRes.Load();

            Assert.IsTrue(depRes.ImLoaded);
            Assert.IsTrue(res.ImLoaded);
            Assert.IsTrue(firstLoadEnter);
            Assert.IsTrue(secondLoadEnter);
            Assert.IsTrue(depRes.State == ResourceState.Loaded);
            Assert.IsTrue(res.State == ResourceState.Loaded);

            firstLoadEnter = secondLoadEnter = false;

            BigResource resFail = new BigResource(true);

            resFail.Loaded += (a) => Assert.Fail();
            resFail.Aborted += (a, e) => firstLoadEnter = true;

            DependentResource depResFail = new DependentResource(resFail);

            depResFail.Loaded += (a) => Assert.Fail();
            depResFail.Aborted += (a, e) => secondLoadEnter = true;

            depResFail.Load();

            Assert.IsFalse(depResFail.ImLoaded);
            Assert.IsTrue(resFail.ImLoaded);
            Assert.IsTrue(firstLoadEnter);
            Assert.IsTrue(secondLoadEnter);
            Assert.IsTrue(depResFail.State == ResourceState.Aborted);
            Assert.IsTrue(resFail.State == ResourceState.Aborted);
        }

        [TestMethod]
        public void ResourcesResourceDependencyLoadAsync()
        {
            BigResource res = new BigResource(false);

            bool firstLoadEnter = false;

            res.Loaded += (a) => firstLoadEnter = true;
            res.Aborted += (a, e) => Assert.Fail();

            DependentResource depRes = new DependentResource(res);

            bool secondLoadEnter = false;

            depRes.Loaded += (a) => secondLoadEnter = true;
            depRes.Aborted += (a, e) => Assert.Fail();

            depRes.Load();

            Assert.IsTrue(depRes.ImLoaded);
            Assert.IsTrue(res.ImLoaded);
            Assert.IsTrue(firstLoadEnter);
            Assert.IsTrue(secondLoadEnter);
            Assert.IsTrue(depRes.State == ResourceState.Loaded);
            Assert.IsTrue(res.State == ResourceState.Loaded);
        }
    }
}
