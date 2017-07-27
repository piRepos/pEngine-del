using System;
using System.Collections.Generic;
using System.Text;

using pEngine.Core.Graphics.Renderer.Shading;

namespace pEngine.Core.Graphics.Shading
{
    public class ShaderStore
    {
		private pEngine gameHost;

		/// <summary>
		/// Makes a new instance of <see cref="ShaderStore"/>.
		/// </summary>
		/// <param name="host">Engine game host.</param>
		public ShaderStore(pEngine host)
		{
			gameHost = host;

			shaders = new Dictionary<Type, GLShader>();
		}

		/// <summary>
		/// True if the shader module is initialized.
		/// </summary>
		public bool Initialized { get; private set; }

		#region Shader management

		private Dictionary<Type, GLShader> shaders;

		/// <summary>
		/// Loaded shader opengl references.
		/// </summary>
		internal IEnumerable<GLShader> ShaderReferences => shaders.Values;

		/// <summary>
		/// Loads a shader in the game engine.
		/// </summary>
		/// <remarks>
		/// This funcion must be called before the gameloop start,
		/// so before the host.Run().
		/// </remarks>
		/// <param name="shader">Shader to load.</param>
		public void RegisterShader(IShader shader)
		{
			if (Initialized)
				throw new InvalidOperationException("Can't load shaders in runtime.");

			if (shaders.ContainsKey(shader.GetType()))
				return;

			shaders.Add(shader.GetType(), new GLShader(shader));
		}

		/// <summary>
		/// Gets an instance of <see cref="{Type}"/> if is loaded.
		/// </summary>
		/// <typeparam name="Type">Shader type to get.</typeparam>
		/// <returns>An instance of the specified type.</returns>
		public Type GetShader<Type>() where Type : IShader
		{
			if (!shaders.ContainsKey(typeof(Type)))
				throw new InvalidOperationException("This shader is not registered.");

			return Activator.CreateInstance<Type>();
		}

		#endregion

		#region Loading

		/// <summary>
		/// Initialize all OpenGL
		/// </summary>
		internal void Initialize()
		{
			foreach (GLShader shader in shaders.Values)
			{
				shader.Compile();
			}

			Initialized = true;
		}

		/// <summary>
		/// Gets an instance of opengl shader for this shader.
		/// </summary>
		/// <returns>An instance of opengl shader.</returns>
		internal GLShader GetGLShader(Type shader)
		{
			return shaders[shader];
		}

		#endregion
	}
}
