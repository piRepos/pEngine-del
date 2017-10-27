using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using pEngine.Core;
using pEngine.Core.Data;
using pEngine.Core.Graphics.Renderer;
using pEngine.Core.Graphics.Renderer.Batches;
using pEngine.Core.Graphics.Shading;
using pEngine.Core.Graphics.Textures;
using pEngine.Core.Graphics.Buffering;
using pEngine.Core.Graphics.Fonts;
using pEngine.Core.Audio;

using pEngine.Debug;

using pEngine.Common.Memory;
using pEngine.Common.Timing.Base;
using pEngine.Common.Threading;

using pEngine.Platform.Windows;
using pEngine.Platform.Monitors;

using OpenGL;

namespace pEngine
{
	using Assets = IEnumerable<Asset>;

	public class pEngine : IDisposable
    {
		#region Sigleton

		private static pEngine instance;

		/// <summary>
		/// Ture if pEngine is already hinstanced.
		/// </summary>
		public static bool Instanced => instance != null;

		/// <summary>
		/// Gets a new hinstance of pEngine host.
		/// This is the host that can run your game.
		/// </summary>
		/// <param name="args">Program arguments.</param>
		/// <param name="gameName">Game name.</param>
		/// <returns>A new hinstance of <see cref="pEngine"/>.</returns>
		public static pEngine GetHost(string[] args, string gameName)
		{
			if (instance == null)
				instance = new pEngineGLFW(args, gameName);

			return instance;
		}

		#endregion

		/// <summary>
		/// Create a new instance of <see cref="pEngine"/>.
		/// </summary>
		/// <param name="args">Program arguments.</param>
		/// <param name="gameName">Game name.</param>
		protected pEngine(string[] args, string gameName)
		{
			GameName = gameName;

			// - Inizialize system informations
			Platform.Environment.Initialize();

			// - Initialize debug module
			Debug = new DebugModule(this);

			// - Initialize asset buffer
			assetBuffer = new TripleBuffer<FrameRenderInfo>();

			// - Initialize threads
			InputLoop = new GameLoop(Input, "InputThread");
			PhysicsLoop = new ThreadedGameLoop(Update, "PhysicsThread");
			GraphicsLoop = new ThreadedGameLoop(Draw, "GraphicsThread");

			// - Loader
			Loader = new GameObjectLoader();

            // - Modules
            Fonts = new FontStore(this);
            Renderer = new Renderer(this);
			Audio = new AudioManager(this);
            Shaders = new ShaderStore(this);
			Batches = new BatchesStore(this);
            Textures = new TextureStore(this);
            Resources = new ResourceLoader(this);
            VideoBuffers = new VideoBufferStore(this);

			// - Register loader servicess
			Loader.AddService(Audio);
            Loader.AddService(Debug);
            Loader.AddService(Fonts);
            Loader.AddService(Loader);
            Loader.AddService(Batches);
            Loader.AddService(Shaders);
            Loader.AddService(Textures);
            Loader.AddService(Resources);
            Loader.AddService(VideoBuffers);

			// - Register collectors
			Debug.AddCollector("InputThreadPerformances", InputLoop.Performance);
			Debug.AddCollector("PhysicsThreadPerformances", PhysicsLoop.Performance);
			Debug.AddCollector("GraphicsThreadPerformances", GraphicsLoop.Performance);

			// - Register default shaders
			Shaders.RegisterShader(new FillShader());
			Shaders.RegisterShader(new TextShader());
			Shaders.RegisterShader(new SpriteShader());
			Shaders.RegisterShader(new FrameBufferShader());
			Shaders.RegisterShader(new MaskShader());
		}

		#region Window

		/// <summary>
		/// Gets the game window.
		/// </summary>
		public IWindow Window { get; protected set; }

		/// <summary>
		/// Current host monitor.
		/// </summary>
		public IMonitor Monitor => Window.Monitor;

		private void HookWindowEvents()
		{
			Window.OnRestore += Restore;
			Window.OnResize += Resize;
		}

		private void Resize()
		{
			PhysicsLoop.Scheduler.Add(() =>
			{
				// - Invalidate positions and sizes
				Core.Physics.Movable.UpdateMatrixViewport(Window.BufferSize);
				RunningGame.Invalidate(InvalidationType.All, InvalidationDirection.Children, RunningGame);
			});
		}

		private bool physicsInvalidation;
		private void Restore()
		{
			physicsInvalidation = true;

			GraphicsLoop.Scheduler.Add(() =>
			{
				// TODO: Invalidate renderer.
			});

			PhysicsLoop.Scheduler.Add(() =>
			{
				// - Invalidate all
				RunningGame.Invalidate(InvalidationType.All, InvalidationDirection.Children, RunningGame);
			});
		}

		#endregion

		#region Modules

