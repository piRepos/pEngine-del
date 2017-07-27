using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

using pEngine.Core.Graphics.Shading;

using pEngineMath = pEngine.Common.Math;

namespace pEngine.Core.Graphics.Renderer.Shading
{
    internal class GLShader
    {

		/// <summary>
		/// Makes a new <see cref="GLShader"/>.
		/// </summary>
		public GLShader(IShader shaderRef)
		{
			Shader = shaderRef;
		}

		/// <summary>
		/// Shader to load
		/// </summary>
		public IShader Shader { get; }

		/// <summary>
		/// Handler to OpenGL shader.
		/// </summary>
		public uint Program { get; private set; }

		#region Compiler

		/// <summary>
		/// True if this shader is linked and loaded correctly.
		/// </summary>
		public bool Initialized { get; private set; }

		/// <summary>
		/// Compile this shader.
		/// </summary>
		public void Compile()
		{
			if (Initialized)
				return;

			Program = Gl.CreateProgram();

			if (Shader.VertexSource.Trim() != "" && Shader.VertexSource != null)
				Compile(Shader.VertexSource, ShaderType.VertexShader);

			if (Shader.GeometrySource.Trim() != "" && Shader.GeometrySource != null)
				Compile(Shader.GeometrySource, ShaderType.GeometryShader);

			if (Shader.FragmentSource.Trim() != "" && Shader.FragmentSource != null)
				Compile(Shader.FragmentSource, ShaderType.FragmentShader);

			Link();

			VertexAttrPointer = Gl.GetAttribLocation(Program, "Vertex");
			ColorAttrPointer = Gl.GetAttribLocation(Program, "Color");
			TexCoordAttrPointer = Gl.GetAttribLocation(Program, "TexCoord");

			Initialized = true;
		}

		/// <summary>
		/// Link this shader.
		/// </summary>
		private void Link()
		{
			Gl.LinkProgram(Program);

			int Result;
			Gl.GetProgram(Program, Gl.LINK_STATUS, out Result);

			if (Result == 0)
			{
				int MsgLen = 0;
				StringBuilder ErrorString = new StringBuilder(1000);

				Gl.GetShaderInfoLog(Program, 1000, out MsgLen, ErrorString);

				throw new InvalidProgramException(ErrorString.ToString());
			}
		}

		private void Compile(string Text, ShaderType Target)
		{
			uint Handler = 0;

			if (Text != null)
			{
				Handler = Gl.CreateShader(Target);

				Gl.ShaderSource(Handler, new string[] { Text });

				// Start compile
				Gl.CompileShader(Handler);

				int Status;
				Gl.GetShader(Handler, ShaderParameterName.CompileStatus, out Status);

				if (Status == 0)
				{
					int MsgLen = 0;
					StringBuilder ErrorString = new StringBuilder(1000);

					Gl.GetShaderInfoLog(Handler, 1000, out MsgLen, ErrorString);

					throw new InvalidProgramException(ErrorString.ToString());
				}

				Gl.AttachShader(Program, Handler);
				Gl.DeleteShader(Handler);
			}
		}

		#endregion

		#region Default handles

		/// <summary>
		/// Handle to shader vertex pointer.
		/// </summary>
		public int VertexAttrPointer { get; private set; }

		/// <summary>
		/// Handle to shader color pointer.
		/// </summary>
		public int ColorAttrPointer { get; private set; }

		/// <summary>
		/// Handle to shader texture coordinates pointer.
		/// </summary>
		public int TexCoordAttrPointer { get; private set; }

		#endregion

		#region Default uniforms

		/// <summary>
		/// Apply the "ModelView" uniform to this shader.
		/// </summary>
		/// <param name="m">Model view matrix.</param>
		public void SetModelView(pEngineMath.Matrix m)
		{
			int matrixUniform = Gl.GetUniformLocation(Program, "ModelView");

			if (matrixUniform < 0)
				throw new InvalidOperationException("Uniform not found.");

			Gl.ProgramUniformMatrix4(Program, matrixUniform, 1, false, m);
		}

		#endregion

		#region Binding

		/// <summary>
		/// Bind this shader.
		/// </summary>
		public void Bind()
		{
			if (!Initialized)
				throw new InvalidOperationException("Shader not compiled.");

			Gl.UseProgram(Program);
		}

		/// <summary>
		/// Unbind this shader.
		/// </summary>
		public void Unbind()
		{
			if (!Initialized)
				throw new InvalidOperationException("Shader not compiled.");

			Gl.UseProgram(0);
		}

		#endregion
		
	}
}
