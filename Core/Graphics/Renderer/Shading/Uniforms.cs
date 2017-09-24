using System;
using System.Collections.Generic;
using System.Text;

using OpenGL;

using pEngine.Common;

namespace pEngine.Core.Graphics.Renderer.Shading
{
	public struct Float1Uniform : IUniform
	{

		public Float1Uniform(string name, float value)
		{
			Value = value;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public float Value { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform1(uniform, Value);
		}

	}

	public struct Int1Uniform : IUniform
	{

		public Int1Uniform(string name, int value)
		{
			Value = value;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public int Value { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform1(uniform, Value);
		}

	}

	public struct IntUniforms : IUniform
	{

		public IntUniforms(string name, params int[] values)
		{
			Values = values;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public int[] Values { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform1(uniform, Values.Length, Values);
		}

	}

	public struct Float4Uniform : IUniform
	{

		public Float4Uniform(string name, float f1, float f2, float f3, float f4)
		{
			Float1 = f1;
			Float2 = f2;
			Float3 = f3;
			Float4 = f4;
			Name = name;
		}

		public Float4Uniform(string name, Color4 Value)
		{
			Float1 = Value.Rf;
			Float2 = Value.Gf;
			Float3 = Value.Bf;
			Float4 = Value.Af;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public float Float1 { get; }

		/// <summary>
		/// Uniform value.
		/// </summary>
		public float Float2 { get; }

		/// <summary>
		/// Uniform value.
		/// </summary>
		public float Float3 { get; }

		/// <summary>
		/// Uniform value.
		/// </summary>
		public float Float4 { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform4(uniform, Float1, Float2, Float3, Float4);
		}

	}

	public struct TextureUniform : IUniform
	{

		public TextureUniform(string name, int attachment)
		{
			Attachment = attachment;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public int Attachment { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform1(uniform, Attachment);
		}

	}

	public struct TextureUniforms : IUniform
	{

		public TextureUniforms(string name, params int[] attachments)
		{
			Attachments = attachments;
			Name = name;
		}

		/// <summary>
		/// Uniform value.
		/// </summary>
		public int[] Attachments { get; }

		/// <summary>
		/// Uniform name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Apply this uniform to a specified shader.
		/// </summary>
		/// <param name="program">Shader program.</param>
		public void Apply(uint program)
		{
			int uniform = Gl.GetUniformLocation(program, Name);
			Gl.Uniform1(uniform, Attachments.Length, Attachments);
		}

	}
}