		/// <summary>
		/// This contains an hinstance of <see cref="DebugModule"/>,
		/// the debug module handle all engine diagnostic informations and
		/// errors.
		/// </summary>
		public DebugModule Debug { get; }

		/// <summary>
		/// Graphics renderer of this engine; this class will provide
		/// a low level layer for rendering assets.
		/// </summary>
		public Renderer Renderer { get; }
		
		/// <summary>
		/// Batches module: this module manage all game vertexs and indexes
		/// storing all in a managed heap.
		/// </summary>
		public BatchesStore Batches { get; }

		/// <summary>
		/// This module manage the shaderl loading and instancing
		/// in runtime.
		/// </summary>
		public ShaderStore Shaders { get; }

		/// <summary>
		/// Manage all game texture resources.
		/// </summary>
		public TextureStore Textures { get; }

		/// <summary>
		/// Manage all game font resources.
		/// </summary>
		public FontStore Fonts { get; }

        /// <summary>
        /// This module manages all video buffers (framebuffers) for optimizations.
        /// </summary>
        public VideoBufferStore VideoBuffers { get; }

		/// <summary>
		/// This module can load object of type <see cref="Resource"/>.
		/// </summary>
		public ResourceLoader Resources { get; }

		/// <summary>
		/// This module manages audio devices.
		/// </summary>
		public AudioManager Audio { get; }

		#endregion

		#region Loader

		/// <summary>
		/// This module handle the game object loading.
		/// </summary>
		public GameObjectLoader Loader { get; }

		private void GameBootstrap()
		{
			// - Load game resources
			PhysicsLoop.Scheduler.Add(() =>
			{
				// - Set viewport
				Core.Physics.Movable.UpdateMatrixViewport(Window.BufferSize);
				
				// - Load resources
				Loader.LoadSync(RunningGame);
			});
		}

		#endregion

		#region Game

		/// <summary>
		/// Name of the running game.
		/// </summary>
		public string GameName { get; }

		/// <summary>
		/// Is the root of the game tree.
		/// </summary>
		public Game RunningGame { get; private set; }

