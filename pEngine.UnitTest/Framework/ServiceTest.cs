using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using pEngine.Framework;
using pEngine.Framework.Modules;
using pEngine.Utils.Threading;

namespace pEngine.UnitTest.Framework
{
	public delegate void TestDelegate(int test);

	public class TestModule : Module
	{
		/// <summary>
		/// Makes a new instance of <see cref="TestModule"/> class.
		/// </summary>
		public TestModule(GameHost host, GameLoop scheduler)
			: base(host, scheduler)
		{
		}

		public override Service GetSettings(GameLoop mainScheduler)
		{
			TestService t = new TestService(this, mainScheduler);
			t.Initialize();
			return t;
		}

		/// <summary>
		/// Just a test event.
		/// </summary>
		public event TestDelegate TestEventInteger;

		/// <summary>
		/// Just a test property.
		/// </summary>
		public int TestPropertyInteger { get; set; }

		/// <summary>
		/// Just a test method.
		/// </summary>
		/// <param name="integer">Wof wof.</param>
		public int TestMethodInteger(int integer)
		{
			return integer;
		}

		/// <summary>
		/// Call me!
		/// </summary>
		/// <param name="test">A number?</param>
		public void CallEvent(int test)
		{
			TestEventInteger?.Invoke(test);
		}
	}

	public class TestService : Service
	{
		/// <summary>
		/// Makes a new instance of <see cref="TestService"/> class.
		/// </summary>
		/// <param name="module">Owner module.</param>
		public TestService(TestModule module, GameLoop mainScheduler)
			: base(module, mainScheduler)
		{

		}

		/// <summary>
		/// Just a test property.
		/// </summary>
		[ServiceProperty("TestPropertyInteger")]
		public int PropertyInteger { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="integer"></param>
		/// <param name="callback"></param>
		[ServiceMethod(ReferencesTo = "TestMethodInteger")]
		public void MethodInteger(int integer, Action<object> callback) { }

		/// <summary>
		/// Just a test event.
		/// </summary>
		[ServiceEvent("TestEventInteger")]
		public event TestDelegate EventInteger;
	}

	/// <summary>
	/// Descrizione del riepilogo per ServiceTest
	/// </summary>
	[TestClass]
	public class ServiceTest
	{
		public ServiceTest()
		{
			Scheduler = new ThreadedGameLoop(null, "TestThread1");
			PrimaryScheduler = new ThreadedGameLoop(null, "TestThread2");
			Module = new TestModule(null, Scheduler);
			Settings = Module.GetSettings(PrimaryScheduler) as TestService;
		}

		private TestContext testContextInstance;

		/// <summary>
		/// Ottiene o imposta il contesto del test che fornisce
		/// le informazioni e le funzionalità per l'esecuzione del test corrente.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		/// <summary>
		/// Module to test.
		/// </summary>
		public TestModule Module { get; }

		/// <summary>
		/// Module settings.
		/// </summary>
		public TestService Settings { get; }

		/// <summary>
		/// Scheduler.
		/// </summary>
		public ThreadedGameLoop Scheduler { get; }

		/// <summary>
		/// Scheduler.
		/// </summary>
		public ThreadedGameLoop PrimaryScheduler { get; }

		#region Attributi di test aggiuntivi
		//
		// È possibile utilizzare i seguenti attributi aggiuntivi per la scrittura dei test:
		//
		// Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test della classe
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }

		/// <summary>
		/// Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test 
		/// </summary>
		[TestInitialize()]
		public void MyTestInitialize()
		{
			Module.Initialize();
			Scheduler.Run();
			PrimaryScheduler.Run();
		}
		
		/// <summary>
		/// Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
		/// </summary>
		[TestCleanup()]
		public void MyTestCleanup()
		{
			Scheduler.Stop();
			Scheduler.CurrentThread.Join();
			PrimaryScheduler.Stop();
			PrimaryScheduler.CurrentThread.Join();
		}
		
		#endregion

		[TestMethod]
		public void FrameworkModuleServicePropertySet()
		{
			ManualResetEvent sem = new ManualResetEvent(false);

			Settings.PropertyInteger = 2;

			Scheduler.Scheduler.Add(() =>
			{
				Assert.AreEqual(Module.TestPropertyInteger, 2);
				sem.Set();
			});

			sem.WaitOne();
			sem.Reset();

			Module.TestPropertyInteger = 10;

			PrimaryScheduler.Scheduler.Add(() =>
			{
				Assert.AreEqual(Settings.PropertyInteger, 10);
				sem.Set();
			});

			sem.WaitOne();
		}

		[TestMethod]
		public void FrameworkModuleServiceMethod()
		{
			ManualResetEvent sem = new ManualResetEvent(false);

			Settings.MethodInteger(30, (ret) =>
			{
				int num = (int)ret;
				Assert.AreEqual(num, 30);
				sem.Set();
			});

			sem.WaitOne();
		}

		[TestMethod]
		public void FrameworkModuleServiceEvent()
		{
			ManualResetEvent sem = new ManualResetEvent(false);

			Settings.EventInteger += (a) =>
			{
				Assert.AreEqual(a, 12);
				sem.Set();
			};

			Module.CallEvent(12);

			sem.WaitOne();
		}
		
	}
}