		/// <summary>
		/// Run the specified game.
		/// </summary>
		/// <param name="game">Game to run.</param>
		public void Run(Game game)
		{
			// - Initializing
			RunningGame = game;
			game.Host = this;

			// - Open window
			Window.Make();
			Window.Show();
			Window.Title = GameName;

			// - Initialization
			HookWindowEvents();
			GameBootstrap();

			// - Audio initialization
			Audio.Initialize();

			// - Graphics library initialization
			GraphicsLoop.Scheduler.Add(() =>
			{
				// - Initialize openGL
				try
				{
                    if (Platform.Environment.OS.Platform == PlatformID.Unix)
                    {
						Egl.IsRequired = true;

                    }

					Gl.Initialize();
				}
				catch (Exception ex)
				{
					Debug.CriticalErrorLog(ex, "Cannot load OpenGL");
					Environment.Exit(0);
				}

				// - Prepare video context
				Window.SetContext();

				// - Initialize renderer
				Renderer.Initialize();

				// - Initialize shader manager
				Shaders.Initialize();
			});

			Debug.Log($"Starting {GameName} in a {Window.Size} window.");

			// - Run gameloops
			PhysicsLoop.Run();
			GraphicsLoop.Run();
			InputLoop.Run();
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void Close()
		{
			InputLoop.Scheduler.Add(() =>
			{
				PhysicsLoop.CurrentThread.Join();
				GraphicsLoop.CurrentThread.Join();

				InputLoop.Stop();
			});

			PhysicsLoop.Stop();
			GraphicsLoop.Stop();

			RunningGame.Dispose();

            Debug.Dispose();
		}

		#endregion

		#region Game loops

		/// <summary>
		/// This thread updates the windows and the input.
		/// </summary>
		public GameLoop InputLoop { get; }

		/// <summary>
		/// This thread updates the physics of the game tree
		/// and prepare the graphics assets for the graphics thread.
		/// </summary>
		public ThreadedGameLoop PhysicsLoop { get; }

		/// <summary>
		/// This thread updates the graphics and swap the buffer to
		/// show the game on the screen.
		/// </summary>
		public ThreadedGameLoop GraphicsLoop { get; }

		/// <summary>
		/// Bridge from physics thread and graphics thread.
		/// </summary>
		private TripleBuffer<FrameRenderInfo> assetBuffer { get; }
		
		/// <summary>
		/// Id of the last rendered frame.
		/// </summary>
		public long LastRenderedFrame { get; set; }

		/// <summary>
		/// Updates the input.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		protected virtual void Input(IFrameBasedClock clock)
		{
			using (InputLoop.Performance.StartCollect("PoolMessages"))
				Window.PollMesages(false);

			if (Window.ShouldClose)
				Close();
		}

		/// <summary>
		/// Updates game tree.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		protected virtual void Update(IFrameBasedClock clock)
		{
			Assets currAssets;

			// - Tree update
			using (PhysicsLoop.Performance.StartCollect("TreeUpdate"))
			{
				RunningGame.Update(clock);
			}

			if (physicsInvalidation)
			{
				physicsInvalidation = false;
				return;
			}

			// - Assets loading
			using (PhysicsLoop.Performance.StartCollect("AssetsCalculation"))
				currAssets = RunningGame.GetAssets();

			// - Send assets to graphics thread
			using (PhysicsLoop.Performance.StartCollect("AssetsSending"))
			{
				using (var buffer = assetBuffer.Get(UsageType.Write))
				{
					if (buffer != null)
					{
						FrameRenderInfo info = new FrameRenderInfo
						{
							Assets = currAssets,
							FrameID = PhysicsLoop.FrameId,
							Batches = Batches.GetDependencyDescriptors().ToList(),
                            Textures = Textures.GetDependencyDescriptors().ToList()
                                               .Concat(Fonts.GetDependencyDescriptors().ToList()),
                            Buffers = VideoBuffers.GetDependencyDescriptors().ToList(),
							VertexMemorySize = (int)Batches.VertexHeapSize,
							IndexMemorySize = (int)Batches.IndexHeapSize
						};

						buffer.Value = info;
					}
				}
			}
		}

		/// <summary>
		/// Update graphics and draw the scene on the screen.
		/// </summary>
		/// <param name="clock">Game loop clock.</param>
		protected virtual void Draw(IFrameBasedClock clock)
		{
			Glfw3.Glfw.SwapInterval(0);
			using (var buffer = assetBuffer.Get(UsageType.Read))
			{
				if (buffer?.Value != null && buffer.Value.FrameID != LastRenderedFrame)
				{

					// - Load meshes
					using (GraphicsLoop.Performance.StartCollect("BatchesLoading"))
					{
						// - Check batch memory size
						Renderer.Vertexs.ResizeMemory(buffer.Value.VertexMemorySize, buffer.Value.IndexMemorySize);
						
						foreach (var batch in buffer.Value.Batches)
						{
							// - Load mesh
							Renderer.Vertexs.LoadBatch(batch);

							PhysicsLoop.Scheduler.Add(() =>
							{
								// - Set mesh loaded
								Batches.SetDependency(batch);
							});
						}
					}

					// - Load textures
					using (GraphicsLoop.Performance.StartCollect("TexturesLoading"))
					{
						foreach (var texture in buffer.Value.Textures)
						{
							// - Load texture
							Renderer.Textures.DispatchTexture(texture);

							PhysicsLoop.Scheduler.Add(() =>
							{
								// - Set texture loaded
								Textures.SetDependency(texture);
							});
						}
					}

					// - Load buffers
					using (GraphicsLoop.Performance.StartCollect("BuffersLoading"))
					{
						foreach (var currBuffer in buffer.Value.Buffers)
						{
							// - Load texture
							Renderer.Buffers.DispatchDescriptor(currBuffer);

							PhysicsLoop.Scheduler.Add(() =>
							{
								// - Set texture loaded
								VideoBuffers.SetDependency(currBuffer);
							});
						}
					}

					// - Assets scheduling
					using (GraphicsLoop.Performance.StartCollect("AssetScheduling"))
					{
						// TODO: Assets scheduling
					}

					// - Clear backbuffer
					using (GraphicsLoop.Performance.StartCollect("ClearBuffer"))
					{
						Gl.Viewport(0, 0, Window.BufferSize.Width, Window.BufferSize.Height);
						Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
						Gl.ClearColor(0, 0, 0, 1);
					}

					// - Process frames
					using (GraphicsLoop.Performance.StartCollect("Drawing"))
					{
                        try
                        {
                            Gl.Enable(EnableCap.Blend);
							Gl.Enable(EnableCap.ScissorTest);
                            Gl.Enable(EnableCap.DepthTest);
                        }
                        catch (Exception) { }

						Renderer.DrawFrame(buffer.Value.Assets);

						try
						{
                            Gl.Disable(EnableCap.DepthTest);
							Gl.Disable(EnableCap.ScissorTest);
						    Gl.Disable(EnableCap.Blend);
						}
                        catch (Exception) { }
					}

					// - Swap backbuffer and frontbuffer
					using (GraphicsLoop.Performance.StartCollect("SwapBuffers"))
					{
						Window.SwapBuffer();
					}

					PhysicsLoop.Scheduler.Add(() =>
					{
						LastRenderedFrame = buffer.Value.FrameID;
					});
				}
			}
		}

		#endregion

		/// <summary>
		/// Releases all resource used by the <see cref="pEngine"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="pEngine"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="pEngine"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="pEngine"/> so
		/// the garbage collector can reclaim the memory that the <see cref="pEngine"/> was occupying.</remarks>
		public void Dispose()
		{
			Close();
		}
    }
}                                 